﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae306 : TimeActEventBase
    {
        public Tae306(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public Tae306(float StartTime, float EndTime, int UNK1, short UNK2, short UNK3, int UNK4)
            : this(StartTime, EndTime)
        {
            this.UNK1 = UNK1;
            this.UNK2 = UNK2;
            this.UNK3 = UNK3;
            this.UNK4 = UNK4;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                UNK1,
                UNK2,
                UNK3,
                UNK4,
            };
            set
            {
                UNK1 = (int)value[0];
                UNK2 = (short)value[1];
                UNK3 = (short)value[2];
                UNK4 = (int)value[3];
            }
        }

        public int UNK1 { get; set; } = 0;
        public short UNK2 { get; set; } = 0;
        public short UNK3 { get; set; } = 0;
        public int UNK4 { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            UNK1 = bin.ReadInt32();
            UNK2 = bin.ReadInt16();
            UNK3 = bin.ReadInt16();
            UNK4 = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(UNK1);
            bin.Write(UNK2);
            bin.Write(UNK3);
            bin.Write(UNK4);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.Type306;
        }
    }
}
