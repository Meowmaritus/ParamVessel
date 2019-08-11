using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae005_DoBehaviorCommon : TimeActEventBase
    {
        public Tae005_DoBehaviorCommon(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                UNK1,
                BehaviorID,
            };
        }

        public int UNK1 { get; set; } = 0;
        public int BehaviorID { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            UNK1 = bin.ReadInt32();
            BehaviorID = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(UNK1);
            bin.Write(BehaviorID);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.DoBehaviorCommon;
        }
    }
}
