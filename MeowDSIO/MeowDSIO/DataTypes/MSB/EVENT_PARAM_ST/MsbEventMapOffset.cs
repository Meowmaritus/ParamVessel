using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventMapOffset : MsbEventBase
    {
        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
        public float Z { get; set; } = 0;
        public float SUx0C { get; set; } = 0; //TODO: Confirm Float/Int

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.MapOffset;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            X = bin.ReadSingle();
            Y = bin.ReadSingle();
            Z = bin.ReadSingle();
            SUx0C = bin.ReadSingle();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(X);
            bin.Write(Y);
            bin.Write(Z);
            bin.Write(SUx0C);
        }
    }
}
