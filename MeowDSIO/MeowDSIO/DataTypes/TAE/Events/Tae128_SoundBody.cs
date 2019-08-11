using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae128_SoundBody : TimeActEventBase
    {
        public Tae128_SoundBody(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                SoundType,
                SoundID,
            };
        }

        public MSB.MsbSoundType SoundType { get; set; } = 0;
        public int SoundID { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            SoundType = (MSB.MsbSoundType)bin.ReadInt32();
            SoundID = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write((int)SoundType);
            bin.Write(SoundID);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.SoundBody;
        }
    }
}
