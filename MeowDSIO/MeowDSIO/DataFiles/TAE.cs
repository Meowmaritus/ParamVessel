using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeowDSIO.DataTypes.TAE;
using System.ComponentModel;
using Newtonsoft.Json;
using MeowDSIO.DataTypes.TAE.Events;

namespace MeowDSIO.DataFiles
{
    
    public class TAE : DataFile
    {
        public IEnumerable<int> AnimationIDs => Animations.Select(x => x.ID);

        public void UpdateAnimGroupIndices()
        {
            for (int i = 0; i < AnimationGroups.Count; i++)
            {
                AnimationGroups[i].DisplayIndex = i + 1;
            }
        }

        public TAEHeader Header { get; set; }
        public string SkeletonName { get; set; }
        public string SibName { get; set; }
        public List<AnimationRef> Animations { get; set; } = new List<AnimationRef>();
        public List<AnimationGroup> AnimationGroups { get; set; } = new List<AnimationGroup>();

        public int EventHeaderSize
        {
            get
            {
                if (Header.VersionMajor == 11 && Header.VersionMinor == 1)
                    return 0x0C;
                else if (Header.VersionMajor == 1 && Header.VersionMinor == 0)
                    return 0x10;
                else
                    throw new NotImplementedException($"Don't know event header size of TAE Version {Header.VersionMajor}.{Header.VersionMinor}");
            }
        }

