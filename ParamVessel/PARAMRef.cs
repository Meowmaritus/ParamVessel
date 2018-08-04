using MeowDSIO.DataFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowsBetterParamEditor
{
    public class PARAMRef
    {
        //private static Dictionary<PARAMRef, string> FancyDisplayNameCache = new Dictionary<PARAMRef, string>();

        public string FancyDisplayName
        {
            get
            {
                return Key;
                //try
                //{
                //    return FancyDisplayNameCache[this];
                //}
                //catch (KeyNotFoundException)
                //{
                //    if (ParamDataContext.SpecialInternalParamNameOverrides.ContainsKey(Key))
                //        FancyDisplayNameCache.Add(this, ParamDataContext.SpecialInternalParamNameOverrides[Key]);
                //    else
                //        FancyDisplayNameCache.Add(this, Value.ID);

                //    // If it somehow throw the KeyNotFoundException here, then there's a problem lol
                //    return FancyDisplayNameCache[this];
                //}
            }
        }

        public string Key { get; set; } = null;
        public PARAM Value { get; set; } = null;
        public bool IsDrawParam { get; set; } = false;
        public string BNDName { get; set; } = null;

        public PARAMRef() { }

        public PARAMRef(string Key, PARAM Value, bool IsDrawParam, string BNDName)
        {
            this.Key = Key;
            this.Value = Value;
            this.IsDrawParam = IsDrawParam;
            this.BNDName = BNDName;
        }

        public override string ToString()
        {
            return $"{BNDName} - {Key} - {Value.ID}";
        }
    }
}
