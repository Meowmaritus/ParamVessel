using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.BND3
{
    public class BND3Entry : IDisposable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? Unknown1 = null;
        private byte[] Data;

        public BND3Entry(int ID, string Name, int? Unknown1, byte[] FileBytes)
        {
            this.ID = ID;
            this.Name = Name;
            this.Unknown1 = Unknown1;
            Data = FileBytes;
        }

        public T ReadDataAs<T>(IProgress<(int, int)> prog)
            where T : DataFile, new()
        {
            return DataFile.LoadFromBytes<T>(Data, Name, prog);
        }

        public void ReplaceData<T>(T data, IProgress<(int, int)> prog)
            where T : DataFile, new()
        {
            Data = DataFile.SaveAsBytes(data, Name, prog);
        }

        public int Size => (Data?.Length ?? 0);

        public byte[] GetBytes()
        {
            return Data;
        }

        public void SetBytes(byte[] newBytes)
        {
            Data = newBytes;
        }

        public void Dispose()
        {
            Data = null;
        }
    }
}
