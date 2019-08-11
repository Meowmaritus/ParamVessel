using MeowDSIO.DataTypes.BND;
using MeowDSIO.DataTypes.REMOBND;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    public class REMOBND : DataFile
    {
        public BNDHeader Header { get; set; } = new BNDHeader();

        public Dictionary<int, RemoCutEntry> Cuts { get; set; } = new Dictionary<int, RemoCutEntry>();

        public TAE Tae { get; set; } = null;

        public int ID { get; set; } = 0;

        public bool IsRemaster { get; set; } = false;

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            var bnd = bin.ReadAsDataFile<BND>(FilePath ?? VirtualUri);

            Header = bnd.Header;

            var sortedBndEntries = bnd.Entries.OrderBy(x => x.ID);

            IsRemaster = false;

            Cuts = new Dictionary<int, RemoCutEntry>();

            foreach (var entry in sortedBndEntries)
            {
                var x64Chk = entry.Name.ToUpper();
                if (x64Chk.Contains(@"\X64\") || x64Chk.Contains(@"\HKXX64\"))
                    IsRemaster = true;

                if (entry.ID == 10000)
                {
                    Tae = entry.ReadDataAs<TAE>();
                    ID = int.Parse(System.IO.Path.GetFileNameWithoutExtension(entry.Name).Substring(3));
                }
                else
                {
                    if (entry.ID % 100 == 1)
                    {
                        int cutID = int.Parse(entry.Name.Substring(4, 4));
                        if (!Cuts.ContainsKey(cutID))
                            Cuts.Add(cutID, new RemoCutEntry());
                        Cuts[cutID].Hkx = entry.GetBytes();
                    }
                    else if (entry.ID % 100 == 0)
                    {
                        int cutID = int.Parse(entry.Name.Substring(4, 4));
                        if (!Cuts.ContainsKey(cutID))
                            Cuts.Add(cutID, new RemoCutEntry());
                        Cuts[cutID].Sibcam = entry.GetBytes();
                    }
                }
            }
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            var bnd = new BND()
            {
                Header = Header
            };

            int i = 100;

            foreach (var cut in Cuts)
            {
                bnd.AddEntry(i, $@"\cut{cut.Key:D4}\camera_win32.sibcam", cut.Value.Sibcam);
                bnd.AddEntry(i + 1, $@"\cut{cut.Key:D4}\{(IsRemaster ? "hkxx64" : "hkxwin32")}\a{cut.Key:D4}.hkx", cut.Value.Hkx);

                i += 100;
            }

            bnd.AddEntry(10000, $@"\taeNew\{(IsRemaster ? "x64" : "win32")}\scn{ID:D6}.tae",
                DataFile.SaveAsBytes(Tae, $@"\taeNew\{(IsRemaster ? "x64" : "win32")}\scn{ID:D6}.tae"));

            bin.WriteDataFile(bnd, FilePath ?? VirtualUri);
        }
    }
}
