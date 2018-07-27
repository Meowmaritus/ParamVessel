using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventTreasure : MsbEventBase
    {
        public int SUx00 { get; set; } = 0;
        public int SUx04 { get; set; } = 0;
        public int PartIndex2 { get; set; } = 0;
        public int ItemLot1 { get; set; } = 0;
        public int SUx0C { get; set; } = 0;
        public int ItemLot2 { get; set; } = 0;
        public int SUx14 { get; set; } = 0;
        public int ItemLot3 { get; set; } = 0;
        public int SUx1C { get; set; } = 0;
        public int ItemLot4 { get; set; } = 0;
        public int SUx24 { get; set; } = 0;
        public int ItemLot5 { get; set; } = 0;
        public int SUx2C { get; set; } = 0;
        public int SUx30 { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.Treasures;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            SUx00 = bin.ReadInt32();
            SUx04 = bin.ReadInt32();
            PartIndex2 = bin.ReadInt32();
            ItemLot1 = bin.ReadInt32();
            SUx0C = bin.ReadInt32();
            ItemLot2 = bin.ReadInt32();
            SUx14 = bin.ReadInt32();
            ItemLot3 = bin.ReadInt32();
            SUx1C = bin.ReadInt32();
            ItemLot4 = bin.ReadInt32();
            SUx24 = bin.ReadInt32();
            ItemLot5 = bin.ReadInt32();
            SUx2C = bin.ReadInt32();
            SUx30 = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(SUx00);
            bin.Write(SUx04);
            bin.Write(PartIndex2);
            bin.Write(ItemLot1);
            bin.Write(SUx0C);
            bin.Write(ItemLot2);
            bin.Write(SUx14);
            bin.Write(ItemLot3);
            bin.Write(SUx1C);
            bin.Write(ItemLot4);
            bin.Write(SUx24);
            bin.Write(ItemLot5);
            bin.Write(SUx2C);
            bin.Write(SUx30);
        }
    }
}
