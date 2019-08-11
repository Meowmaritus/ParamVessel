using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Remo_Tae255 : TimeActEventBase
    {
        public Remo_Tae255(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                SFX,
                UNK2,
                UNK3,
                UNK4,
            };
        }

        public int SFX { get; set; } = 0;
        public int UNK2 { get; set; } = 0;
        public int UNK3 { get; set; } = 0;
        public int UNK4 { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            SFX = bin.ReadInt32();
            UNK2 = bin.ReadInt32();
            UNK3 = bin.ReadInt32();
            UNK4 = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(SFX);
            bin.Write(UNK2);
            bin.Write(UNK3);
            bin.Write(UNK4);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.Remo_Type255;
        }
    }
}