        private Animation LoadAnimationFromOffset(DSBinaryReader bin, int offset, int animID_ForDebug)
        {
            int oldOffset = (int)bin.BaseStream.Position;
            bin.BaseStream.Seek(offset, SeekOrigin.Begin);
            var anim = new Animation();

            try
            {
                int eventCount = bin.ReadInt32();
                int eventHeadersOffset = bin.ReadInt32();
                bin.BaseStream.Seek(0x10, SeekOrigin.Current); //skip shit we don't need
                int animFileOffset = bin.ReadInt32();

                for (int i = 0; i < eventCount; i++)
                {
                    //lazily seek to the start of each event manually.
                    bin.BaseStream.Seek(eventHeadersOffset + (EventHeaderSize * i), SeekOrigin.Begin);

                    int startTimeOffset = bin.ReadInt32();
                    int endTimeOffset = bin.ReadInt32();
                    int eventBodyOffset = bin.ReadInt32();

                    float startTime = -1;
                    float endTime = -1;

                    bin.StepIn(startTimeOffset);
                    {
                        startTime = bin.ReadSingle();
                    }
                    bin.StepOut();

                    bin.StepIn(endTimeOffset);
                    {
                        endTime = bin.ReadSingle();
                    }
                    bin.StepOut();

                    bin.StepIn(eventBodyOffset);
                    {
                        TimeActEventType eventType = (TimeActEventType)bin.ReadInt32();
                        int eventParamOffset = bin.ReadInt32();
                        bin.StepIn(eventParamOffset);
                        {
                            switch (eventType)
                            {
                                case TimeActEventType.Type000: var newType000 = new Tae000(startTime, endTime); newType000.ReadParameters(bin); anim.EventList.Add(newType000); break;
                                case TimeActEventType.Type001: var newType001 = new Tae001(startTime, endTime); newType001.ReadParameters(bin); anim.EventList.Add(newType001); break;
                                case TimeActEventType.Type002: var newType002 = new Tae002(startTime, endTime); newType002.ReadParameters(bin); anim.EventList.Add(newType002); break;
                                case TimeActEventType.Type005: var newType005 = new Tae005(startTime, endTime); newType005.ReadParameters(bin); anim.EventList.Add(newType005); break;
                                case TimeActEventType.Type008: var newType008 = new Tae008(startTime, endTime); newType008.ReadParameters(bin); anim.EventList.Add(newType008); break;
                                case TimeActEventType.Type016: var newType016 = new Tae016(startTime, endTime); newType016.ReadParameters(bin); anim.EventList.Add(newType016); break;
                                case TimeActEventType.Type024: var newType024 = new Tae024(startTime, endTime); newType024.ReadParameters(bin); anim.EventList.Add(newType024); break;
                                case TimeActEventType.Type032: var newType032 = new Tae032(startTime, endTime); newType032.ReadParameters(bin); anim.EventList.Add(newType032); break;
                                case TimeActEventType.Type033: var newType033 = new Tae033(startTime, endTime); newType033.ReadParameters(bin); anim.EventList.Add(newType033); break;
                                case TimeActEventType.Type064: var newType064 = new Tae064(startTime, endTime); newType064.ReadParameters(bin); anim.EventList.Add(newType064); break;
                                case TimeActEventType.Type065: var newType065 = new Tae065(startTime, endTime); newType065.ReadParameters(bin); anim.EventList.Add(newType065); break;
                                case TimeActEventType.Type066: var newType066 = new Tae066(startTime, endTime); newType066.ReadParameters(bin); anim.EventList.Add(newType066); break;
                                case TimeActEventType.Type067: var newType067 = new Tae067(startTime, endTime); newType067.ReadParameters(bin); anim.EventList.Add(newType067); break;
                                case TimeActEventType.Type096: var newType096 = new Tae096(startTime, endTime); newType096.ReadParameters(bin); anim.EventList.Add(newType096); break;
                                case TimeActEventType.Type099: var newType099 = new Tae099(startTime, endTime); newType099.ReadParameters(bin); anim.EventList.Add(newType099); break;
                                case TimeActEventType.Type100: var newType100 = new Tae100(startTime, endTime); newType100.ReadParameters(bin); anim.EventList.Add(newType100); break;
                                case TimeActEventType.Type101: var newType101 = new Tae101(startTime, endTime); newType101.ReadParameters(bin); anim.EventList.Add(newType101); break;
                                case TimeActEventType.Type104: var newType104 = new Tae104(startTime, endTime); newType104.ReadParameters(bin); anim.EventList.Add(newType104); break;
                                case TimeActEventType.Type108: var newType108 = new Tae108(startTime, endTime); newType108.ReadParameters(bin); anim.EventList.Add(newType108); break;
                                case TimeActEventType.Type109: var newType109 = new Tae109(startTime, endTime); newType109.ReadParameters(bin); anim.EventList.Add(newType109); break;
                                case TimeActEventType.Type110: var newType110 = new Tae110(startTime, endTime); newType110.ReadParameters(bin); anim.EventList.Add(newType110); break;
                                case TimeActEventType.Type112: var newType112 = new Tae112(startTime, endTime); newType112.ReadParameters(bin); anim.EventList.Add(newType112); break;
                                case TimeActEventType.Type114: var newType114 = new Tae114(startTime, endTime); newType114.ReadParameters(bin); anim.EventList.Add(newType114); break;
                                case TimeActEventType.Type115: var newType115 = new Tae115(startTime, endTime); newType115.ReadParameters(bin); anim.EventList.Add(newType115); break;
                                case TimeActEventType.Type116: var newType116 = new Tae116(startTime, endTime); newType116.ReadParameters(bin); anim.EventList.Add(newType116); break;
                                case TimeActEventType.Type118: var newType118 = new Tae118(startTime, endTime); newType118.ReadParameters(bin); anim.EventList.Add(newType118); break;
                                case TimeActEventType.Type119: var newType119 = new Tae119(startTime, endTime); newType119.ReadParameters(bin); anim.EventList.Add(newType119); break;
                                case TimeActEventType.Type120: var newType120 = new Tae120(startTime, endTime); newType120.ReadParameters(bin); anim.EventList.Add(newType120); break;
                                case TimeActEventType.Type121: var newType121 = new Tae121(startTime, endTime); newType121.ReadParameters(bin); anim.EventList.Add(newType121); break;
                                case TimeActEventType.Type128: var newType128 = new Tae128(startTime, endTime); newType128.ReadParameters(bin); anim.EventList.Add(newType128); break;
                                case TimeActEventType.Type129: var newType129 = new Tae129(startTime, endTime); newType129.ReadParameters(bin); anim.EventList.Add(newType129); break;
                                case TimeActEventType.Type130: var newType130 = new Tae130(startTime, endTime); newType130.ReadParameters(bin); anim.EventList.Add(newType130); break;
                                case TimeActEventType.Type144: var newType144 = new Tae144(startTime, endTime); newType144.ReadParameters(bin); anim.EventList.Add(newType144); break;
                                case TimeActEventType.Type145: var newType145 = new Tae145(startTime, endTime); newType145.ReadParameters(bin); anim.EventList.Add(newType145); break;
                                case TimeActEventType.Type193: var newType193 = new Tae193(startTime, endTime); newType193.ReadParameters(bin); anim.EventList.Add(newType193); break;
                                case TimeActEventType.Type224: var newType224 = new Tae224(startTime, endTime); newType224.ReadParameters(bin); anim.EventList.Add(newType224); break;
                                case TimeActEventType.Type225: var newType225 = new Tae225(startTime, endTime); newType225.ReadParameters(bin); anim.EventList.Add(newType225); break;
                                case TimeActEventType.Type226: var newType226 = new Tae226(startTime, endTime); newType226.ReadParameters(bin); anim.EventList.Add(newType226); break;
                                case TimeActEventType.Type228: var newType228 = new Tae228(startTime, endTime); newType228.ReadParameters(bin); anim.EventList.Add(newType228); break;
                                case TimeActEventType.Type229: var newType229 = new Tae229(startTime, endTime); newType229.ReadParameters(bin); anim.EventList.Add(newType229); break;
                                case TimeActEventType.Type231: var newType231 = new Tae231(startTime, endTime); newType231.ReadParameters(bin); anim.EventList.Add(newType231); break;
                                case TimeActEventType.Type232: var newType232 = new Tae232(startTime, endTime); newType232.ReadParameters(bin); anim.EventList.Add(newType232); break;
                                case TimeActEventType.Type233: var newType233 = new Tae233(startTime, endTime); newType233.ReadParameters(bin); anim.EventList.Add(newType233); break;
                                case TimeActEventType.Type236: var newType236 = new Tae236(startTime, endTime); newType236.ReadParameters(bin); anim.EventList.Add(newType236); break;
                                case TimeActEventType.Type300: var newType300 = new Tae300(startTime, endTime); newType300.ReadParameters(bin); anim.EventList.Add(newType300); break;
                                case TimeActEventType.Type301: var newType301 = new Tae301(startTime, endTime); newType301.ReadParameters(bin); anim.EventList.Add(newType301); break;
                                case TimeActEventType.Type302: var newType302 = new Tae302(startTime, endTime); newType302.ReadParameters(bin); anim.EventList.Add(newType302); break;
                                case TimeActEventType.Type303: var newType303 = new Tae303(startTime, endTime); newType303.ReadParameters(bin); anim.EventList.Add(newType303); break;
                                case TimeActEventType.Type304: var newType304 = new Tae304(startTime, endTime); newType304.ReadParameters(bin); anim.EventList.Add(newType304); break;
                                case TimeActEventType.Type306: var newType306 = new Tae306(startTime, endTime); newType306.ReadParameters(bin); anim.EventList.Add(newType306); break;
                                case TimeActEventType.Type307: var newType307 = new Tae307(startTime, endTime); newType307.ReadParameters(bin); anim.EventList.Add(newType307); break;
                                case TimeActEventType.Type308: var newType308 = new Tae308(startTime, endTime); newType308.ReadParameters(bin); anim.EventList.Add(newType308); break;
                                case TimeActEventType.Type401: var newType401 = new Tae401(startTime, endTime); newType401.ReadParameters(bin); anim.EventList.Add(newType401); break;
                                case TimeActEventType.Type500: var newType500 = new Tae500(startTime, endTime); newType500.ReadParameters(bin); anim.EventList.Add(newType500); break;
                            }
                        }
                        bin.StepOut();
                    }
                    bin.StepOut();
                }

                bin.BaseStream.Seek(animFileOffset, SeekOrigin.Begin);

                int fileType = bin.ReadInt32();
                if (fileType == 0)
                {
                    anim.IsReference = false;

                    int dataOffset = bin.ReadInt32();
                    //bin.BaseStream.Seek(dataOffset, SeekOrigin.Begin);

                    int nameOffset = bin.ReadInt32();

                    anim.Unk1 = bin.ReadInt32();
                    anim.Unk2 = bin.ReadInt32();

                    if (nameOffset <= 0)
                    {
                        throw new Exception("Anim file type was that of a named one but the name pointer was NULL.");
                    }
                    bin.BaseStream.Seek(nameOffset, SeekOrigin.Begin);
                    anim.FileName = ReadUnicodeString(bin);
                }
                else if (fileType == 1)
                {
                    anim.IsReference = true;

                    anim.FileName = null;

                    bin.ReadInt32(); //offset pointing to next dword for some reason.
                    bin.ReadInt32(); //offset pointing to start of next anim file struct

                    anim.RefAnimID = bin.ReadInt32();
                    //Null 1
                    //Null 2
                    //Null 3
                }
                else
                {
                    throw new Exception($"Unknown anim file type code: {fileType}");
                }

                return anim;
            }
            catch (EndOfStreamException)
            {
                MiscUtil.PrintlnDX($"Warning: reached end of file while parsing animation {animID_ForDebug}; data may not be complete.", ConsoleColor.Yellow);
                //if (!MiscUtil.ConsolePrompYesNo("Would you like to continue loading the file and run the risk of " + 
                //    "accidentally outputting a file that might be missing some of its original data?"))
                //{
                //    throw new LoadAbortedException();
                //}
                //else
                //{
                //    return a;
                //}

                return anim;
            }
            finally
            {
                bin.BaseStream.Seek(oldOffset, SeekOrigin.Begin);
            }
        }

