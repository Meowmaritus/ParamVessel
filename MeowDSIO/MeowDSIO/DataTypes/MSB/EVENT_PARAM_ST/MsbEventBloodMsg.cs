using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventBloodMsg : MsbEventBase
    {
        public short MsgID { get; set; } = 0;
        public short SUx02 { get; set; } = 0;
        public short SUx04 { get; set; } = 0;
        public short SUx06 { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.BloodMsg;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            MsgID = bin.ReadInt16();
            SUx02 = bin.ReadInt16();
            SUx04 = bin.ReadInt16();
            SUx06 = bin.ReadInt16();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(MsgID);
            bin.Write(SUx02);
            bin.Write(SUx04);
            bin.Write(SUx06);
        }
    }
}
