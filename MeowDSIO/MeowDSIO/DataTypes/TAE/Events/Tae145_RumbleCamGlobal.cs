using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae145_RumbleCamGlobal : TimeActEventBase
    {
        public Tae145_RumbleCamGlobal(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                RumbleCamID,
            };
        }

        public int RumbleCamID { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            RumbleCamID = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(RumbleCamID);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.RumbleCamGlobal;
        }
    }
}
