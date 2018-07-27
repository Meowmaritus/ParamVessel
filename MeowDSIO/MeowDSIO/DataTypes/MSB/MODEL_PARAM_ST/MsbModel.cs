using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.MODEL_PARAM_ST
{
    public class MsbModel : MsbStruct
    {
        public string Name { get; set; } = null;
        public ModelParamSubtype ModelType { get; set; } = ModelParamSubtype.MapPiece;
        public string PlaceholderModel { get; set; } = null;
        public int Index { get; set; } = 0;
        public int InstanceCount { get; set; } = 0;
        public int Ux14 { get; set; } = 0;
        public int Ux18 { get; set; } = 0;
        public int Ux1C { get; set; } = 0;

        protected override void InternalRead(DSBinaryReader bin)
        {
            Name = bin.ReadMsbString();
            ModelType = (ModelParamSubtype)bin.ReadInt32();
            Index = bin.ReadInt32();
            PlaceholderModel = bin.ReadMsbString();
            InstanceCount = bin.ReadInt32();
            Ux14 = bin.ReadInt32();
            Ux18 = bin.ReadInt32();
            Ux1C = bin.ReadInt32();
        }

        protected override void InternalWrite(DSBinaryWriter bin)
        {
            bin.Placeholder($"MODEL_PARAM_ST|0|{nameof(Name)}");
            bin.Write((int)ModelType);
            bin.Write(Index);
            bin.Placeholder($"MODEL_PARAM_ST|0|{nameof(PlaceholderModel)}");
            bin.Write(InstanceCount);
            bin.Write(Ux14);
            bin.Write(Ux18);
            bin.Write(Ux1C);

            bin.Replace($"MODEL_PARAM_ST|0|{nameof(Name)}", bin.MsbOffset);
            bin.WriteMsbString(Name, terminate: true);

            bin.Replace($"MODEL_PARAM_ST|0|{nameof(PlaceholderModel)}", bin.MsbOffset);
            bin.WriteMsbString(PlaceholderModel, terminate: true);

            bin.Pad(align: 0x04);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
