using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO
{
    public partial class DSBinaryWriter : BinaryWriter
    {
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

        public void WriteStringUnicode(string str, bool terminate)
        {
            byte[] valueBytes = new byte[terminate ? ((str.Length * 2) + 2) : (str.Length * 2)];

            if (BigEndian)
                Encoding.BigEndianUnicode.GetBytes(str, 0, str.Length, valueBytes, 0);
            else
                Encoding.Unicode.GetBytes(str, 0, str.Length, valueBytes, 0);

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
    }
}
