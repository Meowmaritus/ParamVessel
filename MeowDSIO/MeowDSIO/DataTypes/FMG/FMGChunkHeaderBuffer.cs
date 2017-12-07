using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.FMG
{
    public struct FMGChunkHeaderBuffer
    {
        public int StringOffsetsBeginOffset;

        public int FirstStringIndex;
        public int FirstStringID;
        public int LastStringID;

        private int count;
        private int[] buffer;

        public FMGChunkHeaderBuffer(int stringOffsetsBeginOffset)
        {
            StringOffsetsBeginOffset = stringOffsetsBeginOffset;
            FirstStringIndex = -1;
            FirstStringID = -1;
            LastStringID = -1;

            count = 0;
            buffer = new int[64];
        }

        public void ReadEntries(DSBinaryReader bin, ObservableCollection<FMGEntryRef> entries)
        {
            count = (LastStringID - FirstStringID) + 1;

            if (count > buffer.Length)
            {
                Array.Resize(ref buffer, count * 2 /*Extra "wiggle room"*/);
            }

            bin.StepIn(StringOffsetsBeginOffset + (FirstStringIndex * 4));
            {
                for (int i = 0; i < count; i++)
                {
                    buffer[i] = bin.ReadInt32();
                }

                for (int i = 0; i < count; i++)
                {
                    if (buffer[i] > 0)
                    {
                        bin.Position = buffer[i];
                        entries.Add(new FMGEntryRef(FirstStringID + i, bin.ReadStringUnicode(length: null)));
                    }
                    else
                    {
                        entries.Add(new FMGEntryRef(FirstStringID + i, null));
                    }
                }
            }
            bin.StepOut();
        }
    }
}
