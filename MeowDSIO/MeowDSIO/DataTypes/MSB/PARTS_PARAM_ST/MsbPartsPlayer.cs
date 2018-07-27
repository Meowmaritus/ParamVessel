using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST
{
    public class MsbPartsPlayer : MsbPartsBase
    {
        public int UNK1 { get; set; } = 0;
        public int UNK2 { get; set; } = 0;
        public int UNK3 { get; set; } = 0;
        public int UNK4 { get; set; } = 0;

        internal override PartsParamSubtype GetSubtypeValue()
        {
            return PartsParamSubtype.Players;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            UNK1 = bin.ReadInt32();
            UNK2 = bin.ReadInt32();
            UNK3 = bin.ReadInt32();
            UNK4 = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(UNK1);
            bin.Write(UNK2);
            bin.Write(UNK3);
            bin.Write(UNK4);
        }
    }
}
