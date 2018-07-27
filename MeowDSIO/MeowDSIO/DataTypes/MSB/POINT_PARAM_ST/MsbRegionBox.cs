using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.POINT_PARAM_ST
{
    public class MsbRegionBox : MsbRegionBase
    {
        public int UNK1 { get; set; } = 0;
        public int UNK2 { get; set; } = 0;
        public float Length { get; set; } = 1;
        public float Width { get; set; } = 1;
        public float Height { get; set; } = 1;
        public int EventEntityID { get; set; } = -1;

        internal override (int, int, int) GetOffsetDeltas()
        {
            return (4, 8, 20);
        }

        internal override PointParamSubtype GetSubtypeValue()
        {
            return PointParamSubtype.Boxes;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            UNK1 = bin.ReadInt32();
            UNK2 = bin.ReadInt32();
            Length = bin.ReadSingle();
            Width = bin.ReadSingle();
            Height = bin.ReadSingle();
            EventEntityID = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(UNK1);
            bin.Write(UNK2);
            bin.Write(Length);
            bin.Write(Width);
            bin.Write(Height);
            bin.Write(EventEntityID);
        }
    }
}
