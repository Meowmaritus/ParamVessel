using MeowDSIO.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO
{
    public partial class DSBinaryWriter : BinaryWriter
    {
        internal static Encoding ShiftJISEncoding = Encoding.GetEncoding("shift_jis");

        // Now with 100% less 0DD0ADDE
        public static readonly byte[] PLACEHOLDER_16BIT = new byte[] { 0xDE, 0xAD };
        public static readonly byte[] PLACEHOLDER_32BIT = new byte[] { 0xDE, 0xAD, 0xD0, 0x0D };
        public static readonly byte[] PLACEHOLDER_64BIT = new byte[] { 0xDE, 0xAD, 0xD0, 0x0D, 0xBE, 0xEF, 0xBA, 0xBE };

        public string FileName { get; private set; }

        public long Position { get => BaseStream.Position; set => BaseStream.Position = value; }
        public long Length => BaseStream.Length;
        public void Goto(long absoluteOffset) => BaseStream.Seek(absoluteOffset, SeekOrigin.Begin);
        public void Jump(long relativeOffset) => BaseStream.Seek(relativeOffset, SeekOrigin.Current);

        private Stack<long> StepStack = new Stack<long>();
        private Stack<PaddedRegion> PaddedRegionStack = new Stack<PaddedRegion>();

        private Dictionary<string, long> MarkerDict = new Dictionary<string, long>();

        public bool BigEndian = false;

        public char StrEscapeChar = (char)0;

        private long msbStringStart = -1;

        public void StartMSBStrings()
        {
            msbStringStart = Position;
        }

        public void EndMSBStrings(int blockSize)
        {
            if (msbStringStart < 0)
            {
                throw new DSWriteException(this, $"Tried to call .{nameof(EndMSBStrings)}() without calling .{nameof(StartMSBStrings)}() first.");
            }

            int currentBlockLength = (int)(Position - msbStringStart);

            if (currentBlockLength < blockSize)
            {
                byte[] pad = new byte[blockSize - currentBlockLength];
                Write(pad);
            }

            msbStringStart = -1;
        }

        public void StepInMSB(int offset)
        {
            if (currentMsbStructOffset >= 0)
            {
                StepIn(currentMsbStructOffset + offset);
            }
            else
            {
                throw new DSWriteException(this, $"Attempted to use .{nameof(StepInMSB)}() without running .{nameof(StartMsbStruct)}() first.");
            }
        }

        public void StepIn(long offset)
        {
            StepStack.Push(Position);
            Goto(offset);
        }

        public void StepOut()
        {
            if (StepStack.Count == 0)
                throw new InvalidOperationException("You cannot step out unless StepIn() was previously called on an offset.");

            Goto(StepStack.Pop());
        }

        public void StepIntoPaddedRegion(long length, byte padding)
        {
            PaddedRegionStack.Push(new PaddedRegion(Position, length, padding));
        }

        public void StepOutOfPaddedRegion()
        {
            if (PaddedRegionStack.Count == 0)
                throw new InvalidOperationException("You cannot step out of padded region unless inside of one " +
                    $"as a result of previously calling {nameof(StepIntoPaddedRegion)}().");

            var deepestPaddedRegion = PaddedRegionStack.Pop();
            deepestPaddedRegion.AdvanceWriterToEnd(this);
        }

        public void DoAt(long offset, Action doAction)
        {
            StepIn(offset);
            doAction();
            StepOut();
        }

        public long Placeholder32(string markerName = null, bool allowOverride = true)
        {
            var label = Label(markerName, allowOverride);
            Write(PLACEHOLDER_32BIT);
            return label;
        }

        public long Placeholder64(string markerName = null, bool allowOverride = true)
        {
            var label = Label(markerName, allowOverride);
            Write(PLACEHOLDER_64BIT);
            return label;
        }

        public long Placeholder16(string markerName = null, bool allowOverride = true)
        {
            var label = Label(markerName, allowOverride);
            Write(PLACEHOLDER_16BIT);
            return label;
        }

        public long Label(string markerName = null, bool allowOverride = true)
        {
            var labelOffset = Position;

            if (markerName != null)
            {
                if (MarkerDict.ContainsKey(markerName))
                {
                    if (allowOverride)
                        MarkerDict[markerName] = labelOffset;
                    else
                        throw new DSWriteException(this, $"{nameof(DSBinaryWriter)}.{nameof(Label)} - bool {nameof(allowOverride)} set to FALSE but a Label with the same name has already been added.");
                }
                else
                {
                    MarkerDict.Add(markerName, labelOffset);
                }
                
            }

            return labelOffset;
        }

        public void Goto(string markerName)
        {
            if (markerName == null)
                throw new ArgumentNullException(nameof(markerName));

            if (MarkerDict.ContainsKey(markerName))
            {
                Position = MarkerDict[markerName];
            }
            else
            {
                throw new ArgumentException("No DSBinaryWriter Marker was registered " +
                    $"with the name '{markerName}'.", nameof(markerName));
            }
        }

        public void Replace32(string markerName, int replaceMarkerVal)
        {
            StepIn(MarkerDict[markerName]);
            Write(replaceMarkerVal);
            StepOut();
        }

        public void Replace16(string markerName, ushort replaceMarkerVal)
        {
            StepIn(MarkerDict[markerName]);
            Write(replaceMarkerVal);
            StepOut();
        }

        public void Replace64(string markerName, long replaceMarkerVal)
        {
            StepIn(MarkerDict[markerName]);
            Write(replaceMarkerVal);
            StepOut();
        }

        public void PointToHere(string markerName)
        {
            Replace32(markerName, (int)Position);
        }
    }
}
