using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae104_SpawnSFXAtDmyD : TimeActEventBase
    {
        public Tae104_SpawnSFXAtDmyD(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                SFX,
                Dmy,
                SpawnSlot,
            };
        }

        public int SFX { get; set; } = 0;
        public int Dmy { get; set; } = 0;
        public int SpawnSlot { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            SFX = bin.ReadInt32();
            Dmy = bin.ReadInt32();
            SpawnSlot = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(SFX);
            bin.Write(Dmy);
            bin.Write(SpawnSlot);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.SpawnSFXAtDmyD;
        }
    }
}
