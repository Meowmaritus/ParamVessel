using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Remo_Tae208 : TimeActEventBase
    {
        public Remo_Tae208(float StartTime, float EndTime)
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
                UNK2A,
                UNK2B,
                UNK3A,
                UNK3B,
                UNK4,
                UNK5,
                UNK6,
                UNK7,
                UNK8,
            };
        }

        public short UNK1A { get; set; } = 0;
        public short UNK1B { get; set; } = 0;
        public short UNK2A { get; set; } = 0;
        public short UNK2B { get; set; } = 0;
        public short UNK3A { get; set; } = 0;
        public short UNK3B { get; set; } = 0;
        public int UNK4 { get; set; } = 0;
        public int UNK5 { get; set; } = 0;
        public int UNK6 { get; set; } = 0;
        public int UNK7 { get; set; } = 0;
        public int UNK8 { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            UNK1A = bin.ReadInt16();
            UNK1B = bin.ReadInt16();
            UNK2A = bin.ReadInt16();
            UNK2B = bin.ReadInt16();
            UNK3A = bin.ReadInt16();
            UNK3B = bin.ReadInt16();
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
            bin.Write(UNK2A);
            bin.Write(UNK2B);
            bin.Write(UNK3A);
            bin.Write(UNK3B);
            bin.Write(UNK4);
            bin.Write(UNK5);
            bin.Write(UNK6);
            bin.Write(UNK7);
            bin.Write(UNK8);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.Remo_Type208;
        }
    }
}
