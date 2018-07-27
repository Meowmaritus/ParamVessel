using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventSFX : MsbEventBase
    {
        public int ParticleEffectID { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.SFX;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            ParticleEffectID = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(ParticleEffectID);
        }
    }
}
