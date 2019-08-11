using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae008 : TimeActEventBase
    {
        public Tae008(float StartTime, float EndTime)
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
                UNK3,
                UNK4,
                UNK5,
                UNK6,
                UNK7,
                UNK8,
                UNK9,
                UNK10,
                UNK11,
                UNK12,
            };
        }

        public float UNK1 { get; set; } = 0;
        public float UNK2 { get; set; } = 0;
        public float UNK3 { get; set; } = 0;
        public float UNK4 { get; set; } = 0;
        public float UNK5 { get; set; } = 0;
        public float UNK6 { get; set; } = 0;
        public float UNK7 { get; set; } = 0;
        public float UNK8 { get; set; } = 0;
        public float UNK9 { get; set; } = 0;
        public float UNK10 { get; set; } = 0;
        public float UNK11 { get; set; } = 0;
        public float UNK12 { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            UNK1 = bin.ReadSingle();
            UNK2 = bin.ReadSingle();
            UNK3 = bin.ReadSingle();
            UNK4 = bin.ReadSingle();
            UNK5 = bin.ReadSingle();
            UNK6 = bin.ReadSingle();
            UNK7 = bin.ReadSingle();
            UNK8 = bin.ReadSingle();
            UNK9 = bin.ReadSingle();
            UNK10 = bin.ReadSingle();
            UNK11 = bin.ReadSingle();
            UNK12 = bin.ReadSingle();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(UNK1);
            bin.Write(UNK2);
            bin.Write(UNK3);
            bin.Write(UNK4);
            bin.Write(UNK5);
            bin.Write(UNK6);
            bin.Write(UNK7);
            bin.Write(UNK8);
            bin.Write(UNK9);
            bin.Write(UNK10);
            bin.Write(UNK11);
            bin.Write(UNK12);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.Type008;
        }
    }
}
