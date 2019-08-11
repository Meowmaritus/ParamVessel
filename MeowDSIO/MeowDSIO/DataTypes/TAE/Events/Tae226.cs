﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE.Events
{
    public class Tae226 : TimeActEventBase
    {
        public Tae226(float StartTime, float EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }

        public override IList<object> Parameters
        {
            get => new List<object>
            {
                UNK1,
            };
        }

        public int UNK1 { get; set; } = 0;

        public override void ReadParameters(DSBinaryReader bin)
        {
            UNK1 = bin.ReadInt32();
        }

        public override void WriteParameters(DSBinaryWriter bin)
        {
            bin.Write(UNK1);
        }

        protected override TimeActEventType GetEventType()
        {
            return TimeActEventType.Type226;
        }
    }
}
