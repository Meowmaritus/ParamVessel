using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae066_ApplySpEffect : TimeActEventBase
    {
        public Tae066_ApplySpEffect(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                SpEffect,
            };
        }

        public int SpEffect { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            SpEffect = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(SpEffect);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.ApplySpEffect;
        }
    }
}
