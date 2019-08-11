using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae016_SetEventEditorColors : TimeActEventBase
    {
        public Tae016_SetEventEditorColors(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                EventKind,
                C1R, C1G, C1B, C1F,
                C2R, C2G, C2B, C2F,
                C3R, C3G, C3B, C3F,
            };
        }

        public TimeActEventType EventKind { get; set; } = 0;

        public byte C1R { get; set; } = 0;
        public byte C1G { get; set; } = 0;
        public byte C1B { get; set; } = 0;
        public byte C1F { get; set; } = 0;

        public byte C2R { get; set; } = 0;
        public byte C2G { get; set; } = 0;
        public byte C2B { get; set; } = 0;
        public byte C2F { get; set; } = 0;

        public byte C3R { get; set; } = 0;
        public byte C3G { get; set; } = 0;
        public byte C3B { get; set; } = 0;
        public byte C3F { get; set; } = 0;

        public Color Color1 => new Color(C1R, C1G, C1B, (byte)255);
        public Color Color2 => new Color(C2R, C2G, C2B, (byte)255);
        public Color Color3 => new Color(C3R, C3G, C3B, (byte)255);

        public override void ReadParameters(DSBinaryReader bin)
        {
            EventKind = (TimeActEventType)bin.ReadInt32();

            C1R = bin.ReadByte();
            C1G = bin.ReadByte();
            C1B = bin.ReadByte();
            C1F = bin.ReadByte();

            C2R = bin.ReadByte();
            C2G = bin.ReadByte();
            C2B = bin.ReadByte();
            C2F = bin.ReadByte();

            C3R = bin.ReadByte();
            C3G = bin.ReadByte();
            C3B = bin.ReadByte();
            C3F = bin.ReadByte();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write((int)EventKind);

            bin.Write(C1R);
            bin.Write(C1G);
            bin.Write(C1B);
            bin.Write(C1F);

            bin.Write(C2R);
            bin.Write(C2G);
            bin.Write(C2B);
            bin.Write(C2F);

            bin.Write(C3R);
            bin.Write(C3G);
            bin.Write(C3B);
            bin.Write(C3F);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.SetEventEditorColors;
        }
    }
}
