using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventBlackEyeOrbInvasion : MsbEventBase
    {
        public int NPCHostEventEntityID { get; set; } = 0;
        public int InvasionEventEntityID { get; set; } = 0;
        public int InvasionRegionIndex { get; set; } = 0;
        public int SUx0C { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.BlackEyeOrbInvasions;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            NPCHostEventEntityID = bin.ReadInt32();
            InvasionEventEntityID = bin.ReadInt32();
            InvasionRegionIndex = bin.ReadInt32();
            SUx0C = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(NPCHostEventEntityID);
            bin.Write(InvasionEventEntityID);
            bin.Write(InvasionRegionIndex);
            bin.Write(SUx0C);
        }
    }
}
