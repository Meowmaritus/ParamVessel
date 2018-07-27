using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST
{
    public class MsbPartsObject : MsbPartsBase
    {
        public int UNK1 { get; set; } = 0;

        internal int PartIndex { get; set; } = 0;

        public string CollisionName { get; set; } = "";

        public int UNK2 { get; set; } = 0;

        public short UNK3 { get; set; } = 0;
        public short UNK4 { get; set; } = 0;

        public int UNK5 { get; set; } = 0;
        public int UNK6 { get; set; } = 0;


        internal override PartsParamSubtype GetSubtypeValue()
        {
            return PartsParamSubtype.Objects;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            UNK1 = bin.ReadInt32();
            PartIndex = bin.ReadInt32();
            UNK2 = bin.ReadInt32();

            UNK3 = bin.ReadInt16();
            UNK4 = bin.ReadInt16();

            UNK5 = bin.ReadInt32();
            UNK6 = bin.ReadInt32();

        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(UNK1);
            bin.Write(PartIndex);
            bin.Write(UNK2);

            bin.Write(UNK3);
            bin.Write(UNK4);

            bin.Write(UNK5);
            bin.Write(UNK6);
        }
    }
}
