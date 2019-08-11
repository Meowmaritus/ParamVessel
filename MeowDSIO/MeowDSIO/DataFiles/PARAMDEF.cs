using MeowDSIO.DataTypes.PARAMDEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MeowDSIO.DataFiles
{
    public class PARAMDEF : DataFile, IList<ParamDefEntry>
    {
        public string ID { get; set; } = null;
        public ushort Unknown1 { get; set; } = 1;
        //public ushort Unknown2 { get; set; } = 0;

        public List<ParamDefEntry> Entries { get; set; } = new List<ParamDefEntry>();

        public const ushort ENTRY_LENGTH = 0x00B0;
        public const ushort DESC_OFFSET_OFFSET = 0x0068;

        public short Version { get; set; } = 48;
        public bool BB_IsUnicode { get; set; } = false;
        public int BB_Unknown1 { get; set; } = 56;
        public int BB_Unknown2 { get; set; } = 0;
        public short EntryLength { get; set; } = 156;
        public byte UnknownFlag01 { get; set; } = 0;
        public ushort Unknown3 { get; set; } = 0;

        public ParamDefEntry GetEntry(string entryName)
        {
            try
            {
                return Entries.Where(x => x.Name == entryName).First();
            }
            catch
            {
                return null;
            }
        }

        //public int CalculateEntrySize()
        //{
        //    int totalSize = 0;
        //    int bitField = 0;

        //    foreach (var e in Entries)
        //    {
        //        //bool
        //        if (e.ValueBitCount == 1)
        //        {
        //            bitField++;
        //            if (bitField == 8)
        //            {
        //                bitField = 0;
        //                totalSize++;
        //            }
        //        }
        //        else
        //        {
        //            if (e.InternalValueType == ParamTypeDef.dummy8)
        //            {
        //                if (bitField > 0)
        //                {
        //                    bitField = 0;
        //                    totalSize++;
        //                }
        //            }
        //            else if (e.InternalValueType == ParamTypeDef.u8)
        //            {
        //                if (e.ValueBitCount != 8)
        //                {
        //                    for (int i = 0; i < e.ValueBitCount; i++)
        //                    {
        //                        bitField++;
        //                        if (bitField == 8)
        //                        {
        //                            bitField = 0;
        //                            totalSize++;
        //                        }
        //                    }
        //                }
        //            }

                    
        //        }

        //        totalSize += e.ValueBitCount / 8;
        //    }

        //    return (totalSize);
        //}

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            int length = bin.ReadInt32();
            bin.BigEndian = false;
            //if (length != bin.Length)
            //{
            //    // If lengths don't match, try opposite endianness.
            //    bin.BigEndian = !bin.BigEndian;
            //    bin.Position = 0;
            //    length = bin.ReadInt32();

            //    if (length != bin.Length)
            //    {
            //        throw new Exception("ParamDef File Length Value was incorrect on both little endian and big endian. File is not valid.");
            //    }
            //}

            Version = bin.ReadInt16();

            Unknown1 = bin.ReadUInt16();

            var entryCount = bin.ReadUInt16();

            EntryLength = bin.ReadInt16();

            ID = bin.ReadPaddedStringShiftJIS(0x20, padding: null);

            UnknownFlag01 = bin.ReadByte();
            BB_IsUnicode = bin.ReadBoolean();
            Unknown3 = bin.ReadUInt16();

            if (Version != 0x30 && Version == 0xFF)
            {
                BB_Unknown1 = bin.ReadInt32();
                BB_Unknown2 = bin.ReadInt32();
            }

            var descriptionOffsets = new List<long>();

            Entries.Clear();

            var OFF_EntryStart = bin.Position;

            for (int i = 0; i < entryCount; i++)
            {
                var currentEntryStart = bin.Position = OFF_EntryStart + (EntryLength * i);

                var paramDefEntry = new ParamDefEntry();

                try
                {
                    if (Version == 0x30)
                    {
                        paramDefEntry.DisplayName = bin.ReadPaddedStringShiftJIS(0x40, null);
                    }
                    else if (Version == 0xFF)
                    {
                        if (BB_IsUnicode)
                        {
                            paramDefEntry.DisplayName = bin.ReadPaddedStringUnicode(0x40, null);
                        }
                        else
                        {
                            paramDefEntry.DisplayName = bin.ReadPaddedStringShiftJIS(0x40, null);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

                string _guiValueType_str = bin.ReadPaddedStringShiftJIS(0x8, padding: null);
                if (!Enum.TryParse(_guiValueType_str, out ParamTypeDef guiValueType))
                {
                    throw new Exception($"Invalid [{nameof(ParamTypeDef)} " +
                        $"{nameof(ParamDefEntry)}.{nameof(ParamDefEntry.GuiValueType)}] " +
                        $"value string found in PARAMDEF entry: '{_guiValueType_str}'.");
                }
                paramDefEntry.GuiValueType = guiValueType;

                paramDefEntry.GuiValueStringFormat = bin.ReadPaddedStringShiftJIS(0x8, padding: null);
                paramDefEntry.DefaultValue = bin.ReadSingle();
                paramDefEntry.Min = bin.ReadSingle();
                paramDefEntry.Max = bin.ReadSingle();
                paramDefEntry.Increment = bin.ReadSingle();
                paramDefEntry.GuiValueDisplayMode = bin.ReadInt32();
                paramDefEntry.GuiValueByteCount = bin.ReadInt32();

                if (Version == 0x30)
                {
                    int descriptionOffset = bin.ReadInt32();
                    descriptionOffsets.Add(descriptionOffset);
                }
                else if (Version == 0xFF)
                {
                    long descriptionOffset = bin.ReadInt64();
                    descriptionOffsets.Add(descriptionOffset);
                }
                

                string _internalValueType_str = bin.ReadPaddedStringShiftJIS(0x20, padding: null);
                if (!Enum.TryParse(_internalValueType_str, out ParamTypeDef internalValueType))
                {
                    throw new Exception($"Invalid [{nameof(ParamTypeDef)} " +
                        $"{nameof(ParamDefEntry)}.{nameof(ParamDefEntry.InternalValueType)}] " +
                        $"value string found in PARAMDEF entry: '{_internalValueType_str}'.");
                }
                paramDefEntry.InternalValueType = internalValueType;

                paramDefEntry.Name = bin.ReadPaddedStringShiftJIS(0x20, padding: null);
                paramDefEntry.ID = bin.ReadInt32();

                if (paramDefEntry.Name.Contains(":"))
                {
                    paramDefEntry.ValueBitCount = int.Parse(paramDefEntry.Name.Split(':')[1]);

                    //if (entry.ValueBitCount > 1)
                    //{
                    //    entry.InternalValueType = ParamTypeDef.u8;
                    //}
                }
                else if (paramDefEntry.InternalValueType == ParamTypeDef.dummy8)
                {
                    var lbrack = paramDefEntry.Name.LastIndexOf("[");

                    if (lbrack == -1)
                    {
                        paramDefEntry.ValueBitCount = 8;
                    }
                    else
                    {
                        var rbrack = paramDefEntry.Name.LastIndexOf("]");

                        var padSizeStr = paramDefEntry.Name.Substring(lbrack + 1, rbrack - lbrack - 1);

                        paramDefEntry.ValueBitCount = int.Parse(padSizeStr) * 8;
                    }

                }
                else
                {
                    paramDefEntry.ValueBitCount = paramDefEntry.GuiValueByteCount * 8;

                }


                var currentOffsetPastEntryStart = bin.Position - currentEntryStart;

                var leftoverUnmappedByteCount = EntryLength - currentOffsetPastEntryStart;

                if (leftoverUnmappedByteCount > 0)
                {
                    paramDefEntry.UNMAPPED_DATA = bin.ReadBytes((int)leftoverUnmappedByteCount);
                }


                Entries.Add(paramDefEntry);

                prog?.Report((i, entryCount * 2));
            }

            for (int i = 0; i < entryCount; i++)
            {
                if (descriptionOffsets[i] > 0)
                {
                    bin.Position = descriptionOffsets[i];
                    if (Version == 0x30)
                    {
                        Entries[i].Description = bin.ReadStringShiftJIS(-1, true);
                    }
                    else if (Version == 0xFF)
                    {
                        if (BB_IsUnicode)
                        {
                            Entries[i].Description = bin.ReadStringUnicode(null);
                        }
                        else
                        {
                            Entries[i].Description = bin.ReadStringShiftJIS(-1, true);
                        }
                    }
                }
                else
                {
                    Entries[i].Description = null;
                }

                prog?.Report((entryCount + i, entryCount * 2));
            }

        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            // Placeholder - file length
            bin.Placeholder32();
            // Version
            bin.Write(Version);
            bin.Write(Unknown1);
            bin.Write((ushort)Entries.Count);
            // Entry length
            bin.Write(EntryLength);
            bin.WritePaddedStringShiftJIS(ID, 0x20, padding: 0x20);
            bin.Write(UnknownFlag01);
            bin.Write(BB_IsUnicode);
            bin.Write(Unknown3);

            if (Version != 0x30 && Version == 0xFF)
            {
                bin.Write(BB_Unknown1);
                bin.Write(BB_Unknown2);
            }

            var descriptionOffsets = new List<long>();

            var OFF_Entries = bin.Position;

            bin.Position += (Entries.Count * ENTRY_LENGTH);

            for (int i = 0; i < Entries.Count; i++)
            {
                if (Entries[i].Description != null)
                {
                    descriptionOffsets.Add(bin.Position);

                    if (Version == 0x30)
                    {
                        bin.WriteStringShiftJIS(Entries[i].Description, true);
                    }
                    else if (Version == 0xFF)
                    {
                        if (BB_IsUnicode)
                        {
                            bin.WriteStringUnicode(Entries[i].Description, true);
                        }
                        else
                        {
                            bin.WriteStringShiftJIS(Entries[i].Description, true);
                        }
                    }
                }
                else
                {
                    descriptionOffsets.Add(-1);
                }

                

                prog?.Report((i, Entries.Count * 2));
            }

            // Fill in length real quick
            bin.Position = 0;
            bin.Write((uint)bin.Length);

            bin.Position = OFF_Entries;

            for (int i = 0; i < Entries.Count; i++)
            {
                bin.Position = OFF_Entries + (i * EntryLength);

                if (Version == 48)
                {
                    bin.WritePaddedStringShiftJIS(Entries[i].DisplayName, 0x40, null, false);
                }
                else if (Version == 255)
                {
                    if (BB_IsUnicode)
                    {
                        bin.WritePaddedStringUnicode(Entries[i].DisplayName, 0x40, null, false);
                    }
                    else
                    {
                        bin.WritePaddedStringShiftJIS(Entries[i].DisplayName, 0x40, null, false);
                    }
                }

                bin.WritePaddedStringShiftJIS(Entries[i].GuiValueType.ToString(), 0x8, padding: 0x20);
                bin.WritePaddedStringShiftJIS(Entries[i].GuiValueStringFormat, 0x8, padding: 0x20);
                bin.Write(Entries[i].DefaultValue);
                bin.Write(Entries[i].Min);
                bin.Write(Entries[i].Max);
                bin.Write(Entries[i].Increment);
                bin.Write(Entries[i].GuiValueDisplayMode);
                bin.Write(Entries[i].GuiValueByteCount);

                if (Version == 0x30)
                {
                    bin.Write((uint)(descriptionOffsets[i]));
                }
                else if (Version == 0xFF)
                {
                    bin.Write(descriptionOffsets[i]);
                }

                bin.WritePaddedStringShiftJIS(Entries[i].InternalValueType.ToString(), 0x20, padding: 0x20);
                bin.WritePaddedStringShiftJIS(Entries[i].Name, 0x20, padding: 0x20);
                bin.Write(Entries[i].ID);

                if (Entries[i].UNMAPPED_DATA != null && Entries[i].UNMAPPED_DATA.Length > 0)
                {
                    bin.Write(Entries[i].UNMAPPED_DATA);
                }

                prog?.Report((Entries.Count + i, Entries.Count * 2));
            }
        }

        public int IndexOf(ParamDefEntry item)
        {
            return ((IList<ParamDefEntry>)Entries).IndexOf(item);
        }

        public void Insert(int index, ParamDefEntry item)
        {
            ((IList<ParamDefEntry>)Entries).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<ParamDefEntry>)Entries).RemoveAt(index);
        }

        public ParamDefEntry this[int index] { get => ((IList<ParamDefEntry>)Entries)[index]; set => ((IList<ParamDefEntry>)Entries)[index] = value; }

        public void Add(ParamDefEntry item)
        {
            ((IList<ParamDefEntry>)Entries).Add(item);
        }

        public void Clear()
        {
            ((IList<ParamDefEntry>)Entries).Clear();
        }

        public bool Contains(ParamDefEntry item)
        {
            return ((IList<ParamDefEntry>)Entries).Contains(item);
        }

        public void CopyTo(ParamDefEntry[] array, int arrayIndex)
        {
            ((IList<ParamDefEntry>)Entries).CopyTo(array, arrayIndex);
        }

        public bool Remove(ParamDefEntry item)
        {
            return ((IList<ParamDefEntry>)Entries).Remove(item);
        }

        public int Count => ((IList<ParamDefEntry>)Entries).Count;

        public bool IsReadOnly => ((IList<ParamDefEntry>)Entries).IsReadOnly;

        public IEnumerator<ParamDefEntry> GetEnumerator()
        {
            return ((IList<ParamDefEntry>)Entries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<ParamDefEntry>)Entries).GetEnumerator();
        }
    }
}
