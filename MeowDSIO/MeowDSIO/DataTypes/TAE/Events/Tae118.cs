using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae118 : TimeActEventBase
    {
        public Tae118(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public Tae118(float StartTime, float EndTime, int UNK1, short UNK2, short UNK3, short UNK4, short UNK5)
            : this(StartTime, EndTime)
        {
            this.UNK1 = UNK1;
            this.UNK2 = UNK2;
            this.UNK3 = UNK3;
            this.UNK4 = UNK4;
            this.UNK5 = UNK5;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                UNK1,
                UNK2,
                UNK3,
                UNK4,
                UNK5,
            };
            set
            {
                UNK1 = (int)value[0];
                UNK2 = (short)value[1];
                UNK3 = (short)value[2];
                UNK4 = (short)value[3];
                UNK5 = (short)value[4];
            }
        }

        public int UNK1 { get; set; } = 0;
        public short UNK2 { get; set; } = 0;
        public short UNK3 { get; set; } = 0;
        public short UNK4 { get; set; } = 0;
        public short UNK5 { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            UNK1 = bin.ReadInt32();
            UNK2 = bin.ReadInt16();
            UNK3 = bin.ReadInt16();
            UNK4 = bin.ReadInt16();
            UNK5 = bin.ReadInt16();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(UNK1);
            bin.Write(UNK2);
            bin.Write(UNK3);
            bin.Write(UNK4);
            bin.Write(UNK5);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.Type118;
        }
    }
}
