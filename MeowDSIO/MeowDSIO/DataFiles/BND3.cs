using MeowDSIO.DataTypes.BND3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    public class BND3 : DataFile, IDisposable, IList<BND3Entry>
    {
        public List<BND3Entry> Entries = new List<BND3Entry>();
        public BND3Header Header { get; set; } = new BND3Header();

        public void AddEntry(int id, string name, byte[] data, int? unknown1 = null)
        {
            Entries.Add(new BND3Entry(id, name, unknown1, data));
        }

        public BND3Entry GetFirstEntryWithIDAndName(int id, string name, bool ignoreCase = false)
        {
            try
            {
                return Entries.First(x => x.ID == id && 
                    (ignoreCase ? x.Name.ToUpper() : x.Name) == (ignoreCase ? name.ToUpper() : name));
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public IEnumerable<BND3Entry> GetAllEntriesWithIDAndName(int id, string name, bool ignoreCase = false)
        {
            return Entries.Where(x => x.ID == id && 
                (ignoreCase ? x.Name.ToUpper() : x.Name) == (ignoreCase ? name.ToUpper() : name));
        }

        public BND3Entry GetFirstEntryWithName(string name, bool ignoreCase = false)
        {
            try
            {
                return Entries.First(x => (ignoreCase ? x.Name.ToUpper() : x.Name) == (ignoreCase ? name.ToUpper() : name));
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public IEnumerable<BND3Entry> GetAllEntriesWithName(string name, bool ignoreCase = false)
        {
            return Entries.Where(x => (ignoreCase ? x.Name.ToUpper() : x.Name) == (ignoreCase ? name.ToUpper() : name));
        }

        public BND3Entry GetFirstEntryWithID(int id)
        {
            try
            {
                return Entries.First(x => x.ID == id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
        
        public IEnumerable<BND3Entry> GetAllEntriesWithID(int id)
        {
            return Entries.Where(x => x.ID == id);
        }

        public IEnumerable<BND3Entry> GetAllEntriesWithinIDRange(int? minID, int? maxID)
        {
            return Entries.Where(x => x.ID >= (minID ?? int.MinValue) && x.ID <= (maxID ?? int.MaxValue));
        }


        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            byte[] versionBytes = bin.ReadBytes(BND3Header.Version_ByteLength);
            Header.Version = Encoding.ASCII.GetString(versionBytes);

            if (!Header.Version.StartsWith(BND3Header.Version_RootSig))
            {
                throw new Exception($"Invalid BND3 signature in version string '{Header.Version}'; " +
                    $"it must begin with '{BND3Header.Version_RootSig}' in order to be valid.");
            }

            bin.BigEndian = false;

            Header.Format = bin.ReadByte();
            Header.IsBigEndian_Maybe = bin.ReadByte($"{nameof(Header)}.{nameof(Header.IsBigEndian_Maybe)}", 0, 1) == 1;
            Header.IsPS3_Maybe = bin.ReadByte($"{nameof(Header)}.{nameof(Header.IsPS3_Maybe)}", 0, 1) == 1;
            Header.UnkFlag01 = bin.ReadByte($"{nameof(Header)}.{nameof(Header.UnkFlag01)}", 0);

            bin.BigEndian = Header.IsBigEndian_Maybe;

            int fileCount = bin.ReadInt32();

            bin.ReadInt32(); //Names end offset

            Header.UnknownBytes01 = bin.ReadBytes(BND3Header.UnknownBytes01_Length);

            var e = new BND3EntryHeaderBuffer();

            Entries.Clear();

            int prog_currentbyte = 0;
            int prog_numbytes = (int)bin.Length;

            for (int i = 0; i < fileCount; i++)
            {
                e.Reset();

                e.FileSize = bin.ReadInt32();
                e.FileOffset = bin.ReadInt32();

                if (Header.Format != 0x00)
                {
                    e.FileID = bin.ReadInt32();
                    e.FileNameOffset = bin.ReadInt32();
                }

                if (Header.Format == 0x74 || Header.Format == 0x54 || Header.Format == 0x2E)
                {
                    e.Unknown1 = bin.ReadInt32();
                }

                bin.ReadInt32(); //Entry padding

                Entries.Add(e.GetEntry(bin));

                prog_currentbyte += e.FileSize;
                prog?.Report((prog_currentbyte, prog_numbytes));
            }
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            byte[] versionBytes = Encoding.ASCII.GetBytes(Header.Version);
            bin.Write(versionBytes, BND3Header.Version_ByteLength);

            bin.BigEndian = false;

            bin.Write(Header.Format);
            bin.Write(Header.IsBigEndian_Maybe);
            bin.Write(Header.IsPS3_Maybe);
            bin.Write(Header.UnkFlag01);

            bin.BigEndian = Header.IsBigEndian_Maybe;

            bin.Write(Entries.Count);

            var OFF_NameEndOffset = bin.Position;
            bin.Placeholder(); //Placeholder for name end offset

            bin.Write(Header.UnknownBytes01, BND3Header.UnknownBytes01_Length);

            var OFF_EntryHeaders = bin.Position;

            const int ProgEst_Name = 0x20;
            const int ProgEst_Header = 0x14;

            int prog_cur = 0;
            //Rough estimation of the size of the headers (for both passes)
            int prog_max = Entries.Count * (ProgEst_Header * 2);

            //Rough estimation of name size (only for formats with names)
            if (Header.Format != 0x00)
            {
                prog_max += (Entries.Count + ProgEst_Name);
            }

            //Add the actual data size
            foreach (var e in Entries)
            prog_max += e.Size;

            for (int i = 0; i < Entries.Count; i++)
            {
                bin.Write(Entries[i].Size);
                bin.Placeholder(); //Placeholder for data offset

                if (Header.Format != 0x00)
                {
                    bin.Write(Entries[i].ID);
                    bin.Placeholder(); //Placeholder for name offset
                }

                if (Header.Format == 0x74 || Header.Format == 0x54 || Header.Format == 0x2E)
                {
                    bin.Write(Entries[i].Unknown1 ?? 0);
                }

                if (i < Entries.Count - 1) //Do not include padding after very last entry.
                {
                    if (Header.Format == 0x74 || Header.Format == 0x54)
                    {
                        bin.Write(0x00000040);
                    }
                    else
                    {
                        bin.Write(0x02000000);
                    }
                }

                prog_cur += ProgEst_Header;
                prog?.Report((prog_cur, prog_max));
            }

            var OFF_Names = bin.Position;

            var nameOffsets = new List<int>();
            if (Header.Format != 0x00)
            {
                for (int i = 0; i < Entries.Count; i++)
                {
                    nameOffsets.Add((int)bin.Position);
                    bin.WriteStringShiftJIS(Entries[i].Name, true);

                    prog_cur += ProgEst_Name;
                    prog?.Report((prog_cur, prog_max));
                }
            }

            var OFF_AfterNames = bin.Position;

            bin.Pad(0x10);

            var fileOffsets = new List<int>();
            for (int i = 0; i < Entries.Count; i++)
            {
                fileOffsets.Add((int)bin.Position);
                bin.Write(Entries[i].GetBytes());
                if (i < Entries.Count - 1) //Do not include padding after very last entry.
                {
                    bin.Pad(0x10);
                }
                prog_cur += Entries[i].Size;
                prog?.Report((prog_cur, prog_max));
            }

            bin.Position = OFF_EntryHeaders;
            for (int i = 0; i < Entries.Count; i++)
            {
                bin.Position += 4; //Size
                bin.Write(fileOffsets[i]);

                if (Header.Format != 0x00)
                {
                    bin.Position += 4; //ID
                    bin.Write(nameOffsets[i]);
                }

                if (Header.Format == 0x74 || Header.Format == 0x54 || Header.Format == 0x2E)
                {
                    bin.Position += 4; //Unknown1
                }

                bin.Position += 4; //Padding

                prog_cur += ProgEst_Header;
                prog?.Report((prog_cur, prog_max));
            }

            bin.Position = OFF_NameEndOffset;
            bin.Write((int)OFF_AfterNames);

            bin.Position = bin.Length;
        }

        #region IList

        public int Count => ((IList<BND3Entry>)Entries).Count;
        public bool IsReadOnly => ((IList<BND3Entry>)Entries).IsReadOnly;
        public BND3Entry this[int index] { get => ((IList<BND3Entry>)Entries)[index]; set => ((IList<BND3Entry>)Entries)[index] = value; }

        public int IndexOf(BND3Entry item)
        {
            return ((IList<BND3Entry>)Entries).IndexOf(item);
        }

        public void Insert(int index, BND3Entry item)
        {
            ((IList<BND3Entry>)Entries).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<BND3Entry>)Entries).RemoveAt(index);
        }

        public void Add(BND3Entry item)
        {
            ((IList<BND3Entry>)Entries).Add(item);
        }

        public void Clear()
        {
            ((IList<BND3Entry>)Entries).Clear();
        }

        public bool Contains(BND3Entry item)
        {
            return ((IList<BND3Entry>)Entries).Contains(item);
        }

        public void CopyTo(BND3Entry[] array, int arrayIndex)
        {
            ((IList<BND3Entry>)Entries).CopyTo(array, arrayIndex);
        }

        public bool Remove(BND3Entry item)
        {
            return ((IList<BND3Entry>)Entries).Remove(item);
        }

        public IEnumerator<BND3Entry> GetEnumerator()
        {
            return ((IList<BND3Entry>)Entries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<BND3Entry>)Entries).GetEnumerator();
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            foreach (var e in Entries)
            {
                e?.Dispose();
            }
        }

        
        #endregion
    }
}
