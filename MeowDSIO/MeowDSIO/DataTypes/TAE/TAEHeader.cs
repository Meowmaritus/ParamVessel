using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE
{
    public class TAEHeader
    {
        [ReadOnly(true)]
        public bool IsBigEndian { get; set; } = false;

        //3 Null bytes

        public int Version = 0x1000B;

        public const int VERSION_01_11 = 0x1000B;
        public const int VERSION_00_01 = 0x1;

        //(uint FileLength)

        public uint UnknownB00 { get; set; } = 64; //0x00000040
        public uint UnknownB01 { get; set; } = 1; //0x00000001
        public uint UnknownB02 { get; set; } = 80; //0x00000050
        public uint UnknownB03 { get; set; } = 112; //0x00000070

        public const int UnknownFlagsLength = 0x30;

        public byte[] UnknownFlags { get; set; } = new byte[UnknownFlagsLength]
        {
            0x00, 0x01, 0x00, 0x02,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x02, 0x01, 0x00, 0x01,
            0x02, 0x01, 0x00, 0x02,
            0x00, 0x00, 0x00, 0x01,
            0x00, 0x00, 0x00, 0x00,
        };

        public int FileID { get; set; } = 204100;
        public int UnknownC { get; set; } = 0x00000090;

        public uint UnknownE00 { get; set; } = 0;
        public uint UnknownE01 { get; set; } = 1;
        public uint UnknownE02 { get; set; } = 128;
        public uint UnknownE03 { get; set; } = 0;
        public uint UnknownE04 { get; set; } = 0;

        public int FileID2 { get; set; } = 204100;
        public int FileID3 { get; set; } = 204100;

        public uint UnknownE07 { get; set; } = 0x00000050u;
        public uint UnknownE08 { get; set; } = 0x00000000u;
        public uint UnknownE09 { get; set; } = 0x00000000u;
    }
}
