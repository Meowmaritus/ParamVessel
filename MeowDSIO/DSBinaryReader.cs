using MeowDSIO.Exceptions.DSRead;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO
{
    public class DSBinaryReader : BinaryReader
    {
        public string FileName { get; private set; }

        public DSBinaryReader(string fileName, Stream input)
            : base(input)
        {
            FileName = fileName;
        }

        public DSBinaryReader(string fileName, Stream input, Encoding encoding)
            : base(input, encoding)
        {
            FileName = fileName;
        }

        public DSBinaryReader(string fileName, Stream input, Encoding encoding, bool leaveOpen)
            : base(input, encoding, leaveOpen)
        {
            FileName = fileName;
        }

        private static Encoding ShiftJISEncoding = Encoding.GetEncoding("shift_jis");

        public long Position { get => BaseStream.Position; set => BaseStream.Position = value; }
        public long Length => BaseStream.Length;
        public void Goto(long absoluteOffset) => BaseStream.Seek(absoluteOffset, SeekOrigin.Begin);
        public void Jump(long relativeOffset) => BaseStream.Seek(relativeOffset, SeekOrigin.Current);
        private Stack<long> StepStack = new Stack<long>();
        private Stack<PaddedRegion> PaddedRegionStack = new Stack<PaddedRegion>();

        public bool BigEndian = false;

        public void StepIn(long offset)
        {
            StepStack.Push(Position);
            Goto(offset);
        }

        public void StepOut()
        {
            if (StepStack.Count == 0)
                throw new InvalidOperationException("You cannot step out unless StepIn() was previously called on an offset.");

            Goto(StepStack.Pop());
        }

        public void StepIntoPaddedRegion(long length, byte? padding)
        {
            PaddedRegionStack.Push(new PaddedRegion(Position, length, padding));
        }

        public void StepOutOfPaddedRegion()
        {
            if (PaddedRegionStack.Count == 0)
                throw new InvalidOperationException("You cannot step out of padded region unless inside of one.");

            var deepestPaddedRegion = PaddedRegionStack.Pop();
            deepestPaddedRegion.AdvanceReaderToEnd(this);
        }

        public void StepOutOfPaddedRegion(out byte foundPadding)
        {
            if (PaddedRegionStack.Count == 0)
                throw new InvalidOperationException("You cannot step out of padded region unless inside of one.");

            var deepestPaddedRegion = PaddedRegionStack.Pop();
            deepestPaddedRegion.AdvanceReaderToEnd(this, out foundPadding);
        }

        public void DoAt(long offset, Action doAction)
        {
            StepIn(offset);
            doAction();
            StepOut();
        }

        #region Endianness
        private byte[] PrepareBytes(byte[] b)
        {
            //Check if the BitConverter is expecting little endian
            if (BitConverter.IsLittleEndian)
            {
                //It's expecting little endian so we must reverse our big endian bytes
                Array.Reverse(b);
            }
            return b;
        }

        private byte[] GetPreparedBytes(int count)
        {
            byte[] b = base.ReadBytes(count);
            return PrepareBytes(b);
        }

        public override char ReadChar()
        {
            if (!BigEndian)
                return base.ReadChar();

            return BitConverter.ToChar(GetPreparedBytes(2), 0);
        }
        public override char[] ReadChars(int count)
        {
            if (!BigEndian)
                return base.ReadChars(count);

            char[] chr = new char[count];

            for (int i = 0; i < count; i++)
            {
                chr[i] = BitConverter.ToChar(GetPreparedBytes(2), 0);
            }

            return chr;
        }
        public override decimal ReadDecimal()
        {
            if (!BigEndian)
                return base.ReadDecimal();

            byte[] b = GetPreparedBytes(16);
            int[] chunks = new int[4];

            for (int i = 0; i < 16; i += 4)
            {
                chunks[i / 4] = BitConverter.ToInt32(b, i);
            }

            return new decimal(chunks);
        }
        public override double ReadDouble()
        {
            if (!BigEndian)
                return base.ReadDouble();

            return BitConverter.ToDouble(GetPreparedBytes(8), 0);
        }
        public override short ReadInt16()
        {
            if (!BigEndian)
                return base.ReadInt16();

            return BitConverter.ToInt16(GetPreparedBytes(2), 0);
        }
        public override int ReadInt32()
        {
            if (!BigEndian)
                return base.ReadInt32();

            return BitConverter.ToInt32(GetPreparedBytes(4), 0);
        }
        public override long ReadInt64()
        {
            if (!BigEndian)
                return base.ReadInt64();

            return BitConverter.ToInt64(GetPreparedBytes(8), 0);
        }
        public override float ReadSingle()
        {
            if (!BigEndian)
                return base.ReadSingle();

            return BitConverter.ToSingle(GetPreparedBytes(4), 0);
        }
        public override ushort ReadUInt16()
        {
            if (!BigEndian)
                return base.ReadUInt16();

            return BitConverter.ToUInt16(GetPreparedBytes(2), 0);
        }
        public override uint ReadUInt32()
        {
            if (!BigEndian)
                return base.ReadUInt32();

            return BitConverter.ToUInt32(GetPreparedBytes(4), 0);
        }
        public override ulong ReadUInt64()
        {
            if (!BigEndian)
                return base.ReadUInt64();

            return BitConverter.ToUInt64(GetPreparedBytes(4), 0);
        }
        #endregion


        /// <summary>
        /// Reads an ASCII string.
        /// </summary>
        /// <param name="length">If non-null, reads the specified number of characters. 
        /// <para/>If null, reads characters until it reaches a control character of value 0 (and this 0-value is excluded from the returned string).</param>
        /// <returns>An ASCII string.</returns>
        public string ReadStringAscii(int? length = null)
        {
            if (length.HasValue)
            {
                return Encoding.ASCII.GetString(ReadBytes(length.Value));
            }
            else
            {
                var sb = new StringBuilder();

                byte[] nextByte = new byte[] { 0 };

                while (true)
                {
                    nextByte[0] = ReadByte();

                    if (nextByte[0] > 0)
                        sb.Append(Encoding.ASCII.GetChars(nextByte));
                    else
                        break;
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Reads a Shift-JIS string.
        /// </summary>
        /// <returns>A Shift-JIS string.</returns>
        public string ReadStringShiftJIS(int specificLength = -1, bool stopOnTerminator = true)
        {
            if (!stopOnTerminator)
                return ShiftJISEncoding.GetString(ReadBytes(specificLength).ToArray());

            List<byte> shiftJisData = new List<byte>();

            byte nextByte = 0;

            while (specificLength < 0 || shiftJisData.Count < specificLength)
            {
                nextByte = ReadByte();

                if (stopOnTerminator && nextByte == 0)
                    break;

                shiftJisData.Add(nextByte);
            }

            return ShiftJISEncoding.GetString(shiftJisData.ToArray());
        }

        public string ReadStringShiftJIS(int specificLength)
        {
            return ReadStringShiftJIS(specificLength, false);
        }


        public void Pad(int align)
        {
            var off = Position % align;
            if (off > 0)
            {
                ReadBytes((int)(align - off));
            }
        }

        public byte ReadDelimiter()
        {
            byte result = ReadByte();
            Pad(4);
            return result;
        }

        public Vector2 ReadVector2()
        {
            float x = ReadSingle();
            float y = ReadSingle();
            return new Vector2(x, y);
        }

        public Vector3 ReadVector3()
        {
            float x = ReadSingle();
            float y = ReadSingle();
            float z = ReadSingle();
            return new Vector3(x, y, z);
        }

        public Vector4 ReadVector4()
        {
            float w = ReadSingle();
            float x = ReadSingle();
            float y = ReadSingle();
            float z = ReadSingle();
            return new Vector4(w, x, y, z);
        }

        public string ReadMtdName(out byte delim)
        {
            int valLength = ReadInt32();
            string result = ReadStringShiftJIS(valLength);
            delim = ReadDelimiter();
            return result;
        }

        public string ReadMtdName()
        {
            return ReadMtdName(out _);
        }

        public byte ReadByte(string valName, params byte[] checkValues)
        {
            byte val = ReadByte();
            if (!checkValues.Contains(val))
            {
                throw new Exception($"Unexpected value found for {valName}: {val}");
            }
            return val;
        }

        public TVal CheckConsumeValue<TVal>(
            string ValueNameStr,
            Func<TVal> ReadFunction, 
            TVal ExpectedValue)
            where TVal : IEquatable<TVal>
        {
            TVal ConsumedValue = ReadFunction();
            
            if (!((IEquatable<TVal>)ConsumedValue).Equals(ExpectedValue))
            {
                throw new ConsumeValueCheckFailedException<TVal>(this, ValueNameStr, ExpectedValue, ConsumedValue);
            }

            return ConsumedValue;
        }

        public string ReadPaddedStringShiftJIS(int paddedRegionLength, byte? padding)
        {
            byte[] data = ReadBytes(paddedRegionLength);
            int strEndIndex = -1;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0)
                {
                    strEndIndex = i;
                    break;
                }
            }

            // String has null-terminator in it
            if (strEndIndex > 0)
            {
                return ShiftJISEncoding.GetString(data, 0, strEndIndex);
            }
            // String begins with null-terminator
            else if (strEndIndex == 0)
            {
                // If string ends on very first byte, there's no real point to 
                // running it through the encoding and all that.
                return string.Empty;
            }
            // String has no null-terminator
            else
            {
                return ShiftJISEncoding.GetString(data);
            }
        }
    }
}
