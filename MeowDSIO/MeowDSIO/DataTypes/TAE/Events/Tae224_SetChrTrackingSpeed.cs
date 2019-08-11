using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae224_SetChrTrackingSpeed : TimeActEventBase
    {
        public Tae224_SetChrTrackingSpeed(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                TrackingSpeed,
            };
        }

        public float TrackingSpeed { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            TrackingSpeed = bin.ReadSingle();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(TrackingSpeed);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.SetChrTrackingSpeed;
        }
    }
}
