using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TAE
{
    public class AnimationRef : Data
    {
        [Category("General")]
        [DisplayName("Anim ID")]
        [Description("The internal ID of the animation.")]
        public int ID { get; set; } = -1;

        [Category("General")]
        [DisplayName("Is Reference")]
        [Description("Determines whether this animation is a reference." +
            " If it is, then Referenced Anim ID is " +
            "used instead of Anim File Name.")]
        public bool IsReference { get; set; } = false;

        [Category("Only If \"Is Reference\" Is TRUE (Discarded Otherwise):")]
        [DisplayName("Referenced Anim ID")]
        [Description("Can ONLY stored in the file if \"Is Reference\" is True." +
            " Animation ID to reference. " +
            "This uses the same animation filename as the" +
            " referenced one uses.")]
        public int RefAnimID { get; set; } = -1;

        [Category("Only If \"Is Reference\" Is FALSE (Discarded Otherwise):")]
        [DisplayName("FromSoft Editor File Name")]
        [Description("Cannot be stored in the file if \"Is Reference\" is True. " +
            "This is metadata stored by FromSoft's editor app. Not read by the game.")]
        public string FileName { get; set; } = "a00_0000.hkxwin";

        [Browsable(false)]
        public ObservableCollection<TimeActEventBase> EventList { get; set; }
            = new ObservableCollection<TimeActEventBase>();

        [Browsable(false)]
        public ObservableCollection<TimeActEventGroup> EventGroupList { get; set; }
            = new ObservableCollection<TimeActEventGroup>();

        [Category("Only If \"Is Reference\" Is FALSE (Discarded Otherwise):")]
        [DisplayName("Is Looping Object Anim")]
        [Description("Cannot be stored in the file if \"Is Reference\" is True. Works only on objects; does not work on characters. " +
            "Causes the animation to loop indefinitely until event " +
            "scripts tell it to change animations.")]
        public bool IsLoopingObjAnim { get; set; } = false;

        //HKXのみ使いまし
        [Category("Only If \"Is Reference\" Is FALSE (Discarded Otherwise):")]
        [DisplayName("Use HKX Anim Data Only")]
        [Description("Cannot be stored in the file if \"Is Reference\" is True. Use HKX Anim Data Only")]
        public bool UseHKXOnly { get; set; } = false;

        //TAEデタのみ
        [Category("Only If \"Is Reference\" Is FALSE (Discarded Otherwise):")]
        [DisplayName("Use TAE Event Data Only")]
        [Description("Cannot be stored in the file if \"Is Reference\" is True. Use TAE Event Data Only")]
        public bool TAEDataOnly { get; set; } = false;

        [Category("Only If \"Is Reference\" Is FALSE (Discarded Otherwise):")]
        [DisplayName("Original Anim ID")]
        [Description("If this is -1, the HKX animation " +
            "file used is the one with the same ID as this " +
            "animation's ID. If it is >= 0, it uses the HKX " +
            "file of this number instead.")]
        public int OriginalAnimID { get; set; } = -1;

        

        //public void UpdateEventIndices()
        //{
        //    for (int i = 0; i < Events.Count; i++)
        //    {
        //        Events[i].DisplayIndex = i + 1;
        //    }
        //}

        public float GetLatestEventEndTime()
        {
            float latest = 0;
            foreach (var ev in EventList)
            {
                if (ev.EndTime > latest)
                {
                    latest = ev.EndTime;
                }
            }
            return latest;
        }

        public bool IsModified = false;

        //public AnimationEvent AddNewEvent()
        //{
        //    var newEvent = new AnimationEvent(Anim.Events.Count + 1, AnimationEventType.ApplySpecialProperty, ID);
        //    Anim.Events.Add(newEvent);
        //    return newEvent;
        //}

        public void ResetToDefaultFileName()
        {
            int leftNum = 00;
            int rightNum = 0000;

            if (ID >= 01_0000)
            {
                leftNum = ID / 01_0000;
                rightNum = ID % 01_0000;
            }
            else
            {
                rightNum = ID;
            }

            FileName = $"a{leftNum:D02}_{rightNum:D04}.HKXwin";
        }

        public override string ToString()
        {
            return $"[{ID}] {FileName}";
        }
    }
}
