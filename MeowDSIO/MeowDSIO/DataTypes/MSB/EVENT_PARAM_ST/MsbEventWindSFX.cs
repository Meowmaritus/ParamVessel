using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventWindSFX : MsbEventBase
    {
        public float SUx00 { get; set; } = 0;
        public float SUx04 { get; set; } = 0;
        public float SUx08 { get; set; } = 0;
        public float SUx0C { get; set; } = 0;
        public float SUx10 { get; set; } = 0;
        public float SUx14 { get; set; } = 0;
        public float SUx18 { get; set; } = 0;
        public float SUx1C { get; set; } = 0;
        public float SUx20 { get; set; } = 0;
        public float SUx24 { get; set; } = 0;
        public float SUx28 { get; set; } = 0;
        public float SUx2C { get; set; } = 0;
        public float SUx30 { get; set; } = 0;
        public float SUx34 { get; set; } = 0;
        public float SUx38 { get; set; } = 0;
        public float SUx3C { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.WindSFX;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            SUx00 = bin.ReadSingle();
            SUx04 = bin.ReadSingle();
            SUx08 = bin.ReadSingle();
            SUx0C = bin.ReadSingle();
            SUx10 = bin.ReadSingle();
            SUx14 = bin.ReadSingle();
            SUx18 = bin.ReadSingle();
            SUx1C = bin.ReadSingle();
            SUx20 = bin.ReadSingle();
            SUx24 = bin.ReadSingle();
            SUx28 = bin.ReadSingle();
            SUx2C = bin.ReadSingle();
            SUx30 = bin.ReadSingle();
            SUx34 = bin.ReadSingle();
            SUx38 = bin.ReadSingle();
            SUx3C = bin.ReadSingle();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(SUx00);
            bin.Write(SUx04);
            bin.Write(SUx08);
            bin.Write(SUx0C);
            bin.Write(SUx10);
            bin.Write(SUx14);
            bin.Write(SUx18);
            bin.Write(SUx1C);
            bin.Write(SUx20);
            bin.Write(SUx24);
            bin.Write(SUx28);
            bin.Write(SUx2C);
            bin.Write(SUx30);
            bin.Write(SUx34);
            bin.Write(SUx38);
            bin.Write(SUx3C);
        }
    }
}
