using MeowDSIO.DataTypes.FMG;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MeowDSIO.DataFiles
{
    public class FMG : DataFile, IList<FMGEntryRef>
    {
        public FMGHeader Header { get; set; } = new FMGHeader();
        public ObservableCollection<FMGEntryRef> Entries { get; set; } = new ObservableCollection<FMGEntryRef>();

        private List<FMGChunk> CalculateChunks()
        {
            var chunks = new List<FMGChunk>();

            int startIndex = -1;
            int startID = -1;

            for (int i = 0; i < Entries.Count; i++)
            {
                if (startIndex < 0)
                {
                    startIndex = i;
                    startID = Entries[i].ID;
                    continue;
                }
                else if ((Entries[i].ID - Entries[i - 1].ID) > 1)
                {
                    chunks.Add(new FMGChunk(startIndex, startID, Entries[i - 1].ID));
                    startIndex = i;
                    startID = Entries[i].ID;
                }

            }

            // If there's an unfinished chunk, finish it
            if (startIndex > chunks[chunks.Count - 1].StartIndex)
            {
                chunks.Add(new FMGChunk(startIndex, startID, Entries[Entries.Count - 1].ID));
            }

            return chunks;
        }

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            //UniEscapeChar
            bin.ReadUInt16();

            Header.UnkFlag01 = bin.ReadByte();
            Header.UnkFlag02 = bin.ReadByte();

            //FileSize
            bin.ReadInt32();

            Header.UnkFlag03 = bin.ReadByte();
            Header.IsBigEndian = (bin.ReadByte() == FMGHeader.ENDIAN_FLAG_BIG);
            Header.UnkFlag04 = bin.ReadByte();
            Header.UnkFlag05 = bin.ReadByte();

            int chunkCount = bin.ReadInt32();
            int stringCount = bin.ReadInt32();
            int stringOffsetsBegin = bin.ReadInt32();

            //Pad
            bin.ReadUInt32();

            Entries.Clear();
            FMGChunkHeaderBuffer chunk = new FMGChunkHeaderBuffer(stringOffsetsBegin);
            for (int i = 0; i < chunkCount; i++)
            {
                chunk.FirstStringIndex = bin.ReadInt32();
                chunk.FirstStringID = bin.ReadInt32();
                chunk.LastStringID = bin.ReadInt32();

                chunk.ReadEntries(bin, Entries);
            }
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            var Chunks = CalculateChunks();

            bin.BigEndian = Header.IsBigEndian;

            bin.Write((ushort)0);
            bin.Write(Header.UnkFlag01);
            bin.Write(Header.UnkFlag02);

            bin.Placeholder("FileSize");

            bin.Write(Header.UnkFlag03);

            if (Header.IsBigEndian)
                bin.Write(FMGHeader.ENDIAN_FLAG_BIG);
            else
                bin.Write(FMGHeader.ENDIAN_FLAG_LITTLE);

            bin.Write(Header.UnkFlag04);
            bin.Write(Header.UnkFlag05);

            bin.Write(Chunks.Count);

            bin.Write(Entries.Count);

            bin.Placeholder("StringsBeginPointer");

            bin.Write(0); //PAD

            bin.Label("ChunksBeginOffset");

            foreach (var chunk in Chunks)
            {
                bin.Write(chunk.StartIndex);
                bin.Write(chunk.StartID);
                bin.Write(chunk.EndID);
            }

            bin.PointToHere("StringsBeginPointer");

            bin.Label("StringsBeginOffset");

            bin.Position += (Entries.Count * 4);

            var stringOffsetList = new List<int>();

            for (int i = 0; i < Entries.Count; i++)
            {
                if (Entries[i].Value != null)
                {
                    stringOffsetList.Add((int)bin.Position);
                    bin.WriteStringUnicode(Entries[i].Value, terminate: true);
                }
                else
                {
                    stringOffsetList.Add(0);
                }
            }

            //At the very end of all the strings, place the file end padding:
            bin.Write((ushort)0); //PAD

            //Since we reached max length, might as well go fill in the file size:
            bin.Replace("FileSize", (int)bin.Length);

            bin.Goto("StringsBeginOffset");

            for (int i = 0; i < stringOffsetList.Count; i++)
            {
                bin.Write(stringOffsetList[i]);
            }

            bin.Position = bin.Length;
        }

        #region IList
        public int IndexOf(FMGEntryRef item)
        {
            return ((IList<FMGEntryRef>)Entries).IndexOf(item);
        }

        public void Insert(int index, FMGEntryRef item)
        {
            ((IList<FMGEntryRef>)Entries).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<FMGEntryRef>)Entries).RemoveAt(index);
        }

        public FMGEntryRef this[int index] { get => ((IList<FMGEntryRef>)Entries)[index]; set => ((IList<FMGEntryRef>)Entries)[index] = value; }

        public void Add(FMGEntryRef item)
        {
            ((IList<FMGEntryRef>)Entries).Add(item);
        }

        public void Clear()
        {
            ((IList<FMGEntryRef>)Entries).Clear();
        }

        public bool Contains(FMGEntryRef item)
        {
            return ((IList<FMGEntryRef>)Entries).Contains(item);
        }

        public void CopyTo(FMGEntryRef[] array, int arrayIndex)
        {
            ((IList<FMGEntryRef>)Entries).CopyTo(array, arrayIndex);
        }

        public bool Remove(FMGEntryRef item)
        {
            return ((IList<FMGEntryRef>)Entries).Remove(item);
        }

        public int Count => ((IList<FMGEntryRef>)Entries).Count;

        public bool IsReadOnly => ((IList<FMGEntryRef>)Entries).IsReadOnly;

        public IEnumerator<FMGEntryRef> GetEnumerator()
        {
            return ((IList<FMGEntryRef>)Entries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<FMGEntryRef>)Entries).GetEnumerator();
        }
        #endregion
    }
}
