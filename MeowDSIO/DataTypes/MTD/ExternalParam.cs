using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MTD
{
    public class ExternalParam : Data
    {
        public int UnknownA01 { get; set; }
        public int UnknownA02 { get; set; }
        public int UnknownA03 { get; set; }
        public int UnknownA04 { get; set; }
        public string Name { get; set; }
        public int UnknownB { get; set; }
        public int ShaderDataIndex { get; set; }

        public static ExternalParam Read(DSBinaryReader bin)
        {
            var p = new ExternalParam();

            p.UnknownA01 = bin.ReadInt32();
            p.UnknownA02 = bin.ReadInt32();
            p.UnknownA03 = bin.ReadInt32();
            p.UnknownA04 = bin.ReadInt32();


            bin.ReadDelimiter();


            p.Name = bin.ReadMtdName();
            p.UnknownB = bin.ReadInt32();


            bin.ReadDelimiter();


            p.ShaderDataIndex = bin.ReadInt32();

            return p;
        }

        public static void Write(DSBinaryWriter bin, ExternalParam p)
        {

            bin.Write(p.UnknownA01);
            bin.Write(p.UnknownA02);
            bin.Write(p.UnknownA03);
            bin.Write(p.UnknownA04);


            bin.WriteDelimiter(0xA3);


            bin.WriteMtdName(p.Name, 0x35);
            bin.Write(p.UnknownB);


            bin.WriteDelimiter(0x35);


            bin.Write(p.ShaderDataIndex);
        }

    }
}
