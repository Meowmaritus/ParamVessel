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
    public partial class DSBinaryReader : BinaryReader
    {
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
        /// Reads an ASCII string.
        /// </summary>
        /// <param name="length">If non-null, reads the specified number of characters. 
        /// <para/>If null, reads characters until it reaches a control character of value 0 (and this 0-value is excluded from the returned string).</param>
        /// <returns>An ASCII string.</returns>
        public string ReadStringUnicode(int? length = null)
        {
            if (length.HasValue)
            {
                if (BigEndian)
                    return Encoding.BigEndianUnicode.GetString(ReadBytes(length.Value * 2));
                else
                    return Encoding.Unicode.GetString(ReadBytes(length.Value * 2));
            }
            else
            {
                var sb = new StringBuilder();

                byte[] nextBytes = new byte[] { 0, 0 };

                while (true)
                {
                    nextBytes = ReadBytes(2);

                    if (nextBytes[0] != 0 || nextBytes[1] != 0)
                    {
                        if (BigEndian)
                            sb.Append(Encoding.BigEndianUnicode.GetChars(nextBytes));
                        else
                            sb.Append(Encoding.Unicode.GetChars(nextBytes));
                    }
                    else
                    {
                        break;
                    }
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

        public byte ReadMtdDelimiter()
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
            delim = ReadMtdDelimiter();
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
