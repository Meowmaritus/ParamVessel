using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae144_RumbleCamOnDmy : TimeActEventBase
    {
        public Tae144_RumbleCamOnDmy(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                RumbleCamID,
                Dmy,
                FalloffStart,
                FalloffEnd,
            };
        }

        public short RumbleCamID { get; set; } = 0;
        public short Dmy { get; set; } = 0;
        public float FalloffStart { get; set; } = 0;
        public float FalloffEnd { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            RumbleCamID = bin.ReadInt16();
            Dmy = bin.ReadInt16();
            FalloffStart = bin.ReadSingle();
            FalloffEnd = bin.ReadSingle();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(RumbleCamID);
            bin.Write(Dmy);
            bin.Write(FalloffStart);
            bin.Write(FalloffEnd);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.RumbleCamOnDmy;
        }
    }
}
