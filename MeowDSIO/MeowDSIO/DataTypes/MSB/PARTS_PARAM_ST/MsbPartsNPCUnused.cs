using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST
{
    public class MsbPartsNPCUnused : MsbPartsNPC
    {
        internal override PartsParamSubtype GetSubtypeValue()
        {
            return PartsParamSubtype.UnusedNPCs;
        }
    }
}
