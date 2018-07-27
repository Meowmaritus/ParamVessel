using MeowDSIO.DataTypes.BND;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    public class ANIBND : DataFile
    {
        public BNDHeader Header { get; set; } = new BNDHeader();

        public bool IsRemaster { get; set; } = false;

        public string ChrModelName { get; set; } = null;

        public string DirChr => $@"N:\FRPG\data\Model\chr\{ChrModelName}";
        public string DirHkx => $@"{DirChr}\{(IsRemaster ? "hkxx64" : "hkxwin32")}";
        public string DirTae => $@"{DirChr}\taeNew\{(IsRemaster ? "x64" : "win32")}";

        public Dictionary<int, byte[]> AnimationHKXs { get; set; } = new Dictionary<int, byte[]>();

        public byte[] StandardSkeletonHKX { get; set; } = null;
        public TAE StandardTAE { get; set; } = null;

        public byte[] PlayerSkeletonHKX { get; set; } = null;
        public Dictionary<int, TAE> PlayerTAE { get; set; } = new Dictionary<int, TAE>();

        public bool IsPlayerAnibndFormat { get; set; } = false;
        public bool IsSplitData { get; set; } = false;

        public List<string> AdditionalAnibnd { get; set; } = new List<string>();
        public List<string> AdditionalAnibndLoad { get; set; } = new List<string>();
        public List<string> AdditionalAnibndDelayLoad { get; set; } = new List<string>();

        public static readonly byte[] BYTES_DIV_DATA = new byte[]
        {
            0x82, 0xB1, 0x82, 0xCC, 0x83, 0x74, 0x83, 0x40, 0x83, 0x43, 0x83, 0x8B, 0x82, 0xF0,
            0x34, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x91, 0xE4, 0x82, 0xC9, 0x8C, 0x8B, 0x8D,
            0x87, 0x82, 0xB7, 0x82, 0xE9, 0x82, 0xC6, 0x83, 0x74, 0x83, 0x40, 0x83, 0x43, 0x83,
            0x8B, 0x96, 0xBC, 0x82, 0xCC, 0x95, 0xAA, 0x8A, 0x84, 0x83, 0x66, 0x81, 0x5B, 0x83,
            0x5E, 0x82, 0xAA, 0x92, 0xC7, 0x89, 0xC1, 0x82, 0xB3, 0x82, 0xEA, 0x82, 0xDC, 0x82,
            0xB7, 0x2E, 0x0D, 0x0A,
        };

        public static readonly byte[] BYTES_VER_0001 = new byte[]
        {
            0x82, 0xB1, 0x82, 0xCC, 0x83, 0x74, 0x83, 0x40, 0x83, 0x43, 0x83, 0x8B, 0x82, 0xF0,
            0x39, 0x39, 0x39, 0x39, 0x39, 0x39, 0x39, 0x94, 0xD4, 0x82, 0xC9, 0x8C, 0x8B, 0x8D,
            0x87, 0x82, 0xB7, 0x82, 0xE9, 0x82, 0xC6, 0x90, 0x56, 0x8C, 0x60, 0x8E, 0xAE, 0x83,
            0x66, 0x81, 0x5B, 0x83, 0x5E, 0x82, 0xC6, 0x82, 0xB5, 0x82, 0xC4, 0x88, 0xB5, 0x82,
            0xED, 0x82, 0xEA, 0x82, 0xDC, 0x82, 0xB7, 0x2E, 0x0D, 0x0A,
        };

        public const int ID_STANDARD_ANIM_START = 0;
        public const int ID_STANDARD_ANIM_END = 999999;

        public const int ID_PLAYER_ANIM_START = 0;
        public const int ID_PLAYER_ANIM_END = 3999999;

        public const int ID_SKELETON = 1000000;
        public const int ID_STANDARD_TAE = 3000000;

        public const int ID_PLAYER_SKELETON = 4000000;

        public const int ID_PLAYER_TAE_START = 5000000;
        public const int ID_PLAYER_TAE_END = 5099999;

        public const int ID_ADDITIONAL_ANIBND_START = 6000000;
        public const int ID_ADDITIONAL_ANIBND_END = 6099999;

        public const int ID_ADDITIONAL_ANIBND_LOAD_START = 6100000;
        public const int ID_ADDITIONAL_ANIBND_LOAD_END = 6199999;

        public const int ID_ADDITIONAL_ANIBND_DELAYLOAD_START = 6200000;
        public const int ID_ADDITIONAL_ANIBND_DELAYLOAD_END = 6299999;

        public const int ID_VER_0001 = 9999999;

        public const string FILENAME_LOAD = "load_";
        public const string FILENAME_DELAYLOAD = "delayload_";
        public const string FILENAME_DIV_DATA = "div_data";
        public const string FILENAME_VER_0001 = "ver_0001";

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            var bnd = bin.ReadAsDataFile<BND>();
            Header = bnd.Header;

            IsPlayerAnibndFormat = bnd.Any(x => x.ID == ID_VER_0001);


            foreach (var entry in bnd)
            {
                if (IsPlayerAnibndFormat)
                {
                    if (entry.ID >= ID_PLAYER_ANIM_START && entry.ID <= ID_PLAYER_ANIM_END)
                    {
                        AnimationHKXs.Add(entry.ID - ID_PLAYER_ANIM_START, entry.GetBytes());
                    }
                    else if (entry.ID == ID_PLAYER_SKELETON)
                    {
                        PlayerSkeletonHKX = entry.GetBytes();
                    }
                    else if (entry.ID >= ID_PLAYER_TAE_START && entry.ID <= ID_PLAYER_TAE_END)
                    {
                        PlayerTAE.Add(entry.ID - ID_PLAYER_TAE_START, entry.ReadDataAs<TAE>());
                    }
                }
                else
                {
                    if (entry.ID >= ID_STANDARD_ANIM_START && entry.ID < ID_STANDARD_ANIM_END)
                    {
                        AnimationHKXs.Add(entry.ID - ID_STANDARD_ANIM_START, entry.GetBytes());
                    }
                    else if (entry.ID == ID_SKELETON)
                    {
                        StandardSkeletonHKX = entry.GetBytes();
                    }
                    else if (entry.ID == ID_STANDARD_TAE)
                    {
                        StandardTAE = entry.ReadDataAs<TAE>();
                    }
                }

                if (entry.ID >= ID_ADDITIONAL_ANIBND_START && entry.ID <= ID_ADDITIONAL_ANIBND_END)
                {
                    var anibndName = MiscUtil.GetFileNameWithoutDirectoryOrExtension(entry.Name);
                    if (anibndName == FILENAME_DIV_DATA)
                    {
                        IsSplitData = true;
                    }
                    else
                    {
                        AdditionalAnibnd.Add(anibndName);
                    }
                }
                else if (entry.ID >= ID_ADDITIONAL_ANIBND_LOAD_START && entry.ID <= ID_ADDITIONAL_ANIBND_LOAD_END)
                {
                    AdditionalAnibndLoad.Add(
                        MiscUtil.GetFileNameWithoutDirectoryOrExtension(entry.Name)
                        .Substring(FILENAME_LOAD.Length));
                }
                else if (entry.ID >= ID_ADDITIONAL_ANIBND_DELAYLOAD_START && entry.ID <= ID_ADDITIONAL_ANIBND_DELAYLOAD_END)
                {
                    AdditionalAnibndDelayLoad.Add(
                        MiscUtil.GetFileNameWithoutDirectoryOrExtension(entry.Name)
                        .Substring(FILENAME_DELAYLOAD.Length));
                }

                if (ChrModelName == null)
                {
                    if (entry.Name.StartsWith(@"N:\FRPG\data\Model\chr\"))
                    {
                        string chr = entry.Name.Substring(@"N:\FRPG\data\Model\chr\".Length);
                        ChrModelName = chr.Substring(0, chr.IndexOf('\\'));
                    }
                }
                else
                {
                    if (!IsRemaster && entry.Name.StartsWith($@"N:\FRPG\data\Model\chr\{ChrModelName}\hkxx64\") ||
                    entry.Name.StartsWith($@"N:\FRPG\data\Model\chr\{ChrModelName}\taeNew\x64\"))
                        IsRemaster = true;
                }

                
            }
        }

        private string GetPlayerHkxFileName(int animID)
        {
            int leftNum = 00;
            int rightNum = 0000;

            if (animID >= 01_0000)
            {
                leftNum = animID / 01_0000;
                rightNum = animID % 01_0000;
            }
            else
            {
                rightNum = animID;
            }

            if (leftNum >= 100)
                return $@"{DirHkx}\a{leftNum:D03}\a{leftNum:D03}_{rightNum:D04}.hkx";
            else
                return $@"{DirHkx}\a{leftNum:D02}\a{leftNum:D02}_{rightNum:D04}.hkx";
        }

        private string GetStandardHkxFileName(int animID)
        {
            int leftNum = 00;
            int rightNum = 0000;

            if (animID >= 01_0000)
            {
                leftNum = animID / 01_0000;
                rightNum = animID % 01_0000;
            }
            else
            {
                rightNum = animID;
            }

            if (leftNum >= 100)
                return $@"{DirHkx}\a{leftNum:D03}_{rightNum:D04}.hkx";
            else
                return $@"{DirHkx}\a{leftNum:D02}_{rightNum:D04}.hkx";
        }

        private string GetPlayerTaeFileName(int id)
        {
            if (id >= 100)
                return $@"{DirTae}\a{id:D03}.tae";
            else
                return $@"{DirTae}\a{id:D02}.tae";
        }

        private string GetStandardTaeFileName()
        {
            return $@"{DirTae}\{ChrModelName}.tae";
        }

        private string GetAdditionalAnibndName(string a)
        {
            return $@"{DirHkx}\{a}.txt";
        }

        private string GetAdditionalAnibndLoadName(string a)
        {
            return $@"{DirHkx}\{FILENAME_LOAD}{a}.txt";
        }

        private string GetAdditionalAnibndDelayLoadName(string a)
        {
            return $@"{DirHkx}\{FILENAME_DELAYLOAD}{a}.txt";
        }

        private string GetDivDataName()
        {
            return $@"{DirHkx}\{FILENAME_DIV_DATA}.txt";
        }

        private string GetVer0001Name()
        {
            return $@"{DirHkx}\{FILENAME_VER_0001}.txt";
        }

        private string GetSkeletonFileName()
        {
            return $@"{DirHkx}\Skeleton.hkx";
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            var bnd = new BND();
            bnd.Header = Header;

            if (IsPlayerAnibndFormat)
            {
                if (AnimationHKXs != null)
                {
                    foreach (var anim in AnimationHKXs)
                    {
                        bnd.Add(new BNDEntry(anim.Key + ID_PLAYER_ANIM_START,
                            GetPlayerHkxFileName(anim.Key), null, anim.Value));
                    }
                }

                if (PlayerTAE != null)
                {
                    foreach (var tae in PlayerTAE)
                    {
                        var entry = new BNDEntry(tae.Key + ID_PLAYER_TAE_START,
                            GetPlayerTaeFileName(tae.Key), null, new byte[] { });
                        entry.ReplaceData(tae.Value);
                        bnd.Add(entry);
                    }
                }

                if (PlayerSkeletonHKX != null)
                    bnd.Add(new BNDEntry(ID_PLAYER_SKELETON, GetSkeletonFileName(), null, PlayerSkeletonHKX));
            }
            else
            {
                if (AnimationHKXs != null)
                {
                    foreach (var anim in AnimationHKXs)
                    {
                        bnd.Add(new BNDEntry(anim.Key + ID_STANDARD_ANIM_START,
                            GetStandardHkxFileName(anim.Key), null, anim.Value));
                    }
                }

                if (StandardTAE != null)
                {
                    var taeEntry = new BNDEntry(ID_STANDARD_TAE, GetStandardTaeFileName(), null, new byte[] { });
                    taeEntry.ReplaceData(StandardTAE);
                    bnd.Add(taeEntry);
                }

                if (StandardSkeletonHKX != null)
                    bnd.Add(new BNDEntry(ID_SKELETON, GetSkeletonFileName(), null, StandardSkeletonHKX));
            }

            int currentAnibnd = 0;

            if (IsSplitData)
            {
                bnd.Add(new BNDEntry((currentAnibnd++) + ID_ADDITIONAL_ANIBND_START,
                    GetDivDataName(), null, BYTES_DIV_DATA));
            }

            if (AdditionalAnibnd != null)
            {
                foreach (var a in AdditionalAnibnd)
                {
                    bnd.Add(new BNDEntry((currentAnibnd++) + ID_ADDITIONAL_ANIBND_START,
                        GetAdditionalAnibndName(a), null, BYTES_DIV_DATA));
                }
            }

            if (AdditionalAnibndLoad != null)
            {
                currentAnibnd = 0;
                foreach (var a in AdditionalAnibndLoad)
                {
                    bnd.Add(new BNDEntry((currentAnibnd++) + ID_ADDITIONAL_ANIBND_LOAD_START,
                        GetAdditionalAnibndLoadName(a), null, new byte[] { }));
                }
            }

            if (AdditionalAnibndDelayLoad != null)
            {
                currentAnibnd = 0;
                foreach (var a in AdditionalAnibndDelayLoad)
                {
                    bnd.Add(new BNDEntry((currentAnibnd++) + ID_ADDITIONAL_ANIBND_DELAYLOAD_START,
                        GetAdditionalAnibndDelayLoadName(a), null, new byte[] { }));
                }
            }

            if (IsPlayerAnibndFormat)
            {
                bnd.Add(new BNDEntry(ID_VER_0001, GetVer0001Name(), null, BYTES_VER_0001));
            }

            bnd.Entries = bnd.Entries.OrderBy(x => x.ID).ToList();

            bin.WriteDataFile(bnd, VirtualUri);
        }
    }
}
