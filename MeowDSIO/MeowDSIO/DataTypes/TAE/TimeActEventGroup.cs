using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE
{
    public class TimeActEventGroup
    {
        public List<TimeActEventBase> Events = new List<TimeActEventBase>();
        public TimeActEventType GeneralType;
    }
}
