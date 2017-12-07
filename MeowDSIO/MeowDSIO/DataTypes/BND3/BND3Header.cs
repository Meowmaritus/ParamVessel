using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.BND3
{
    public class BND3Header
    {
        public const int Version_ByteLength = 0x0C;
        public const string Version_RootSig = "BND3";
        public string Version { get; set; }
        public byte Format { get; set; }
        public bool IsBigEndian_Maybe { get; set; }
        public bool IsPS3_Maybe { get; set; }
        public byte UnkFlag01 { get; set; }

        public const int UnknownBytes01_Length = 0x0C;
        public byte[] UnknownBytes01 { get; set; }


        public static readonly byte[] SupportedFormatValues = new byte[]
        {
            //DeS:
            0x00,
            0x0E,
            0x2E,

            //DaS PC:
            0x70,
            0x74,
            0x54,
        };
    }
}
