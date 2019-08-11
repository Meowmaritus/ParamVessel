using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae232 : TimeActEventBase
    {
        public Tae232(float StartTime, float EndTime)
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
            };
        }

        public byte UNK1 { get; set; } = 0;
        public byte UNK2 { get; set; } = 0;
        public byte UNK3 { get; set; } = 0;
        public byte UNK4 { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            UNK1 = bin.ReadByte();
            UNK2 = bin.ReadByte();
            UNK3 = bin.ReadByte();
            UNK4 = bin.ReadByte();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(UNK1);
            bin.Write(UNK2);
            bin.Write(UNK3);
            bin.Write(UNK4);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.Type232;
        }
    }
}
