using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae130_SoundOther : TimeActEventBase
    {
        public Tae130_SoundOther(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                SoundType,
                SoundID,
                Dmy,
                SpawnSlot,
            };
        }

        public MSB.MsbSoundType SoundType { get; set; } = 0;
        public int SoundID { get; set; } = 0;
        public int Dmy { get; set; } = 0;
        public int SpawnSlot { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            SoundType = (MSB.MsbSoundType)bin.ReadInt32();
            SoundID = bin.ReadInt32();
            Dmy = bin.ReadInt32();
            SpawnSlot = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write((int)SoundType);
            bin.Write(SoundID);
            bin.Write(Dmy);
            bin.Write(SpawnSlot);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.SoundOther;
        }
    }
}
