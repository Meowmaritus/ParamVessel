using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST
{
    public class MsbPartsCollisionUnused : MsbPartsBase
    {
        public byte UNK1 { get; set; } = 0;
        public byte UNK2 { get; set; } = 0;
        public byte UNK3 { get; set; } = 0;
        public byte UNK4 { get; set; } = 0;

        public short UNK5 { get; set; } = 0;
        public short UNK6 { get; set; } = 0;

        public int UNK7 { get; set; } = 0;
        public int UNK8 { get; set; } = 0;

        internal override PartsParamSubtype GetSubtypeValue()
        {
            return PartsParamSubtype.UnusedCollisions;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            UNK1 = bin.ReadByte();
            UNK2 = bin.ReadByte();
            UNK3 = bin.ReadByte();
            UNK4 = bin.ReadByte();

            UNK5 = bin.ReadInt16();
            UNK6 = bin.ReadInt16();

            UNK7 = bin.ReadInt32();
            UNK8 = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(UNK1);
            bin.Write(UNK2);
            bin.Write(UNK3);
            bin.Write(UNK4);

            bin.Write(UNK5);
            bin.Write(UNK6);

            bin.Write(UNK7);
            bin.Write(UNK8);
        }
    }
}
