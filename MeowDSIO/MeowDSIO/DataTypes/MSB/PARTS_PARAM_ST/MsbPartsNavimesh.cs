using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST
{
    public class MsbPartsNavimesh : MsbPartsBase
    {
        public int NaviMeshGroup1 { get; set; } = 0;
        public int NaviMeshGroup2 { get; set; } = 0;
        public int NaviMeshGroup3 { get; set; } = 0;
        public int NaviMeshGroup4 { get; set; } = 0;

        public int UNK1 { get; set; } = 0;
        public int UNK2 { get; set; } = 0;
        public int UNK3 { get; set; } = 0;
        public int UNK4 { get; set; } = 0;

        internal override PartsParamSubtype GetSubtypeValue()
        {
            return PartsParamSubtype.Navimeshes;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            NaviMeshGroup1 = bin.ReadInt32();
            NaviMeshGroup2 = bin.ReadInt32();
            NaviMeshGroup3 = bin.ReadInt32();
            NaviMeshGroup4 = bin.ReadInt32();

            UNK1 = bin.ReadInt32();
            UNK2 = bin.ReadInt32();
            UNK3 = bin.ReadInt32();
            UNK4 = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(NaviMeshGroup1);
            bin.Write(NaviMeshGroup2);
            bin.Write(NaviMeshGroup3);
            bin.Write(NaviMeshGroup4);

            bin.Write(UNK1);
            bin.Write(UNK2);
            bin.Write(UNK3);
            bin.Write(UNK4);
        }
    }
}
