using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae000_DoCommand : TimeActEventBase
    {
        public Tae000_DoCommand(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                CommandType,
                ParamFloat,
                SomeID,
            };
        }

        public TaeGeneralCommandType CommandType { get; set; } = 0;
        public float ParamFloat { get; set; } = 0;
        public int SomeID { get; set; } = -1;

        public override void ReadParameters(DSBinaryReader bin)
        {
            CommandType = (TaeGeneralCommandType)bin.ReadInt32();
            ParamFloat = bin.ReadSingle();
            SomeID = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write((int)CommandType);
            bin.Write(ParamFloat);
            bin.Write(SomeID);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.DoCommand;
        }
    }
}
