using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventObjAct : MsbEventBase
    {
        public int ObjActEntityID { get; set; } = 0;

        internal int PartIndex2 { get; set; } = 0;
        public string PartName2 { get; set; } = "";

        public short ParameterID { get; set; } = 0;
        public short SUx0A { get; set; } = 0;
        public int EventFlagID { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.ObjActs;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            ObjActEntityID = bin.ReadInt32();
            PartIndex2 = bin.ReadInt32();
            ParameterID = bin.ReadInt16();
            SUx0A = bin.ReadInt16();
            EventFlagID = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(ObjActEntityID);
            bin.Write(PartIndex2);
            bin.Write(ParameterID);
            bin.Write(SUx0A);
            bin.Write(EventFlagID);
        }
    }
}
