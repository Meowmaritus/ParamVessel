using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae307_DoBehaviorKnockback : TimeActEventBase
    {
        public Tae307_DoBehaviorKnockback(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                UNK1,
                UNK2,
                Dmy,
                BehaviorParamID,
            };
        }

        public short UNK1 { get; set; } = 0;
        public short UNK2 { get; set; } = 0;
        public int Dmy { get; set; } = 0;
        public int BehaviorParamID { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            UNK1 = bin.ReadInt16();
            UNK2 = bin.ReadInt16();
            Dmy = bin.ReadInt32();
            BehaviorParamID = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(UNK1);
            bin.Write(UNK2);
            bin.Write(Dmy);
            bin.Write(BehaviorParamID);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.DoBehaviorKnockback;
        }
    }
}
