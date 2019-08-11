using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae300_SetValue : TimeActEventBase
    {
        public Tae300_SetValue(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                ValueType,
                ValueSubType,
                ParamFloat1,
                ParamFloat2,
                SomeID,
            };
        }

        public short ValueType { get; set; } = 0;
        public short ValueSubType { get; set; } = 1;
        public float ParamFloat1 { get; set; } = 0;
        public float ParamFloat2 { get; set; } = 1;
        public int SomeID { get; set; } = -1;

        public override void ReadParameters(DSBinaryReader bin)
        {
            ValueType = bin.ReadInt16();
            ValueSubType = bin.ReadInt16();
            ParamFloat1 = bin.ReadSingle();
            ParamFloat2 = bin.ReadSingle();
            SomeID = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(ValueType);
            bin.Write(ValueSubType);
            bin.Write(ParamFloat1);
            bin.Write(ParamFloat2);
            bin.Write(SomeID);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.SetValue;
        }
    }
}
