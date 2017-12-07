using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.FMG
{
    public class FMGEntryRef
    {
        public int ID { get; set; } = 0;
        public string Value { get; set; } = null;

        public FMGEntryRef(int ID, string Value)
        {
            this.ID = ID;
            this.Value = Value;
        }
    }
}
