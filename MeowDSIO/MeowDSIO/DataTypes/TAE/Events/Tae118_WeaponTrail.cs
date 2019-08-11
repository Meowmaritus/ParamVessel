using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae118_WeaponTrail : TimeActEventBase
    {
        public Tae118_WeaponTrail(float StartTime, float EndTime)
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
                StartDmy,
                EndDmy,
                SpawnSlot,
            };
        }

        public int SFX { get; set; } = 0;
        public short UNK2 { get; set; } = 0;
        public short StartDmy { get; set; } = 0;
        public short EndDmy { get; set; } = 0;
        public short SpawnSlot { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            SFX = bin.ReadInt32();
            UNK2 = bin.ReadInt16();
            StartDmy = bin.ReadInt16();
            EndDmy = bin.ReadInt16();
            SpawnSlot = bin.ReadInt16();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(SFX);
            bin.Write(UNK2);
            bin.Write(StartDmy);
            bin.Write(EndDmy);
            bin.Write(SpawnSlot);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.WeaponTrail;
        }
    }
}
