using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Remo_Tae161 : TimeActEventBase
    {
        public Remo_Tae161(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object> {};
        }

        // This event is actually empty.

        public override void ReadParameters(DSBinaryReader bin)
        {

        }

        public override void WriteParameters(DSBinaryWriter bin)
        {

        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.Remo_Type161;
        }
    }
}