        private string ReadUnicodeString(DSBinaryReader bin)
        {
            StringBuilder sb = new StringBuilder();
            byte[] next = { 0, 0 };
            bool endString = false;
            do
            {
                next = bin.ReadBytes(2);
                endString = (next[0] == 0 && next[1] == 0);

                if (!endString)
                {
                    sb.Append(Encoding.Unicode.GetString(next));
                }
            }
            while (!endString);
            return sb.ToString();
        }

        //TODO: Measure real progress.
        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            Header = new TAEHeader();
            var fileSignature = bin.ReadBytes(4);
            if (fileSignature.Where((x, i) => x != Header.Signature[i]).Any())
            {
                throw new Exception($"Invalid signature in this TAE file: " + 
                    $"[{string.Join(",", fileSignature.Select(x => x.ToString("X8")))}] " + 
                    $"(ASCII: '{Encoding.ASCII.GetString(fileSignature)}')");
            }

            Header.IsBigEndian = bin.ReadBoolean();

            bin.BigEndian = Header.IsBigEndian;
            bin.ReadBytes(3); //3 null bytes after big endian flag

            Header.VersionMajor = bin.ReadUInt16();
            Header.VersionMinor = bin.ReadUInt16();

            var fileSize = bin.ReadInt32();

            Header.UnknownB00 = bin.ReadUInt32();
            Header.UnknownB01 = bin.ReadUInt32();
            Header.UnknownB02 = bin.ReadUInt32();
            Header.UnknownB03 = bin.ReadUInt32();

