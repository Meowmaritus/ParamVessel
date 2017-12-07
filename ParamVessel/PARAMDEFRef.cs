using MeowDSIO.DataFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowsBetterParamEditor
{
    public class PARAMDEFRef
    {
        public string Key { get; set; } = null;
        public PARAMDEF Value { get; set; } = null;

        public PARAMDEFRef() { }

        public PARAMDEFRef(string Key, PARAMDEF Value)
        {
            this.Key = Key;
            this.Value = Value;
        }
    }
}
