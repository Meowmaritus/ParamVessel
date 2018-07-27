using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventSound : MsbEventBase
    {
        public MsbSoundType SoundType { get; set; } = MsbSoundType.Environment;
        public int SoundID { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.Sounds;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            SoundType = (MsbSoundType)bin.ReadInt32();
            SoundID = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write((int)SoundType);
            bin.Write(SoundID);
        }
    }
}