            Header.UnknownFlags = bin.ReadBytes(TAEHeader.UnknownFlagsLength);

            Header.FileID = bin.ReadInt32();
            
            //Animation IDs
            var animCount = bin.ReadInt32();
            int OFF_AnimID = bin.ReadInt32();
            bin.DoAt(OFF_AnimID, () =>
            {
                for (int i = 0; i < animCount; i++)
                {
                    var animID = bin.ReadInt32();
                    var animOffset = bin.ReadInt32();
                    var anim = LoadAnimationFromOffset(bin, animOffset, animID);
                    var animRef = new AnimationRef() { ID = animID, Anim = anim };

                    Animations.Add(animRef);
                }
            });

            //Anim Groups
            int OFF_AnimGroups = bin.ReadInt32();
            bin.DoAt(OFF_AnimGroups, () =>
            {
                int animGroupCount = bin.ReadInt32();
                int actualAnimGroupsOffset = bin.ReadInt32();

                bin.BaseStream.Seek(actualAnimGroupsOffset, SeekOrigin.Begin);

                for (int i = 0; i < animGroupCount; i++)
                {
                    var nextAnimGroup = new AnimationGroup(i + 1);
                    nextAnimGroup.FirstID = bin.ReadInt32();
                    nextAnimGroup.LastID = bin.ReadInt32();
                    var _firstIdOffset = bin.ReadInt32();

                    AnimationGroups.Add(nextAnimGroup);
                }
            });

            Header.UnknownC = bin.ReadInt32();
            //We already found the animation count and offsets from the anim ids earlier
            int _animCount = bin.ReadInt32();
            int _animOffset = bin.ReadInt32();
            if (_animCount != Animations.Count)
            {
                throw new Exception($"Animation IDs count [{Animations.Count}] is different than Animations count [{_animCount}]!");
            }

