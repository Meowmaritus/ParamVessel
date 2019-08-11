using MeowDSIO.DataTypes.TAE.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE
{
    public abstract class TimeActEventBase
    {
        private const bool DEBUG_PRINT_A_SHITTON_OF_PARAM_SIZES = false;

        [System.ComponentModel.Browsable(false)]
        public int Index { get; set; } = -1;

        [System.ComponentModel.Browsable(false)]
        public abstract IList<object> Parameters { get; }

        [System.ComponentModel.Browsable(false)]
        protected abstract TimeActEventType GetEventType();

        [System.ComponentModel.Browsable(false)]
        public TimeActEventType EventType => GetEventType();

        [System.ComponentModel.Browsable(false)]
        public float StartTime { get; set; } = 0;

        [System.ComponentModel.Browsable(false)]
        public float EndTime { get; set; } = 0;

        public const double FRAME = 0.0333333333333333;

        [System.ComponentModel.Browsable(false)]
        public float StartTimeFr => (float)(Math.Round(StartTime / FRAME) * FRAME);

        [System.ComponentModel.Browsable(false)]
        public float EndTimeFr => (float)(Math.Round(EndTime / FRAME) * FRAME);

        public abstract void ReadParameters(DSBinaryReader bin);
        public abstract void WriteParameters(DSBinaryWriter bin);


        public event EventHandler<int> RowChanged;
        private void RaiseRowChanged(int oldRow)
        {
            RowChanged?.Invoke(this, oldRow);
        }
        [System.ComponentModel.Browsable(false)]
        private int _row = -1;


        [System.ComponentModel.Browsable(false)]
        public int Row
        {
            get => _row;
            set
            {
                if (value != _row)
                {
                    int oldRow = _row;
                    _row = value;
                    RaiseRowChanged(oldRow);
                }
            }
        }

        public static void CopyEventParameters<T>(T a, T b)
            where T : TimeActEventBase
        {
            using (var memStream = new System.IO.MemoryStream())
            {
                using (var bw = new DSBinaryWriter("_CopyEventParameters", memStream))
                {
                    a.WriteParameters(bw);
                    bw.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                    using (var br = new DSBinaryReader("_CopyEventParameters_Read", memStream))
                    {
                        b.ReadParameters(br);
                    }
                }
            }
        }

        public static TimeActEventBase GetNewEvent(TimeActEventType eventType, float startTime, float endTime)
        {
            if (DEBUG_PRINT_A_SHITTON_OF_PARAM_SIZES)
                return null;

            switch (eventType)
            {
                case TimeActEventType.DoCommand: return new Tae000_DoCommand(startTime, endTime);
                case TimeActEventType.DoBehaviorAtk: return new Tae001_DoBehaviorAtk(startTime, endTime);
                case TimeActEventType.DoBehaviorBullet: return new Tae002_DoBehaviorBullet(startTime, endTime);
                case TimeActEventType.DoBehaviorCommon: return new Tae005_DoBehaviorCommon(startTime, endTime);
                case TimeActEventType.Type008: return new Tae008(startTime, endTime);
                case TimeActEventType.SetEventEditorColors: return new Tae016_SetEventEditorColors(startTime, endTime);
                case TimeActEventType.Type024: return new Tae024(startTime, endTime);
                case TimeActEventType.Type032: return new Tae032(startTime, endTime);
                case TimeActEventType.Type033: return new Tae033(startTime, endTime);
                case TimeActEventType.CastSelectedSpell: return new Tae064_CastSelectedSpell(startTime, endTime);
                case TimeActEventType.Type065: return new Tae065(startTime, endTime);
                case TimeActEventType.ApplySpEffect: return new Tae066_ApplySpEffect(startTime, endTime); 
                case TimeActEventType.ApplySpEffectB: return new Tae067_ApplySpEffectB(startTime, endTime);
                case TimeActEventType.SpawnSFXAtDmy: return new Tae096_SpawnSFXAtDmy(startTime, endTime);
                case TimeActEventType.SpawnSFXAtDmyB: return new Tae099_SpawnSFXAtDmyB(startTime, endTime);
                case TimeActEventType.SpawnSFXAtDmyC: return new Tae100_SpawnSFXAtDmyC(startTime, endTime);
                case TimeActEventType.DoSomethingToSFXSlot: return new Tae101_DoSomethingToSFXSlot(startTime, endTime);
                case TimeActEventType.SpawnSFXAtDmyD: return new Tae104_SpawnSFXAtDmyD(startTime, endTime);
                case TimeActEventType.SpawnSFXAtDmyAndFollow: return new Tae108_SpawnSFXAtDmyAndFollow(startTime, endTime);
                case TimeActEventType.SpawnSFXAtDmyE: return new Tae109_SpawnSFXAtDmyE(startTime, endTime);
                case TimeActEventType.DoSomethingToSFXSlotB: return new Tae110_DoSomethingToSFXSlotB(startTime, endTime);
                case TimeActEventType.Type112: return new Tae112(startTime, endTime);
                case TimeActEventType.Type114: return new Tae114(startTime, endTime);
                case TimeActEventType.Type115: return new Tae115(startTime, endTime);
                case TimeActEventType.Type116: return new Tae116(startTime, endTime);
                case TimeActEventType.WeaponTrail: return new Tae118_WeaponTrail(startTime, endTime); 
                case TimeActEventType.Type119: return new Tae119(startTime, endTime);
                case TimeActEventType.Type120: return new Tae120(startTime, endTime);
                case TimeActEventType.Type121: return new Tae121(startTime, endTime);
                case TimeActEventType.SoundBody: return new Tae128_SoundBody(startTime, endTime);
                case TimeActEventType.SoundDmyPoly: return new Tae129_SoundDmyPoly(startTime, endTime);
                case TimeActEventType.SoundOther: return new Tae130_SoundOther(startTime, endTime);
                case TimeActEventType.RumbleCamOnDmy: return new Tae144_RumbleCamOnDmy(startTime, endTime);
                case TimeActEventType.RumbleCamGlobal: return new Tae145_RumbleCamGlobal(startTime, endTime);
                case TimeActEventType.SetChrOpacity: return new Tae193_SetChrOpacity(startTime, endTime);
                case TimeActEventType.SetChrTrackingSpeed: return new Tae224_SetChrTrackingSpeed(startTime, endTime);
                case TimeActEventType.Type225: return new Tae225(startTime, endTime);
                case TimeActEventType.Type226: return new Tae226(startTime, endTime);
                case TimeActEventType.Type228: return new Tae228(startTime, endTime);
                case TimeActEventType.Type229: return new Tae229(startTime, endTime);
                case TimeActEventType.Type231: return new Tae231(startTime, endTime);
                case TimeActEventType.Type232: return new Tae232(startTime, endTime);
                case TimeActEventType.Type233: return new Tae233(startTime, endTime);
                case TimeActEventType.Type236: return new Tae236(startTime, endTime);
                case TimeActEventType.SetValue: return new Tae300_SetValue(startTime, endTime);
                case TimeActEventType.Type301: return new Tae301(startTime, endTime);
                case TimeActEventType.Type302: return new Tae302(startTime, endTime);
                case TimeActEventType.PlayAnimation: return new Tae303_PlayAnimation(startTime, endTime);
                case TimeActEventType.DoThrowDamage: return new Tae304_DoThrowDamage(startTime, endTime);
                case TimeActEventType.Type306: return new Tae306(startTime, endTime);
                case TimeActEventType.DoBehaviorKnockback: return new Tae307_DoBehaviorKnockback(startTime, endTime);
                case TimeActEventType.Type308: return new Tae308(startTime, endTime);
                case TimeActEventType.Type400: return new Tae400(startTime, endTime);
                case TimeActEventType.Type401: return new Tae401(startTime, endTime);
                case TimeActEventType.Type500: return new Tae500(startTime, endTime);
                // Demon's Souls Specific Event Types:
                case TimeActEventType.DeS_Type230: return new DeS_Tae230(startTime, endTime);
                case TimeActEventType.Remo_Type020: return new Remo_Tae020(startTime, endTime);
                case TimeActEventType.Remo_Type021: return new Remo_Tae021(startTime, endTime);
                case TimeActEventType.Remo_Type102: return new Remo_Tae102(startTime, endTime);
                case TimeActEventType.Remo_Type160: return new Remo_Tae160(startTime, endTime);
                case TimeActEventType.Remo_Type161: return new Remo_Tae161(startTime, endTime);
                case TimeActEventType.Remo_Type162: return new Remo_Tae162(startTime, endTime);
                case TimeActEventType.Remo_Type176: return new Remo_Tae176(startTime, endTime);
                case TimeActEventType.Remo_Type180: return new Remo_Tae180(startTime, endTime);
                case TimeActEventType.Remo_Type181: return new Remo_Tae181(startTime, endTime);
                case TimeActEventType.Remo_Type182: return new Remo_Tae182(startTime, endTime);
                case TimeActEventType.Remo_Type183: return new Remo_Tae183(startTime, endTime);
                case TimeActEventType.Remo_Type196: return new Remo_Tae196(startTime, endTime);
                case TimeActEventType.Remo_Type197: return new Remo_Tae197(startTime, endTime);
                case TimeActEventType.Remo_Type198: return new Remo_Tae198(startTime, endTime);
                case TimeActEventType.Remo_Type199: return new Remo_Tae199(startTime, endTime);
                case TimeActEventType.Remo_Type200: return new Remo_Tae200(startTime, endTime);
                case TimeActEventType.Remo_Type208: return new Remo_Tae208(startTime, endTime);
                case TimeActEventType.Remo_Type209: return new Remo_Tae209(startTime, endTime);
                case TimeActEventType.Remo_Type210: return new Remo_Tae210(startTime, endTime);
                case TimeActEventType.Remo_Type217: return new Remo_Tae217(startTime, endTime);
                case TimeActEventType.Remo_Type218: return new Remo_Tae218(startTime, endTime);
                case TimeActEventType.Remo_Type219: return new Remo_Tae219(startTime, endTime);
                case TimeActEventType.Remo_Type234: return new Remo_Tae234(startTime, endTime);
                case TimeActEventType.Remo_Type255: return new Remo_Tae255(startTime, endTime);
            }
            return null;
        }
    }
}
