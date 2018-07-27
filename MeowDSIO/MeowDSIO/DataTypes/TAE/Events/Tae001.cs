using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae001 : TimeActEventBase
    {
        public Tae001(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public Tae001(float StartTime, float EndTime, int HitType, int UNK2, int DmgLevel)
            : this(StartTime, EndTime)
        {
            this.HitType = HitType;
            this.UNK2 = UNK2;
            this.DmgLevel = DmgLevel;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                HitType,
                UNK2,
                DmgLevel,
            };
            set
            {
                HitType = (int)value[0];
                UNK2 = (int)value[1];
                DmgLevel = (int)value[2];
            }
        }

        public int HitType { get; set; } = 0;
        public int UNK2 { get; set; } = 0;
        public int DmgLevel { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            HitType = bin.ReadInt32();
            UNK2 = bin.ReadInt32();
            DmgLevel = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(HitType);
            bin.Write(UNK2);
            bin.Write(DmgLevel);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.Type001;
        }
    }
}