            Header.UnknownE00 = bin.ReadUInt32();
            Header.UnknownE01 = bin.ReadUInt32();
            Header.UnknownE02 = bin.ReadUInt32();
            Header.UnknownE03 = bin.ReadUInt32();
            Header.UnknownE04 = bin.ReadUInt32();
            Header.FileID2 = bin.ReadInt32();
            Header.FileID3 = bin.ReadInt32();
            Header.UnknownE07 = bin.ReadUInt32();
            Header.UnknownE08 = bin.ReadUInt32();
            Header.UnknownE09 = bin.ReadUInt32();

            int filenamesOffset = bin.ReadInt32();
            bin.BaseStream.Seek(filenamesOffset, SeekOrigin.Begin);

            int skeletonNameOffset = bin.ReadInt32();
            int sibNameOffset = bin.ReadInt32();

            bin.BaseStream.Seek(skeletonNameOffset, SeekOrigin.Begin);

            SkeletonName = ReadUnicodeString(bin);

            bin.BaseStream.Seek(sibNameOffset, SeekOrigin.Begin);

            SibName = ReadUnicodeString(bin);
        }

        //TODO: Measure real progress.
        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            //SkeletonName, SibName:
            bin.Seek(0x94, SeekOrigin.Begin);
            bin.Write(0x00000098);
            bin.Write(0x000000A8);
            bin.Write(0x000000A8 + (SkeletonName.Length * 2 + 2));
            bin.Write(0x00000000);
            bin.Write(0x00000000);
            bin.Write(Encoding.Unicode.GetBytes(SkeletonName));
            bin.Write((short)0); //string terminator
            bin.Write(Encoding.Unicode.GetBytes(SibName));
            bin.Write((short)0); //string terminator

            //Animation IDs - First Pass
            int OFF_AnimationIDs = (int)bin.BaseStream.Position;

            var animationIdOffsets = new Dictionary<int, int>(); //<animation ID, offset>
            foreach (var anim in Animations)
            {
                animationIdOffsets.Add(anim.ID, (int)bin.BaseStream.Position);
                bin.Write(anim.ID);
                bin.Placeholder(); //Pointer to animation will be inserted here.
            }

            //Animation Groups - Full Pass
            int OFF_AnimationGroups = (int)bin.BaseStream.Position;
            bin.Write(AnimationGroups.Count);
            bin.Write((int)(bin.BaseStream.Position + 4)); //Pointer that always points to the next dword for some reason.

            foreach (var g in AnimationGroups)
            {
                bin.Write(g.FirstID);
                bin.Write(g.LastID);

                if (!animationIdOffsets.ContainsKey(g.FirstID))
                    throw new Exception($"Animation group begins on an animation ID that isn't " + 
                        $"present in the list of animations.");

                bin.Write(animationIdOffsets[g.FirstID]);
            }

            //Animation First Pass
            int OFF_Animations = (int)bin.BaseStream.Position;
            var animationOffsets = new Dictionary<int, int>(); //<animation ID, offset>
            var animationTimeConstantLists = new Dictionary<int, List<float>>(); //<animation ID, List<time constant>>
            foreach (var anim in Animations)
            {
                animationOffsets.Add(anim.ID, (int)bin.BaseStream.Position);
                bin.Write(anim.Anim.EventList.Count);
                bin.Placeholder(); //PLACEHOLDER: animation event headers offset
                //Println($"Wrote Anim{anim.Key} event header offset placeholder value (0xDEADD00D) at address {(bin.BaseStream.Position-4):X8}");
                bin.Write(0); //Null 1
                bin.Write(0); //Null 2
                animationTimeConstantLists.Add(anim.ID, new List<float>());
                //Populate all of the time constants used:
                foreach (var e in anim.Anim.EventList)
                {
                    if (!animationTimeConstantLists[anim.ID].Contains(e.StartTime))
                        animationTimeConstantLists[anim.ID].Add(e.StartTime);
                    if (!animationTimeConstantLists[anim.ID].Contains(e.EndTime))
                        animationTimeConstantLists[anim.ID].Add(e.EndTime);
                }
                bin.Write(animationTimeConstantLists[anim.ID].Count); //# time constants in this anim
                bin.Placeholder(); //PLACEHOLDER: Time Constants offset
                //Println($"Wrote Anim{anim.Key} time constant offset placeholder value (0xDEADD00D) at address {(bin.BaseStream.Position-4):X8}");
                bin.Placeholder(); //PLACEHOLDER: Animation file struct offset
                //Println($"Wrote Anim{anim.Key} anim file offset placeholder value (0xDEADD00D) at address {(bin.BaseStream.Position-4):X8}");
            }



