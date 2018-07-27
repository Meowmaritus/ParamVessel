using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventEnvironment : MsbEventBase
    {
        public int SUx00 { get; set; } = 0;
        public float SUx04 { get; set; } = 0;
        public float SUx08 { get; set; } = 0;
        public float SUx0C { get; set; } = 0;
        public float SUx10 { get; set; } = 0;
        public float SUx14 { get; set; } = 0;
        public float SUx18 { get; set; } = 0;
        public float SUx1C { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.Environment;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            SUx00 = bin.ReadInt32();
            SUx04 = bin.ReadSingle();
            SUx08 = bin.ReadSingle();
            SUx0C = bin.ReadSingle();
            SUx10 = bin.ReadSingle();
            SUx14 = bin.ReadSingle();
            SUx18 = bin.ReadSingle();
            SUx1C = bin.ReadSingle();
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
        }
    }
}
