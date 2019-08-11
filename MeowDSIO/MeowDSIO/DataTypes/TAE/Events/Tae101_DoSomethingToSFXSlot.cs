using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae101_DoSomethingToSFXSlot : TimeActEventBase
    {
        public Tae101_DoSomethingToSFXSlot(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                Slot,
            };
        }

        public int Slot { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            Slot = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(Slot);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.DoSomethingToSFXSlot;
        }
    }
}