            //Animation ID Second Pass
            bin.DoAt(OFF_AnimationIDs, () =>
            {
                foreach (var anim in Animations)
                {
                    //Move from the animation ID offset to the animation pointer offset.
                    bin.Jump(4);

                    //Write pointer to animation into this animation ID.
                    bin.Write(animationOffsets[anim.ID]);
                }
            });


            //Animations Second Pass
            var eventHeaderStartOffsets = new Dictionary<int, int>(); //<anim ID, event header start offset>
            var animationFileOffsets = new Dictionary<int, int>(); //<animation ID, offset>
            var animTimeConstantStartOffsets = new Dictionary<int, int>(); //<animation ID, start offset>
            var animTimeConstantOffsets = new Dictionary<int, Dictionary<float, int>>(); //<animation ID, Dictionary<time const, offset>>
            {
                //The unnamed animation files contain the ID of the last named animation file.
                //int lastNamedAnimation = -1;

                //TODO: Check if it's possible for the very first animation data listed to be unnamed, 
                //and what would go in the last named animation ID value if that were the case?

                foreach (var anim in Animations)
                {
                    //ANIMATION FILE:
                    {
                        //Write anim file struct:
                        animationFileOffsets.Add(anim.ID, (int)bin.BaseStream.Position);
                        if (!anim.Anim.IsReference)
                        {
                            bin.Write(0x00000000); //type 0 - named
                            bin.Write((int)(bin.BaseStream.Position + 0x04)); //offset pointing to next dword for some reason.
                            bin.Write((int)(bin.BaseStream.Position + 0x10)); //offset pointing to name start
                            bin.Write(anim.Anim.Unk1); //Unknown
                            bin.Write(anim.Anim.Unk2); //Unknown
                            bin.Write(0x00000000); //Null
                            //name start:
                            if (anim.Anim.FileName.Length > 0)
                            {
                                bin.Write(Encoding.Unicode.GetBytes(anim.Anim.FileName));
                            }
                            bin.Write((short)0); //string terminator
                        }
                        else
                        {
                            bin.Write(0x00000001); //type 1 - nameless
                            bin.Write((int)(bin.BaseStream.Position + 0x04)); //offset pointing to next dword for some reason.
                            bin.Write((int)(bin.BaseStream.Position + 0x14)); //offset pointing to start of next anim file struct
                            bin.Write(anim.Anim.RefAnimID); //Last named animation ID, to which this one is linked.
                            bin.Write(0x00000000); //Null 1
                            bin.Write(0x00000000); //Null 2
                            bin.Write(0x00000000); //Null 3
                        }
                    }
                    animTimeConstantStartOffsets.Add(anim.ID, (int)bin.BaseStream.Position);
                    //Write the time constants and record their offsets:
                    animTimeConstantOffsets.Add(anim.ID, new Dictionary<float, int>());
                    foreach (var tc in animationTimeConstantLists[anim.ID])
                    {
                        animTimeConstantOffsets[anim.ID].Add(tc, (int)bin.BaseStream.Position);
                        bin.Write(tc);
                    }


                    //Event headers (note: all event headers are (EventHeaderSize) long):
                    eventHeaderStartOffsets.Add(anim.ID, (int)bin.BaseStream.Position);
                    foreach (var e in anim.Anim.EventList)
                    {
                        long currentEventHeaderStart = bin.Position;
                        bin.Write((int)animTimeConstantOffsets[anim.ID][e.StartTime]); //offset of start time in time constants.
                        bin.Write((int)animTimeConstantOffsets[anim.ID][e.EndTime]); //offset of end time in time constants.
                        bin.Placeholder(); //PLACEHOLDER: Event body
                        long currentEventHeaderLength = bin.Position - currentEventHeaderStart;
                        if (currentEventHeaderLength < EventHeaderSize)
                        {
                            bin.Write(new byte[EventHeaderSize - currentEventHeaderLength]);
                        }
                    }

                    //Event bodies
                    var eventBodyOffsets = new Dictionary<TimeActEventBase, int>();
                    foreach (var e in anim.Anim.EventList)
                    {
                        eventBodyOffsets.Add(e, (int)bin.BaseStream.Position);

                        bin.Write((int)e.EventType);
                        bin.Write((int)(bin.BaseStream.Position + 4)); //one of those pointers to very next dword.

                        //Note: the logic for the length of a particular event param array is handled 
                        //      in the read function as well as in the AnimationEvent class itself.

                        e.WriteParameters(bin);

                        //foreach (var p in e.Parameters)
                        //{
                        //    var paramVal = p.Value.ToUpper();
                        //    if (paramVal.EndsWith("F") || paramVal.Contains(".") || paramVal.Contains(","))
                        //    {
                        //        bin.Write(float.Parse(paramVal.Replace("F", "")));
                        //    }
                        //    else
                        //    {
                        //        bin.Write(int.Parse(paramVal));
                        //    }
                        //}
                    }

                    //Event headers pass 2:
                    bin.DoAt(eventHeaderStartOffsets[anim.ID], () =>
                    {
                        foreach (var e in anim.Anim.EventList)
                        {
                            //skip to event body offset field:
                            bin.Seek(8, SeekOrigin.Current);

                            //write event body offset:
                            bin.Write(eventBodyOffsets[e]);
                        }
                    });

                }
            }

