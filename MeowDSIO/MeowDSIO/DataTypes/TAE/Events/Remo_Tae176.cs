using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Remo_Tae176 : TimeActEventBase
    {
        public Remo_Tae176(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                UNK1A,
                UNK1B,
                UNK2,
                UNK3,
                UNK4,
                UNK5,
                UNK6,
                UNK7,
                UNK8,
            };
        }

        public short UNK1A { get; set; } = 0;
        public short UNK1B { get; set; } = 0;
        public int UNK2 { get; set; } = 0;
        public int UNK3 { get; set; } = 0;
        public int UNK4 { get; set; } = 0;
        public int UNK5 { get; set; } = 0;
        public int UNK6 { get; set; } = 0;
        public int UNK7 { get; set; } = 0;
        public int UNK8 { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            UNK1A = bin.ReadInt16();
            UNK1B = bin.ReadInt16();
            UNK2 = bin.ReadInt32();
            UNK3 = bin.ReadInt32();
            UNK4 = bin.ReadInt32();
            UNK5 = bin.ReadInt32();
            UNK6 = bin.ReadInt32();
            UNK7 = bin.ReadInt32();
            UNK8 = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(UNK1A);
            bin.Write(UNK1B);
            bin.Write(UNK2);
            bin.Write(UNK3);
            bin.Write(UNK4);
            bin.Write(UNK5);
            bin.Write(UNK6);
            bin.Write(UNK7);
            bin.Write(UNK8);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.Remo_Type176;
        }
    }
}
