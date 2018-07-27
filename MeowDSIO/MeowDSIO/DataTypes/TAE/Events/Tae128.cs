using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae128 : TimeActEventBase
    {
        public Tae128(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public Tae128(float StartTime, float EndTime, MSB.MsbSoundType SoundType, int SoundID)
            : this(StartTime, EndTime)
        {
            this.SoundType = SoundType;
            this.SoundID = SoundID;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                SoundType,
                SoundID,
            };
            set
            {
                SoundType = (MSB.MsbSoundType)value[0];
                SoundID = (int)value[1];
            }
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
            return TimeActEventType.Type128;
        }
    }
}
