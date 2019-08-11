using MeowDSIO.DataTypes.FLVER;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    public class FLVEROptimized : DataFile
    {
        //public FlverHeader Header { get; set; } = new FlverHeader();
        //public List<FlverDummy> Dummies { get; set; } = new List<FlverDummy>();
        //public List<FlverBone> Bones { get; set; } = new List<FlverBone>();
        public List<FlverSubmesh> Submeshes { get; set; } = new List<FlverSubmesh>();
        public List<FlverVertexStructLayout> VertexStructLayouts { get; set; } = new List<FlverVertexStructLayout>();

        //public List<FlverVertexStructLayout> DEBUG_LIST_VertexStructLayout { get; set; } = new List<FlverVertexStructLayout>();
        //public List<FlverMaterial> DEBUG_LIST_Materials { get; set; } = new List<FlverMaterial>();
        //public List<FlverVertexGroup> DEBUG_LIST_VertexGroups { get; set; } = new List<FlverVertexGroup>();
        //public List<FlverFaceSet> DEBUG_LIST_FaceSets { get; set; } = new List<FlverFaceSet>();

        public static FLVEROptimized ReadFromBnd(string bndPath, int modelIndex)
        {
            var bnd = DataFile.LoadFromFile<BND>(bndPath);
            foreach (var entry in bnd)
            {
                if (entry.ID == EntityBND.ID_FLVER_START + modelIndex)
                {
                    return entry.ReadDataAs<FLVEROptimized>();
                }
            }
            return null;
        }

        public static List<string> ReadTextureNamesFromBnd(string bndPath, int modelIndex)
        {
            List<string> texNames = new List<string>();
            var bnd = DataFile.LoadFromFile<BND>(bndPath);
            foreach (var entry in bnd)
            {
                if (entry.ID == EntityBND.ID_TPF_START + modelIndex)
                {
                    var tpf = entry.ReadDataAs<TPF>();
                    foreach (var t in tpf)
                        texNames.Add(MiscUtil.GetFileNameWithoutDirectoryOrExtension(t.Name));
                    break;
                }
            }

            return texNames;
        }

        public static Dictionary<string, byte[]> ReadTextureDataFromBnd(string bndPath, int modelIndex)
        {
            var data = new Dictionary<string, byte[]>();
            var bnd = DataFile.LoadFromFile<BND>(bndPath);
            foreach (var entry in bnd)
            {
                if (entry.ID == EntityBND.ID_TPF_START + modelIndex)
                {
                    var tpf = entry.ReadDataAs<TPF>();
                    foreach (var t in tpf)
                        data.Add(t.Name, t.DDSBytes);
                    break;
                }
            }
            return data;
        }

        public static readonly byte[] FlverNullableVector3_NullBytes_A = new byte[]
        {
            0xFF, 0xFF, 0x7F, 0x7F,
            0xFF, 0xFF, 0x7F, 0x7F,
            0xFF, 0xFF, 0x7F, 0x7F,
        };

        public static readonly byte[] FlverNullableVector3_NullBytes_B = new byte[]
        {
            0xFF, 0xFF, 0x7F, 0xFF,
            0xFF, 0xFF, 0x7F, 0xFF,
            0xFF, 0xFF, 0x7F, 0xFF,
        };

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            bin.Position += 12;

            int dataOffset = bin.ReadInt32();

            bin.Position += 4;

            int dummyCount = bin.ReadInt32();
            int materialCount = bin.ReadInt32();
            int boneCount = bin.ReadInt32();
            int meshCount = bin.ReadInt32();
            int vertexGroupCount = bin.ReadInt32();

            //Skip bounding box and some unknown stuff
            bin.Position += 40;

            int faceSetCount = bin.ReadInt32();
            int vertexStructLayoutCount = bin.ReadInt32();
            int materialParameterCount = bin.ReadInt32();

            // Skip some unknowns.
            bin.Position += (9 * 4);

            // Skip Dummies
            bin.Position += 64 * dummyCount;

            var INFO_material = new List<(int ParamCount, int ParamStartIndex)>();
            var LIST_material = new List<FlverMaterial>();

            for (int i = 0; i < materialCount; i++)
            {
                var mat = new FlverMaterial();

                var nameOffset = bin.ReadInt32();
                var mtdNameOffset = bin.ReadInt32();

                bin.StepIn(nameOffset);
                {
                    mat.Name = bin.ReadStringUnicode();
                }
                bin.StepOut();

                bin.StepIn(mtdNameOffset);
                {
                    mat.MTDName = bin.ReadStringUnicode();
                }
                bin.StepOut();

                int paramCount = bin.ReadInt32();
                int paramStartIndex = bin.ReadInt32();

                INFO_material.Add((paramCount, paramStartIndex));

                mat.Flags = bin.ReadInt32();
                mat.UnknownInt1 = bin.ReadInt32();
                mat.UnknownInt2 = bin.ReadInt32();
                mat.UnknownInt3 = bin.ReadInt32();

                LIST_material.Add(mat);
            }

            // Skip Bones
            bin.Position += (128 * boneCount);

            var LOC_mesh = new List<(int Start, int BoneIndicesStartOffset)>();

            var INFO_mesh = new List<(int MaterialIndex, List<int> FaceSetIndices, List<int> VertexGroupIndices)>();

            Submeshes = new List<FlverSubmesh>();
            for (int i = 0; i < meshCount; i++)
            {
                LOC_mesh.Add(((int)bin.Position, -1));

                bin.Position += 4;

                int materialIndex = bin.ReadInt32();

                bin.Position += 24;

                var faceSetIndices = new List<int>();
                int faceSetIndicesCount = bin.ReadInt32();
                int faceSetIndicesOffset = bin.ReadInt32();
                bin.StepIn(faceSetIndicesOffset);
                {
                    for (int j = 0; j < faceSetIndicesCount; j++)
                    {
                        faceSetIndices.Add(bin.ReadInt32());
                    }
                }
                bin.StepOut();

                var vertexGroupIndices = new List<int>();
                int vertexGroupIndicesCount = bin.ReadInt32();
                int vertexGroupIndicesOffset = bin.ReadInt32();
                bin.StepIn(vertexGroupIndicesOffset);
                {
                    for (int j = 0; j < vertexGroupIndicesCount; j++)
                    {
                        vertexGroupIndices.Add(bin.ReadInt32());
                    }
                }
                bin.StepOut();

                INFO_mesh.Add((materialIndex, faceSetIndices, vertexGroupIndices));

                // TODO: Check this lol
                Submeshes.Add(new FlverSubmesh(new FLVER()));
            }

            var LIST_faceSets = new List<FlverFaceSet>();

            for (int i = 0; i < faceSetCount; i++)
            {
                var faceset = new FlverFaceSet();

                faceset.Flags = (FlverFaceSetFlags)bin.ReadUInt32();
                faceset.IsTriangleStrip = bin.ReadBoolean();
                faceset.CullBackfaces = bin.ReadBoolean();
                faceset.UnknownByte1 = bin.ReadByte();
                faceset.UnknownByte2 = bin.ReadByte();
                int indexCount = bin.ReadInt32();
                int indexBufferOffset = bin.ReadInt32();
                int indexBufferSize = bin.ReadInt32();

                bin.StepIn(indexBufferOffset + dataOffset);
                {
                    for (int j = 0; j < indexCount; j++)
                    {
                        faceset.VertexIndices.Add(bin.ReadUInt16());
                    }
                }
                bin.StepOut();

                faceset.UnknownInt1 = bin.ReadInt32();
                faceset.UnknownInt2 = bin.ReadInt32();
                faceset.UnknownInt3 = bin.ReadInt32();

                LIST_faceSets.Add(faceset);
            }

            var LIST_vertexGroups = new List<FlverVertexGroup>();

            var INFO_vertexGroup = new List<(int VertexBufferSize, int VertexBufferOffset)>();

            for (int i = 0; i < vertexGroupCount; i++)
            {
                var vertexGroup = new FlverVertexGroup(null);

                vertexGroup.UnknownInt1 = bin.ReadInt32();

                vertexGroup.VertexStructLayoutIndex = bin.ReadInt32();
                vertexGroup.VertexSize = bin.ReadInt32();

                vertexGroup.VertexCount = bin.ReadInt32();

                vertexGroup.UnknownInt2 = bin.ReadInt32();
                vertexGroup.UnknownInt3 = bin.ReadInt32();

                int vertexBufferSize = bin.ReadInt32();
                int vertexBufferOffset = bin.ReadInt32();

                INFO_vertexGroup.Add((vertexBufferSize, vertexBufferOffset));

                LIST_vertexGroups.Add(vertexGroup);
            }

            VertexStructLayouts = new List<FlverVertexStructLayout>();

            for (int i = 0; i < vertexStructLayoutCount; i++)
            {
                var vsl = new FlverVertexStructLayout();

                int memberCount = bin.ReadInt32();
                vsl.Unknown1 = bin.ReadInt32();
                vsl.Unknown2 = bin.ReadInt32();

                int memberOffset = bin.ReadInt32();

                bin.StepIn(memberOffset);
                {
                    for (int j = 0; j < memberCount; j++)
                    {
                        var vslMember = new FlverVertexStructMember();
                        vslMember.Unknown1 = bin.ReadInt32();
                        vslMember.StructOffset = bin.ReadInt32();
                        vslMember.ValueType = (FlverVertexStructMemberValueType)bin.ReadInt32();
                        vslMember.Semantic = (FlverVertexStructMemberSemantic)bin.ReadInt32();
                        vslMember.Index = bin.ReadInt32();
                        vsl.Members.Add(vslMember);
                    }
                }
                bin.StepOut();

                VertexStructLayouts.Add(vsl);
            }

            for (int i = 0; i < meshCount; i++)
            {
                foreach (var vertexGroupIndex in INFO_mesh[i].VertexGroupIndices)
                {
                    LIST_vertexGroups[vertexGroupIndex].ContainingSubmesh = Submeshes[i];
                    Submeshes[i].VertexGroups.Add(LIST_vertexGroups[vertexGroupIndex]);
                }
            }

            var LIST_materialParams = new List<FlverMaterialParameter>();

            for (int i = 0; i < materialParameterCount; i++)
            {
                var mp = new FlverMaterialParameter();

                int valueOffset = bin.ReadInt32();
                bin.StepIn(valueOffset);
                {
                    mp.Value = bin.ReadStringUnicode();
                }
                bin.StepOut();

                int nameOffset = bin.ReadInt32();
                bin.StepIn(nameOffset);
                {
                    mp.Name = bin.ReadStringUnicode();
                }
                bin.StepOut();

                mp.UnknownFloat1 = bin.ReadSingle();
                mp.UnknownFloat2 = bin.ReadSingle();

                mp.UnknownByte1 = bin.ReadByte();
                mp.UnknownByte2 = bin.ReadByte();
                mp.UnknownByte3 = bin.ReadByte();
                mp.UnknownByte4 = bin.ReadByte();

                mp.UnknownInt1 = bin.ReadInt32();
                mp.UnknownInt2 = bin.ReadInt32();
                mp.UnknownInt3 = bin.ReadInt32();

                LIST_materialParams.Add(mp);
            }

            for (int i = 0; i < materialCount; i++)
            {
                for (int j = INFO_material[i].ParamStartIndex;
                    j < (INFO_material[i].ParamStartIndex + INFO_material[i].ParamCount); j++)
                {
                    LIST_material[i].Parameters.Add(LIST_materialParams[j]);
                }
            }

            foreach (var mesh in Submeshes)
            {
                foreach (var vertexGroup in mesh.VertexGroups)
                {
                    var vertexGroupInfo = INFO_vertexGroup[LIST_vertexGroups.IndexOf(vertexGroup)];

                    var vertexStructLayout =
                         VertexStructLayouts[vertexGroup.VertexStructLayoutIndex];

                    var verticesStartOffset = vertexGroupInfo.VertexBufferOffset + dataOffset;

                    bin.StepIn(verticesStartOffset);
                    {
                        for (int j = 0; j < vertexGroup.VertexCount; j++)
                        {
                            int currentStuctStartOffset = verticesStartOffset + j * vertexGroup.VertexSize;
                            bin.StepIn(currentStuctStartOffset);
                            {
                                var vert = new FlverVertex();

                                foreach (var member in vertexStructLayout.Members)
                                {
                                    bin.StepIn(currentStuctStartOffset + member.StructOffset);
                                    {
                                        switch (member.ValueType)
                                        {
                                            case FlverVertexStructMemberValueType.BoneIndicesStruct:
                                                switch (member.Semantic)
                                                {
                                                    case FlverVertexStructMemberSemantic.BoneIndices:
                                                        vert.BoneIndices = new FlverBoneIndices(
                                                            vertexGroup.ContainingSubmesh,
                                                            bin.ReadSByte(),
                                                            bin.ReadSByte(),
                                                            bin.ReadSByte(),
                                                            bin.ReadSByte());
                                                        break;
                                                    default:
                                                        throw new Exception($"Invalid FLVER Vertex Struct Member " +
                                                            $"Semantic ({member.Semantic}) " +
                                                            $"for Value Type {member.ValueType}.");
                                                }
                                                break;
                                            case FlverVertexStructMemberValueType.BoneWeightsStruct:
                                                switch (member.Semantic)
                                                {
                                                    case FlverVertexStructMemberSemantic.BoneWeights:
                                                        vert.BoneWeights = new FlverBoneWeights(
                                                            bin.ReadInt16(),
                                                            bin.ReadInt16(),
                                                            bin.ReadInt16(),
                                                            bin.ReadInt16());
                                                        break;
                                                    default:
                                                        throw new Exception($"Invalid FLVER Vertex Struct Member " +
                                                            $"Semantic ({member.Semantic}) " +
                                                            $"for Value Type {member.ValueType}.");
                                                }
                                                break;
                                            case FlverVertexStructMemberValueType.UV:
                                                switch (member.Semantic)
                                                {
                                                    case FlverVertexStructMemberSemantic.UV:
                                                        vert.UVs.Add(bin.ReadFlverUV());
                                                        break;
                                                    default:
                                                        throw new Exception($"Invalid FLVER Vertex Struct Member " +
                                                            $"Semantic ({member.Semantic}) " +
                                                            $"for Value Type {member.ValueType}.");
                                                }
                                                break;

                                            case FlverVertexStructMemberValueType.UVPair:
                                                switch (member.Semantic)
                                                {
                                                    case FlverVertexStructMemberSemantic.UV:
                                                        vert.UVs.Add(bin.ReadFlverUV());
                                                        vert.UVs.Add(bin.ReadFlverUV());
                                                        break;
                                                    default:
                                                        throw new Exception($"Invalid FLVER Vertex Struct Member " +
                                                            $"Semantic ({member.Semantic}) " +
                                                            $"for Value Type {member.ValueType}.");
                                                }
                                                break;

                                            case FlverVertexStructMemberValueType.Vector3:
                                                var value_Vector3 = bin.ReadVector3();

                                                switch (member.Semantic)
                                                {
                                                    case FlverVertexStructMemberSemantic.Position:
                                                        vert.Position = value_Vector3;
                                                        break;
                                                    default:
                                                        throw new Exception($"Invalid FLVER Vertex Struct Member " +
                                                            $"Semantic ({member.Semantic}) " +
                                                            $"for Value Type {member.ValueType}.");
                                                }
                                                break;
                                            case FlverVertexStructMemberValueType.PackedVector4:
                                                switch (member.Semantic)
                                                {
                                                    case FlverVertexStructMemberSemantic.Normal:
                                                        vert.Normal = bin.ReadFlverPackedVector4();
                                                        break;
                                                    case FlverVertexStructMemberSemantic.BiTangent:
                                                        vert.BiTangent = bin.ReadFlverPackedVector4();
                                                        break;
                                                    case FlverVertexStructMemberSemantic.VertexColor:
                                                        vert.VertexColor = bin.ReadFlverVertexColor();
                                                        break;
                                                    case FlverVertexStructMemberSemantic.UnknownVector4A:
                                                        vert.UnknownVector4A = bin.ReadFlverPackedVector4();
                                                        break;
                                                    default:
                                                        throw new Exception($"Invalid FLVER Vertex Struct Member " +
                                                            $"Semantic ({member.Semantic}) " +
                                                            $"for Value Type {member.ValueType}.");
                                                }
                                                break;
                                            default:
                                                throw new Exception($"Invalid FLVER Value Type: {member.ValueType}");

                                        }
                                    }
                                    bin.StepOut();
                                }

                                vertexGroup.ContainingSubmesh.Vertices.Add(vert);
                            }
                            bin.StepOut();


                        }
                    }
                    bin.StepOut();
                }




            }

            for (int i = 0; i < meshCount; i++)
            {
                Submeshes[i].Material = LIST_material[INFO_mesh[i].MaterialIndex];

                foreach (var faceSetIndex in INFO_mesh[i].FaceSetIndices)
                {
                    Submeshes[i].FaceSets.Add(LIST_faceSets[faceSetIndex]);
                }
            }

            //DEBUG_LIST_VertexStructLayout = LIST_vertexStructLayouts;
            //DEBUG_LIST_Materials = LIST_material;
            //DEBUG_LIST_VertexGroups = LIST_vertexGroups;
            //DEBUG_LIST_FaceSets = LIST_faceSets;
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            throw new InvalidOperationException("This is only here for performance " +
                "when reading models. Information is lost upon loading. Resaving is not possible.");
        }
    }
}
