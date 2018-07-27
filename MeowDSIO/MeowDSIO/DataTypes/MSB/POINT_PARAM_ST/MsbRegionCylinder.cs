using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.POINT_PARAM_ST
{
    public class MsbRegionCylinder : MsbRegionBase
    {
        public int UNK1 { get; set; } = 0;
        public int UNK2 { get; set; } = 0;
        public float Radius { get; set; } = 1;
        public float Height { get; set; } = 2;
        public int EventEntityID { get; set; } = -1;

        internal override (int, int, int) GetOffsetDeltas()
        {
            return (4, 8, 16);
        }

        internal override PointParamSubtype GetSubtypeValue()
        {
            return PointParamSubtype.Cylinders;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            UNK1 = bin.ReadInt32();
            UNK2 = bin.ReadInt32();
            Radius = bin.ReadSingle();
            Height = bin.ReadSingle();
            EventEntityID = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(UNK1);
            bin.Write(UNK2);
            bin.Write(Radius);
            bin.Write(Height);
            bin.Write(EventEntityID);
        }
    }
}