            //Animations Third Pass
            bin.DoAt(OFF_Animations, () =>
            {
                foreach (var anim in Animations)
                {
                    bin.Seek(animationOffsets[anim.ID] + 4, SeekOrigin.Begin);
                    //event header start offset:
                    if (anim.Anim.EventList.Count > 0)
                        bin.Write(eventHeaderStartOffsets[anim.ID]);
                    else
                        bin.Write(0x00000000);
                    //Println($"Wrote Anim{anim.Key} event header offset value 0x{eventHeaderStartOffsets[anim.Key]:X8} at address {(bin.BaseStream.Position-4):X8}");
                    bin.Seek(0xC, SeekOrigin.Current);
                    //time constants offset (writing offset of the *first constant listed*):
                    if (animationTimeConstantLists[anim.ID].Count > 0)
                    {
                        bin.Write(animTimeConstantStartOffsets[anim.ID]);
                        //Println($"Wrote Anim{anim.Key} time constants offset value 0x{animTimeConstantStartOffsets[anim.Key]:X8} at address {(bin.BaseStream.Position-4):X8}");
                    }
                    else
                    {
                        bin.Write(0x00000000); //Null
                        //Println($"Wrote Anim{anim.Key} time constants offset value NULL at address {(bin.BaseStream.Position-4):X8}");
                    }
                    //anim file struct offset:
                    bin.Write((int)animationFileOffsets[anim.ID]);
                    //Println($"Wrote Anim{anim.Key} anim file offset value 0x{((int)animationFileOffsets[anim.Key]):X8} at address {(bin.BaseStream.Position-4):X8}");
                }
            });

            var END_OF_FILE = (int)bin.Position;

            //final header write:
            bin.Seek(0, SeekOrigin.Begin);

            bin.Write(Header.Signature);

            bin.Write(Header.IsBigEndian);
            bin.BigEndian = Header.IsBigEndian;
            bin.Write(new byte[] { 0, 0, 0 }); //3 null bytes after big endian flag.

            bin.Write(Header.VersionMajor);
            bin.Write(Header.VersionMinor);

            bin.Write((int)bin.BaseStream.Length); //File length

            bin.Write(Header.UnknownB00);
            bin.Write(Header.UnknownB01);
            bin.Write(Header.UnknownB02);
            bin.Write(Header.UnknownB03);

            bin.Write(Header.UnknownFlags);

            bin.Write(Header.FileID);
            bin.Write(Animations.Count); //Technically the animation ID count
            bin.Write(OFF_AnimationIDs);
            bin.Write(OFF_AnimationGroups);
            bin.Write(Header.UnknownC);
            bin.Write(Animations.Count); //Techically the animation count
            bin.Write(OFF_Animations);

            bin.Write(Header.UnknownE00);
            bin.Write(Header.UnknownE01);
            bin.Write(Header.UnknownE02);
            bin.Write(Header.UnknownE03);
            bin.Write(Header.UnknownE04);
            bin.Write(Header.FileID2);
            bin.Write(Header.FileID3);
            bin.Write(Header.UnknownE07);
            bin.Write(Header.UnknownE08);
            bin.Write(Header.UnknownE09);

            //Here would be the value at the very beginning of this method!

            //Go to end and pretend like this was a totally normal file write and not the shitshow it was.
            bin.Seek(END_OF_FILE, SeekOrigin.Begin);
        }
    }
}
