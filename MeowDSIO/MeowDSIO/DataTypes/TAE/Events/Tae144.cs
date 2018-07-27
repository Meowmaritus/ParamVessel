using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae144 : TimeActEventBase
    {
        public Tae144(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public Tae144(float StartTime, float EndTime, int UNK1, int UNK2, int UNK3)
            : this(StartTime, EndTime)
        {
            this.UNK1 = UNK1;
            this.UNK2 = UNK2;
            this.UNK3 = UNK3;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                UNK1,
                UNK2,
                UNK3,
            };
            set
            {
                UNK1 = (int)value[0];
                UNK2 = (int)value[1];
                UNK3 = (int)value[2];
            }
        }

        public int UNK1 { get; set; } = 0;
        public int UNK2 { get; set; } = 0;
        public int UNK3 { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            UNK1 = bin.ReadInt32();
            UNK2 = bin.ReadInt32();
            UNK3 = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(UNK1);
            bin.Write(UNK2);
            bin.Write(UNK3);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.Type144;
        }
    }
}
