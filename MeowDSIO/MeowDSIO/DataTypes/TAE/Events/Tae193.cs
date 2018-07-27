using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae193 : TimeActEventBase
    {
        public Tae193(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public Tae193(float StartTime, float EndTime, float Opacity, float FadeDuration)
            : this(StartTime, EndTime)
        {
            this.Opacity = Opacity;
            this.FadeDuration = FadeDuration;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                Opacity,
                FadeDuration,
            };
            set
            {
                Opacity = (float)value[0];
                FadeDuration = (float)value[1];
            }
        }

        public float Opacity { get; set; } = 0;
        public float FadeDuration { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            Opacity = bin.ReadSingle();
            FadeDuration = bin.ReadSingle();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(Opacity);
            bin.Write(FadeDuration);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.Type193;
        }
    }
}
