using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE
{
    public abstract class TimeActEventBase
    {
        public int Index { get; set; } = -1;

        public abstract IList<object> Parameters { get; set; }

        protected abstract TimeActEventType GetEventType();

        public TimeActEventType EventType => GetEventType();

        public float StartTime { get; set; } = 0;
        public float EndTime { get; set; } = 0;

        public abstract void ReadParameters(DSBinaryReader bin);
        public abstract void WriteParameters(DSBinaryWriter bin);
    }
}
