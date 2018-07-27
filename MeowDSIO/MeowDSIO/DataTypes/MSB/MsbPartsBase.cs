using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB
{
    public abstract class MsbPartsBase : MsbStruct
    {
        public string Name { get; set; } = "";
        public int Index { get; set; } = 0;

        internal int ModelIndex { get; set; } = 0;

        public string ModelName { get; set; }  = "";

        public string PlaceholderModel { get; set; } = "";

        public float PosX { get; set; } = 0;
        public float PosY { get; set; } = 0;
        public float PosZ { get; set; } = 0;

        public float RotX { get; set; } = 0;
        public float RotY { get; set; } = 0;
        public float RotZ { get; set; } = 0;

        public float ScaleX { get; set; } = 0;
        public float ScaleY { get; set; } = 0;
        public float ScaleZ { get; set; } = 0;

        public int DrawGroup1 { get; set; } = 0;
        public int DrawGroup2 { get; set; } = 0;
        public int DrawGroup3 { get; set; } = 0;
        public int DrawGroup4 { get; set; } = 0;

        public int DispGroup1 { get; set; } = 0;
        public int DispGroup2 { get; set; } = 0;
        public int DispGroup3 { get; set; } = 0;
        public int DispGroup4 { get; set; } = 0;

        public int Ux60 { get; set; } = 0;

        //BASE DATA

        public int EventEntityID { get; set; } = -1;
        public byte LightID { get; set; } = 0;
        public byte FogID { get; set; } = 0;
        public byte ScatterID { get; set; } = 0;

        public byte BUx07 { get; set; } = 0;
        public byte BUx08 { get; set; } = 0;
        public byte BUx09 { get; set; } = 0;
        public byte BUx0A { get; set; } = 0;
        public byte BUx0B { get; set; } = 0;

        public short BUx0C { get; set; } = 0;
        public short BUx0E { get; set; } = 0;
        public short BUx10 { get; set; } = 0;
        public short BUx12 { get; set; } = 0;

        public int BUx14 { get; set; } = 0;
        //public int BUx18 { get; set; } = 0;

        protected abstract void SubtypeRead(DSBinaryReader bin);
        protected abstract void SubtypeWrite(DSBinaryWriter bin);
        internal abstract PartsParamSubtype GetSubtypeValue();
        internal PartsParamSubtype Type => GetSubtypeValue();

        protected override void InternalRead(DSBinaryReader bin)
        {
            Name = bin.ReadMsbString();

            bin.AssertInt32((int)Type);

            Index = bin.ReadInt32();
            ModelIndex = bin.ReadInt32();

            PlaceholderModel = bin.ReadMsbString();

            PosX = bin.ReadSingle();
            PosY = bin.ReadSingle();
            PosZ = bin.ReadSingle();

            RotX = bin.ReadSingle();
            RotY = bin.ReadSingle();
            RotZ = bin.ReadSingle();

            ScaleX = bin.ReadSingle();
            ScaleY = bin.ReadSingle();
            ScaleZ = bin.ReadSingle();

            DrawGroup1 = bin.ReadInt32();
            DrawGroup2 = bin.ReadInt32();
            DrawGroup3 = bin.ReadInt32();
            DrawGroup4 = bin.ReadInt32();

            DispGroup1 = bin.ReadInt32();
            DispGroup2 = bin.ReadInt32();
            DispGroup3 = bin.ReadInt32();
            DispGroup4 = bin.ReadInt32();

            int baseDataOffset = bin.ReadInt32();
            int subtypeDataOffset = bin.ReadInt32();

            Ux60 = bin.ReadInt32();

            bin.StepInMSB(baseDataOffset);
            {
                EventEntityID = bin.ReadInt32();

                LightID = bin.ReadByte();
                FogID = bin.ReadByte();
                ScatterID = bin.ReadByte();

                BUx07 = bin.ReadByte();
                BUx08 = bin.ReadByte();
                BUx09 = bin.ReadByte();
                BUx0A = bin.ReadByte();
                BUx0B = bin.ReadByte();

                BUx0C = bin.ReadInt16();
                BUx0E = bin.ReadInt16();
                BUx10 = bin.ReadInt16();
                BUx12 = bin.ReadInt16();

                BUx14 = bin.ReadInt32();
                //BUx18 = bin.ReadInt32();

            }
            bin.StepOut();

            bin.StepInMSB(subtypeDataOffset);
            {
                SubtypeRead(bin);
            }
            bin.StepOut();
        }

        protected override void InternalWrite(DSBinaryWriter bin)
        {
            bin.Placeholder($"PARTS_PARAM_ST|{Type}|{Index}|{nameof(Name)}");

            bin.Write((int)Type);

            bin.Write(Index);
            bin.Write(ModelIndex);

            bin.Placeholder($"PARTS_PARAM_ST|{Type}|{Index}|{nameof(PlaceholderModel)}");

            bin.Write(PosX);
            bin.Write(PosY);
            bin.Write(PosZ);

            bin.Write(RotX);
            bin.Write(RotY);
            bin.Write(RotZ);

            bin.Write(ScaleX);
            bin.Write(ScaleY);
            bin.Write(ScaleZ);

            bin.Write(DrawGroup1);
            bin.Write(DrawGroup2);
            bin.Write(DrawGroup3);
            bin.Write(DrawGroup4);

            bin.Write(DispGroup1);
            bin.Write(DispGroup2);
            bin.Write(DispGroup3);
            bin.Write(DispGroup4);

            bin.Placeholder($"PARTS_PARAM_ST|{Type}|{Index}|(BASE DATA OFFSET)");
            bin.Placeholder($"PARTS_PARAM_ST|{Type}|{Index}|(SUBTYPE DATA OFFSET)");

            bin.Write(Ux60);

            int nameByteCount = DSBinaryWriter.ShiftJISEncoding.GetByteCount(Name);
            int placeholderModelByteCount = DSBinaryWriter.ShiftJISEncoding.GetByteCount(PlaceholderModel);

            int blockSize = (nameByteCount + 1) + (placeholderModelByteCount + 1);

            if (string.IsNullOrEmpty(PlaceholderModel))
            {
                blockSize += 5;
            }

            blockSize = (blockSize + 3) & (-0x4);

            bin.StartMSBStrings();
            {
                bin.Replace($"PARTS_PARAM_ST|{Type}|{Index}|{nameof(Name)}", bin.MsbOffset);
                bin.WriteMsbString(Name, terminate: true);

                bin.Replace($"PARTS_PARAM_ST|{Type}|{Index}|{nameof(PlaceholderModel)}", bin.MsbOffset);
                bin.WriteMsbString(PlaceholderModel, terminate: true);
            }
            bin.EndMSBStrings(blockSize);

            bin.Replace($"PARTS_PARAM_ST|{Type}|{Index}|(BASE DATA OFFSET)", bin.MsbOffset);
            bin.Write(EventEntityID);
            bin.Write(LightID);
            bin.Write(FogID);
            bin.Write(ScatterID);

            bin.Write(BUx07);
            bin.Write(BUx08);
            bin.Write(BUx09);
            bin.Write(BUx0A);
            bin.Write(BUx0B);

            bin.Write(BUx0C);
            bin.Write(BUx0E);
            bin.Write(BUx10);
            bin.Write(BUx12);

            bin.Write(BUx14);
            //bin.Write(BUx18);


            bin.Replace($"PARTS_PARAM_ST|{Type}|{Index}|(SUBTYPE DATA OFFSET)", bin.MsbOffset);
            SubtypeWrite(bin);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
