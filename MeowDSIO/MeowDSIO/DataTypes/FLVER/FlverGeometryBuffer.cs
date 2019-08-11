using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.FLVER
{
    public struct FlverGeometryBufferFaceSet
    {
        public int IndexCount;
        public IndexBuffer IndexBuffer;
        public bool BackfaceCulling;
        public bool IsTriangleStrip;
    }

    public struct FlverGeometryBufferSubmesh
    {
        FlverGeometryBufferFaceSet[] FaceSets;
        VertexPositionColorNormalTangentTexture[] Vertices;

        public string TexName_Diffuse;
        public string TexName_Specular;
        public string TexName_Normal;
    }

    public class FlverGeometryBuffer : DataFile
    {
        public FlverGeometryBufferSubmesh[] Submeshes;

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            
        }
    }
}
