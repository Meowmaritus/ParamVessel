using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae002_DoBehaviorBullet : TimeActEventBase
    {
        public Tae002_DoBehaviorBullet(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                Dmy,
                BulletIndex,
                BehaviorJudgeID,
                UNK4,
            };
        }

        public int Dmy { get; set; } = 0;
        public int BulletIndex { get; set; } = 0;
        public int BehaviorJudgeID { get; set; } = 0;
        public int UNK4 { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            Dmy = bin.ReadInt32();
            BulletIndex = bin.ReadInt32();
            BehaviorJudgeID = bin.ReadInt32();
            UNK4 = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(Dmy);
            bin.Write(BulletIndex);
            bin.Write(BehaviorJudgeID);
            bin.Write(UNK4);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.DoBehaviorBullet;
        }
    }
}
