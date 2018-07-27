using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST
{
    public class MsbPartsCollision : MsbPartsBase
    {
        public byte UNK1 { get; set; } = 0;
        public byte UNK2 { get; set; } = 0;
        public byte UNK3 { get; set; } = 0;
        public byte UNK4 { get; set; } = 0;

        public float UNK5 { get; set; } = 0;

        public int UNK6 { get; set; } = 0;
        public int UNK7 { get; set; } = 0;

        public int VagrantEntityID1 { get; set; } = 0;
        public int VagrantEntityID2 { get; set; } = 0;
        public int VagrantEntityID3 { get; set; } = 0;

        public short UNK8 { get; set; } = 0;
        public short UNK9 { get; set; } = 0;

        public int BonfireObjectEntityID { get; set; } = 0;

        public int UNK10 { get; set; } = 0;
        public int UNK11 { get; set; } = 0;
        public int UNK12 { get; set; } = 0;

        public int MultiplayerID { get; set; } = 0;

        public short UNK13 { get; set; } = 0;
        public short UNK14 { get; set; } = 0;

        public int UNK15 { get; set; } = 0;
        public int UNK16 { get; set; } = 0;
        public int UNK17 { get; set; } = 0;
        public int UNK18 { get; set; } = 0;

        public int UNK19 { get; set; } = 0;
        public int UNK20 { get; set; } = 0;

        internal override PartsParamSubtype GetSubtypeValue()
        {
            return PartsParamSubtype.Collisions;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            UNK1 = bin.ReadByte();
            UNK2 = bin.ReadByte();
            UNK3 = bin.ReadByte();
            UNK4 = bin.ReadByte();

            UNK5 = bin.ReadSingle();

            UNK6 = bin.ReadInt32();
            UNK7 = bin.ReadInt32();

            VagrantEntityID1 = bin.ReadInt32();
            VagrantEntityID2 = bin.ReadInt32();
            VagrantEntityID3 = bin.ReadInt32();

            UNK8 = bin.ReadInt16();
            UNK9 = bin.ReadInt16();

            BonfireObjectEntityID = bin.ReadInt32();

            UNK10 = bin.ReadInt32();
            UNK11 = bin.ReadInt32();
            UNK12 = bin.ReadInt32();

            MultiplayerID = bin.ReadInt32();

            UNK13 = bin.ReadInt16();
            UNK14 = bin.ReadInt16();

            UNK15 = bin.ReadInt32();
            UNK16 = bin.ReadInt32();
            UNK17 = bin.ReadInt32();
            UNK18 = bin.ReadInt32();

            UNK19 = bin.ReadInt32();
            UNK20 = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(UNK1);
            bin.Write(UNK2);
            bin.Write(UNK3);
            bin.Write(UNK4);

            bin.Write(UNK5);

            bin.Write(UNK6);
            bin.Write(UNK7);

            bin.Write(VagrantEntityID1);
            bin.Write(VagrantEntityID2);
            bin.Write(VagrantEntityID3);

            bin.Write(UNK8);
            bin.Write(UNK9);

            bin.Write(BonfireObjectEntityID);

            bin.Write(UNK10);
            bin.Write(UNK11);
            bin.Write(UNK12);

            bin.Write(MultiplayerID);

            bin.Write(UNK13);
            bin.Write(UNK14);

            bin.Write(UNK15);
            bin.Write(UNK16);
            bin.Write(UNK17);
            bin.Write(UNK18);

            bin.Write(UNK19);
            bin.Write(UNK20);
        }
    }
}
