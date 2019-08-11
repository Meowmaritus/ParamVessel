using MeowDSIO.DataTypes.BND;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    public class FFXData
    {
        public byte[] FFX { get; set; } = null;
        public TPF Textures { get; set; } = null;
        public FLVER Model { get; set; } = null;
    }

    public class FFXBND : DataFile, IDictionary<int, FFXData>
    {
        public BNDHeader Header { get; set; } = new BNDHeader();

        public bool IsRemaster { get; set; } = false;

        private string DirTPF => $@"N:\FRPG\data\INTERROOT_{(IsRemaster ? "x64" : "win32")}\sfx\tex";
        private string DirFLVER => $@"N:\FRPG\data\INTERROOT_{(IsRemaster ? "x64" : "win32")}\sfx\model";
        private string DirFFX => $@"N:\FRPG\data\Sfx\OutputData\Main\{(IsRemaster ? "x64" : "win32")}";

        public const int ID_FFX_START = 0;
        public const int ID_FFX_END = 99999;

        public const int ID_TPF_START = 100000;
        public const int ID_TPF_END = 199999;

        public const int ID_FLVER_START = 200000;
        public const int ID_FLVER_END = 299999;

        public Dictionary<int, FFXData> FFXs { get; set; } = new Dictionary<int, FFXData>();

        private void RegisterTPF(int ffxid, TPF tpf)
        {
            if (!FFXs.ContainsKey(ffxid))
                FFXs.Add(ffxid, new FFXData());

            FFXs[ffxid].Textures = tpf;
        }

        private void RegisterFLVER(int ffxid, FLVER flver)
        {
            if (!FFXs.ContainsKey(ffxid))
                FFXs.Add(ffxid, new FFXData());

            FFXs[ffxid].Model = flver;
        }

        private void RegisterFFX(int ffxid, byte[] ffx)
        {
            if (!FFXs.ContainsKey(ffxid))
                FFXs.Add(ffxid, new FFXData());

            FFXs[ffxid].FFX = ffx;
        }

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            var bnd = bin.ReadAsDataFile<BND>();
            Header = bnd.Header;

            if (bnd.Any(x => x.Name.ToUpper().Contains("_X64")))
                IsRemaster = true;

            foreach (var entry in bnd)
            {
                if (int.TryParse(MiscUtil.GetFileNameWithoutDirectoryOrExtension(entry.Name).Substring(1), out int ffxid))
                {
                    if (entry.ID >= ID_FFX_START && entry.ID <= ID_FFX_END)
                    {
                        RegisterFFX(ffxid, entry.GetBytes());
                    }
                    else if (entry.ID >= ID_TPF_START && entry.ID <= ID_TPF_END)
                    {
                        RegisterTPF(ffxid, entry.ReadDataAs<TPF>());
                    }
                    else if (entry.ID >= ID_FLVER_START && entry.ID <= ID_FLVER_END)
                    {
                        RegisterFLVER(ffxid, entry.ReadDataAs<FLVER>());
                    }
                    else throw new Exception($"FFXBND entry ID out of known ranges: {entry.ID}");
                }
                else throw new Exception($"Unknown FFXBND entry Name: {entry.Name}");
            }
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            var bnd = new BND();
            bnd.Header = Header;

            int curFFX = 0;
            int curTPF = 0;
            int curFLVER = 0;

            foreach (var entry in FFXs)
            {
                if (entry.Value.FFX != null)
                {
                    bnd.Add(new BNDEntry(curFFX++, $@"{DirFFX}\f{entry.Key:D7}.ffx", entry.Value.FFX));
                }

                if (entry.Value.Textures != null)
                {
                    string tpfName = $@"{DirTPF}\s{entry.Key:D5}.tpf";
                    bnd.Add(new BNDEntry(ID_TPF_START + (curTPF++), tpfName, SaveAsBytes(entry.Value.Textures, tpfName)));
                }

                if (entry.Value.Model != null)
                {
                    string flverName = $@"{DirFLVER}\s{entry.Key:D5}.flver";
                    bnd.Add(new BNDEntry(ID_FLVER_START + (curFLVER++), flverName, SaveAsBytes(entry.Value.Model, flverName)));
                }
            }


            bnd.Entries = bnd.Entries.OrderBy(x => x.ID).ToList();

            bin.WriteDataFile(bnd, FilePath ?? VirtualUri);
        }

        #region IDictionary

        public ICollection<int> Keys => ((IDictionary<int, FFXData>)FFXs).Keys;

        public ICollection<FFXData> Values => ((IDictionary<int, FFXData>)FFXs).Values;

        public int Count => ((IDictionary<int, FFXData>)FFXs).Count;

        public bool IsReadOnly => ((IDictionary<int, FFXData>)FFXs).IsReadOnly;

        public FFXData this[int key] { get => ((IDictionary<int, FFXData>)FFXs)[key]; set => ((IDictionary<int, FFXData>)FFXs)[key] = value; }

        public bool ContainsKey(int key)
        {
            return ((IDictionary<int, FFXData>)FFXs).ContainsKey(key);
        }

        public void Add(int key, FFXData value)
        {
            ((IDictionary<int, FFXData>)FFXs).Add(key, value);
        }

        public bool Remove(int key)
        {
            return ((IDictionary<int, FFXData>)FFXs).Remove(key);
        }

        public bool TryGetValue(int key, out FFXData value)
        {
            return ((IDictionary<int, FFXData>)FFXs).TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<int, FFXData> item)
        {
            ((IDictionary<int, FFXData>)FFXs).Add(item);
        }

        public void Clear()
        {
            ((IDictionary<int, FFXData>)FFXs).Clear();
        }

        public bool Contains(KeyValuePair<int, FFXData> item)
        {
            return ((IDictionary<int, FFXData>)FFXs).Contains(item);
        }

        public void CopyTo(KeyValuePair<int, FFXData>[] array, int arrayIndex)
        {
            ((IDictionary<int, FFXData>)FFXs).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<int, FFXData> item)
        {
            return ((IDictionary<int, FFXData>)FFXs).Remove(item);
        }

        public IEnumerator<KeyValuePair<int, FFXData>> GetEnumerator()
        {
            return ((IDictionary<int, FFXData>)FFXs).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<int, FFXData>)FFXs).GetEnumerator();
        }

        #endregion
    }
}
