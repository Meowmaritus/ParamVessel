using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO
{
    public class DSBinaryWriter : BinaryWriter
    {
        public string FileName { get; private set; }

        public DSBinaryWriter(string fileName, Stream output)
            : base(output)
        {
            FileName = fileName;
        }

        public DSBinaryWriter(string fileName, Stream output, Encoding encoding)
            : base(output, encoding)
        {
            FileName = fileName;
        }

        public DSBinaryWriter(string fileName, Stream output, Encoding encoding, bool leaveOpen)
            : base(output, encoding, leaveOpen)
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

        public char StrEscapeChar = (char)0;

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

        public void StepIntoPaddedRegion(long length, byte padding)
        {
            PaddedRegionStack.Push(new PaddedRegion(Position, length, padding));
        }

        public void StepOutOfPaddedRegion()
        {
            if (PaddedRegionStack.Count == 0)
                throw new InvalidOperationException("You cannot step out of padded region unless inside of one " + 
                    $"as a result of previously calling {nameof(StepIntoPaddedRegion)}().");

            var deepestPaddedRegion = PaddedRegionStack.Pop();
            deepestPaddedRegion.AdvanceWriterToEnd(this);
        }


        public void WritePaddedStringShiftJIS(string str, int paddedRegionLength, byte? padding, bool forceTerminateAtMaxLength = false)
        {
            byte[] jis = ShiftJISEncoding.GetBytes(str);
            int origSize = jis.Length;
            Array.Resize(ref jis, paddedRegionLength);

            if (paddedRegionLength > origSize)
            {
                if (padding.HasValue)
                {
                    // Start at [origSize + 1] because [origSize] is the null-terminator
                    for (int i = origSize + 1; i < paddedRegionLength; i++)
                    {
                        jis[i] = padding.Value;
                    }
                }
            }
            else if (paddedRegionLength < origSize && forceTerminateAtMaxLength)
            {
                jis[jis.Length - 1] = 0;
            }

            Write(jis);
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

        private void WritePreparedBytes(byte[] b)
        {
            base.Write(PrepareBytes(b));
        }

        public override void Write(char[] chars)
        {
            if (!BigEndian)
            {
                base.Write(chars);
                return;
            }

            for (int i = 0; i < chars.Length; i++)
            {
                WritePreparedBytes(BitConverter.GetBytes(chars[i]));
            }
        }
        public override void Write(long value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }
        public override void Write(uint value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }
        public override void Write(int value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }
        public override void Write(ushort value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }
        public override void Write(short value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }
        public override void Write(decimal value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            int[] chunks = Decimal.GetBits(value);
            byte[] bytes = new byte[16];

            for (int i = 0; i < 16; i += 4)
            {
                byte[] b = BitConverter.GetBytes(chunks[i / 4]);
                for (int j = 0; j < 4; j++)
                {
                    bytes[i + j] = b[j];
                }
            }

            WritePreparedBytes(bytes);
        }
        public override void Write(double value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }
        public override void Write(float value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }
        public override void Write(char ch)
        {
            if (!BigEndian)
            {
                base.Write(ch);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(ch));
        }
        private new void Write(string value)
        {
            //Make generic string write method unavailable to the outside world.
            //Caller must instead call one of the more specific string write methods.
        }
        public override void Write(ulong value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }

        public void Write(Vector2 value)
        {
            Write(value.X);
            Write(value.Y);
        }

        public void Write(Vector3 value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }

        public void Write(Vector4 value)
        {
            Write(value.W);
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }

        public void Write(byte[] value, int specificLength)
        {
            Array.Resize(ref value, specificLength);
            Write(value);
        }

        /// <summary>
        /// Writes an ASCII string directly without padding or truncating it.
        /// </summary>
        /// <param name="str">The string to write.</param>
        /// <param name="terminate">Whether to append a string terminator character of value 0 to the end of the written string.</param>
        public void WriteStringAscii(string str, bool terminate)
        {
            byte[] valueBytes = new byte[terminate ? str.Length + 1 : str.Length];
            Encoding.ASCII.GetBytes(str, 0, str.Length, valueBytes, 0);
            Write(valueBytes);
        }

        /// <summary>
        /// Writes a Shift-JIS string.
        /// </summary>
        /// <param name="str">The string to write.</param>
        /// /// <param name="terminate">Whether to append a string terminator character of value 0 to the end of the written string.</param>
        public void WriteStringShiftJIS(string str, bool terminate)
        {
            byte[] b = ShiftJISEncoding.GetBytes(str);
            if (terminate)
                Array.Resize(ref b, b.Length + 1);

            Write(b);
        }

        public void Pad(int align)
        {
            var off = Position % align;
            if (off > 0)
            {
                var correct = align - off;
                while (correct-- > 0)
                    Write((Byte)0);
            }
        }

        public void WriteDelimiter(byte val)
        {
            Write(val);
            Pad(4);
        }

        public void WriteMtdName(string name, byte delim)
        {
            byte[] shift_jis = ShiftJISEncoding.GetBytes(name);

            Write(shift_jis.Length);

            Write(shift_jis);

            WriteDelimiter(delim);
        }

        #endregion

    }
}
