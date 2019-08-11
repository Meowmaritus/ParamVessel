using MeowDSIO.DataTypes.PARAM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MeowDSIO.DataFiles
{
    public enum ParamGameType
    {
        DarkSouls1,
        Bloodborne,
    }

    public class PARAM : DataFile, IList<ParamRow>
    {
        public string ID { get; set; } = null;

        public short UNK1 { get; set; } = 1;
        public short UNK2 { get; set; } = 2;

        public ObservableCollection<ParamRow> Entries { get; set; }
        public short BB_Unknown1 = 0;
        public short BB_Unknown2 = 1;
        public int ParamFormatVersion { get; set; }

        public ParamGameType GameType { get; set; } = ParamGameType.DarkSouls1;

        public bool IsBigEndian { get; set; } = false;

        public bool IsDarkSoulsRemastered { get; set; } = false;

        public int EntrySize { get; set; } = -1;

        public PARAMDEF AppliedPARAMDEF { get; set; } = null;

        public void ApplyPARAMDEFTemplate(PARAMDEF def)
        {
            AppliedPARAMDEF = def;

            foreach (var e in Entries)
            {
                e.LoadValuesFromRawData(this);
                e.ClearRawData();
            }
        }

        public ParamRow GetRow(int id)
        {
            var possibleRows = Entries.Where(x => x.ID == id);
            if (possibleRows.Any())
                return possibleRows.First();
            else
                return null;
        }

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            GameType = ParamGameType.DarkSouls1;

            int stringsOffset = bin.ReadInt32();

            if (stringsOffset > bin.Length)
            {
                bin.BigEndian = true;
                bin.Position = 0;
                stringsOffset = bin.ReadInt32();
                if (stringsOffset > bin.Length)
                {
                    throw new Exception("Strings offset is longer than file length on both little endian and big endian.");
                }
            }

            IsBigEndian = bin.BigEndian;

            long firstRowDataOffset = -1;
            ushort rowCount = 0;

            int gameCheck = bin.ReadInt16();
            if (gameCheck == 0)
            {
                GameType = ParamGameType.Bloodborne;
            }
            else
            {
                //Undo the read of that int16
                bin.Position -= 2;
            }

            if (GameType == ParamGameType.DarkSouls1)
            {
                IsDarkSoulsRemastered = ((FilePath ?? VirtualUri).ToUpper().Contains("INTERROOT_X64"));

                firstRowDataOffset = bin.ReadUInt16();
                UNK1 = bin.AssertInt16(0, 1, 2);
                UNK2 = bin.AssertInt16(1, 2, 3, 4);
            }
            else if (GameType == ParamGameType.Bloodborne)
            {
                bin.Position = 4;
                bin.AssertInt16(0);
                BB_Unknown1 = bin.ReadInt16();
                BB_Unknown2 = bin.ReadInt16();
            }

            rowCount = bin.ReadUInt16();


            //byte namePad = 0;
            ID = bin.ReadPaddedStringShiftJIS(0x20, padding: null);

            ParamFormatVersion = bin.ReadInt32();

            if (GameType == ParamGameType.Bloodborne)
            {
                firstRowDataOffset = bin.ReadInt64();
                bin.AssertInt64(0);
            }

            var nameOffsetList = new List<long>();
            var dataOffsetList = new List<long>();

            EntrySize = 0;// (int)((stringsOffset - firstRowDataOffset) / rowCount);

            if (rowCount == 1)
            {
                EntrySize = stringsOffset - (int)bin.Position;
            }

            Entries = new ObservableCollection<ParamRow>();

            for (int i = 0; i < rowCount; i++)
            {
                prog?.Report((i, rowCount * 3));

                var newEntry = new ParamRow();

                if (GameType == ParamGameType.DarkSouls1)
                {
                    newEntry.ID = bin.ReadInt32();
                    dataOffsetList.Add(bin.ReadUInt32());
                    nameOffsetList.Add(bin.ReadUInt32());
                }
                else if (GameType == ParamGameType.Bloodborne)
                {
                    newEntry.ID = bin.ReadInt64();
                    dataOffsetList.Add(bin.ReadInt64());
                    nameOffsetList.Add(bin.ReadInt64());
                }

                Entries.Add(newEntry);

                if (i > 0 && EntrySize == 0)
                {
                    EntrySize = (int)(dataOffsetList[i] - dataOffsetList[i - 1]);
                }
            }

            for (int i = 0; i < rowCount; i++)
            {
                prog?.Report((i + rowCount, rowCount * 3));

                bin.Position = dataOffsetList[i];
                Entries[i].RawData = bin.ReadBytes(EntrySize);
            }

            for (int i = 0; i < rowCount; i++)
            {
                prog?.Report((i + (rowCount * 2), rowCount * 3));

                bin.Position = nameOffsetList[i];

                if (GameType == ParamGameType.Bloodborne && ParamFormatVersion == 0x00070400)
                {
                    Entries[i].Name = bin.ReadStringUnicode();
                }
                else
                {
                    Entries[i].Name = bin.ReadStringShiftJIS();
                }
            }

            bin.Position = bin.Length;
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            bin.BigEndian = IsBigEndian;

            if (AppliedPARAMDEF != null)
            {
                foreach (var e in Entries)
                {
                    e.ReInitRawData(this);
                    e.SaveValuesToRawData(this);
                }
            }

            if (GameType == ParamGameType.DarkSouls1)
            {
                bin.Placeholder32("StringsOffsetDS1");
                bin.Placeholder16("DataStartOffsetDS1");

                bin.Write(UNK1);
                bin.Write(UNK2);
                
            }
            else if (GameType == ParamGameType.Bloodborne)
            {
                bin.Placeholder32("StringsOffsetBB");
                bin.Write((ushort)0);
                bin.Write(BB_Unknown1);
                bin.Write(BB_Unknown2);
            }

            bin.Write((ushort)Entries.Count);
            bin.WritePaddedStringShiftJIS(ID, 0x20, null);
            bin.Write(ParamFormatVersion);

            if (GameType == ParamGameType.Bloodborne)
            {
                bin.Placeholder64("DataStartOffsetBB");
                bin.Write((long)0);
            }

            var OFF_RowHeaders = bin.Position;

            //SKIP row headers and go right to data.
            //Row headers are filled in last because offsets.
            bin.Position += (Entries.Count * (GameType == ParamGameType.DarkSouls1 ? 0xC : 0x18));

            var dataOffsets = new List<long>();

            //var entrySize = AppliedPARAMDEF.CalculateEntrySize();

            for (int i = 0; i < Entries.Count; i++)
            {
                prog?.Report((i, Entries.Count * 3));

                dataOffsets.Add(bin.Position);
                bin.Write(Entries[i].RawData, EntrySize);
            }

            var nameOffsets = new List<long>();

            for (int i = 0; i < Entries.Count; i++)
            {
                prog?.Report((Entries.Count + i, Entries.Count * 3));

                nameOffsets.Add(bin.Position);
                if (GameType == ParamGameType.Bloodborne && ParamFormatVersion == 0x00070400)
                    bin.WriteStringUnicode(Entries[i].Name, terminate: true);
                else
                    bin.WriteStringShiftJIS(Entries[i].Name, terminate: true);
            }

            // Fill in the first 2 offsets in the file real quick:

            bin.Position = 0;

            if (GameType == ParamGameType.DarkSouls1)
            {
                bin.Replace32("StringsOffsetDS1", (int)nameOffsets[0]);
                bin.Replace16("DataStartOffsetDS1", (ushort)dataOffsets[0]);
            }
            else if (GameType == ParamGameType.Bloodborne)
            {
                bin.Replace32("StringsOffsetBB", (int)nameOffsets[0]);
                bin.Replace64("DataStartOffsetBB", dataOffsets[0]);
            }

            // Finally fill in the row headers:

            bin.Position = OFF_RowHeaders;

            for (int i = 0; i < Entries.Count; i++)
            {
                prog?.Report((i + (Entries.Count * 2), Entries.Count * 3));

                if (GameType == ParamGameType.DarkSouls1)
                {
                    bin.Write((int)Entries[i].ID);
                    bin.Write((int)dataOffsets[i]);
                    bin.Write((int)nameOffsets[i]);
                }
                else if (GameType == ParamGameType.Bloodborne)
                {
                    bin.Write((long)Entries[i].ID);
                    bin.Write((long)dataOffsets[i]);
                    bin.Write((long)nameOffsets[i]);
                }
            }

            bin.Position = bin.Length;

            //bin.Pad(0x14);
        }

        public int IndexOf(ParamRow item)
        {
            return ((IList<ParamRow>)Entries).IndexOf(item);
        }

        public void Insert(int index, ParamRow item)
        {
            ((IList<ParamRow>)Entries).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<ParamRow>)Entries).RemoveAt(index);
        }

        public ParamRow this[int index] { get => ((IList<ParamRow>)Entries)[index]; set => ((IList<ParamRow>)Entries)[index] = value; }

        public void Add(ParamRow item)
        {
            ((IList<ParamRow>)Entries).Add(item);
        }

        public void Clear()
        {
            ((IList<ParamRow>)Entries).Clear();
        }

        public bool Contains(ParamRow item)
        {
            return ((IList<ParamRow>)Entries).Contains(item);
        }

        public void CopyTo(ParamRow[] array, int arrayIndex)
        {
            ((IList<ParamRow>)Entries).CopyTo(array, arrayIndex);
        }

        public bool Remove(ParamRow item)
        {
            return ((IList<ParamRow>)Entries).Remove(item);
        }

        public int Count => ((IList<ParamRow>)Entries).Count;

        public bool IsReadOnly => ((IList<ParamRow>)Entries).IsReadOnly;

        public IEnumerator<ParamRow> GetEnumerator()
        {
            return ((IList<ParamRow>)Entries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<ParamRow>)Entries).GetEnumerator();
        }
    }
}
