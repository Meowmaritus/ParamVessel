using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventSpawnPoint : MsbEventBase
    {
        public int RegionIndex2 { get; set; } = 0;
        public int SUx04 { get; set; } = 0;
        public int SUx08 { get; set; } = 0;
        public int SUx0C { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.SpawnPoints;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            RegionIndex2 = bin.ReadInt32();
            SUx04 = bin.ReadInt32();
            SUx08 = bin.ReadInt32();
            SUx0C = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(RegionIndex2);
            bin.Write(SUx04);
            bin.Write(SUx08);
            bin.Write(SUx0C);
        }
    }
}
