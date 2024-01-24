using System.IO;
using System.Numerics;
using System.Windows.Media;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class GeometryStructChunk : Chunk
{
    public float Ambient;
    public float Diffuse;
    public int Flags;
    public bool Has2TextureCoordinates;
    public bool HasNormals;
    public bool HasTextureCoordinates;
    public bool HasVertexColors;
    public bool HasVertexTranslation;
    public bool IsGeometryLit;
    public bool IsNativeGeometry;

    public bool IsTriangleStrip;
    public bool ModulateMaterialColor;
    public int MorphTargetCount;
    public List<MorphTarget> MorphTargets;

    public List<Color> PrelitColors;
    public float Specular;
    public List<Vector2> TextureCoordinates;
    public int TriangleCount;
    public List<Triangle> Triangles;
    public int VertexCount;

    public GeometryStructChunk(GeometryChunk? geometryChunk, ChunkHeader header) : base(
        geometryChunk, header)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine(
            $"GeometryStructChunk.Read: Reading geometry struct chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read flags
        Flags = (int)binaryReader.ReadUInt32();

        {
            // Flag reference:
            // 0x00000001 	rpGEOMETRYTRISTRIP 	Is triangle strip (if disabled it will be an triangle list)
            // 0x00000002 	rpGEOMETRYPOSITIONS 	Vertex translation
            // 0x00000004 	rpGEOMETRYTEXTURED 	Texture coordinates
            // 0x00000008 	rpGEOMETRYPRELIT 	Vertex colors
            // 0x00000010 	rpGEOMETRYNORMALS 	Store normals
            // 0x00000020 	rpGEOMETRYLIGHT 	Geometry is lit (dynamic and static)
            // 0x00000040 	rpGEOMETRYMODULATEMATERIALCOLOR 	Modulate material color
            // 0x00000080 	rpGEOMETRYTEXTURED2 	Texture coordinates 2
            // 0x01000000 	rpGEOMETRYNATIVE 	Native Geometry

            // Decode flags
            IsTriangleStrip = (Flags & 0x00000001) != 0;
            HasVertexTranslation = (Flags & 0x00000002) != 0;
            HasTextureCoordinates = (Flags & 0x00000004) != 0;
            HasVertexColors = (Flags & 0x00000008) != 0;
            HasNormals = (Flags & 0x00000010) != 0;
            IsGeometryLit = (Flags & 0x00000020) != 0;
            ModulateMaterialColor = (Flags & 0x00000040) != 0;
            Has2TextureCoordinates = (Flags & 0x00000080) != 0;
            IsNativeGeometry = (Flags & 0x01000000) != 0;
        }

        // Read triangle count
        TriangleCount = (int)binaryReader.ReadUInt32();

        // Read vertex count
        VertexCount = (int)binaryReader.ReadUInt32();

        // Read morph target count
        MorphTargetCount = (int)binaryReader.ReadUInt32();

        // If the header version is < 0x34000, then we need to read the ambient, specular and diffuse values
        if (LibraryIdUtils.LibraryIdUnpackVersion(Header.Version) < 0x34000)
        {
            // Read ambient
            Ambient = binaryReader.ReadSingle();

            // Read specular
            Specular = binaryReader.ReadSingle();

            // Read diffuse
            Diffuse = binaryReader.ReadSingle();
        }

        // Depending on the flags, we need to read different data
        // We are only going to read the data we need for now,
        // That's when the flags does not binary and with 0x01000000
        if (!IsNativeGeometry)
        {
            // Read prelit colors
            if (HasVertexColors) ReadPrelitColors(binaryReader);

            // Read texture coordinates
            if (HasTextureCoordinates) ReadTextureCoordinates(binaryReader);

            if (Has2TextureCoordinates)
            {
                ReadTextureCoordinates(binaryReader);
                ReadTextureCoordinates(binaryReader);
            }

            // Read triangles
            ReadTriangles(binaryReader);

            // Read morph targets
            ReadMorphTargets(binaryReader);
        }

        // Print debug message
        Console.WriteLine(
            $"GeometryStructChunk.Read: Read geometry struct chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    private void ReadPrelitColors(BinaryReader fileAccess)
    {
        PrelitColors = new List<Color>();

        for (var vertexIndex = 0; vertexIndex < VertexCount; vertexIndex++)
        {
            // Read prelit color

            // Color takes a float, and we read a byte. Convert it to a float (0-1)
            
            var r = fileAccess.ReadByte();
            var g = fileAccess.ReadByte();
            var b = fileAccess.ReadByte();
            var a = fileAccess.ReadByte();
            var prelitColor = Color.FromArgb(a, r, g, b);
            
            // Add prelit color to prelit colors
            PrelitColors.Add(prelitColor);
        }
    }

    private void ReadTextureCoordinates(BinaryReader fileAccess)
    {
        TextureCoordinates = new List<Vector2>();

        for (var vertexIndex = 0; vertexIndex < VertexCount; vertexIndex++)
        {
            // Read texture coordinate

            // Texture coordinate takes two floats
            var textureCoordinate = new Vector2
            {
                X = fileAccess.ReadSingle(),
                Y = fileAccess.ReadSingle()
            };

            // Add texture coordinate to texture coordinates
            TextureCoordinates.Add(textureCoordinate);
        }
    }

    private void ReadTriangles(BinaryReader fileAccess)
    {
        Triangles = new List<Triangle>();

        for (var triangleIndex = 0; triangleIndex < TriangleCount; triangleIndex++)
        {
            // Read triangle

            // Triangle takes three ints
            var triangle = new Triangle
            {
                VertexIndex2 = fileAccess.ReadInt16(), // TODO: Maybe uint?
                VertexIndex1 = fileAccess.ReadInt16(),
                MaterialIndex = fileAccess.ReadInt16(),
                VertexIndex3 = fileAccess.ReadInt16()
            };

            // Add triangle to triangles
            Triangles.Add(triangle);
        }
    }

    private void ReadMorphTargets(BinaryReader fileAccess)
    {
        MorphTargets = new List<MorphTarget>();

        for (var morphTargetIndex = 0; morphTargetIndex < MorphTargetCount; morphTargetIndex++)
        {
            // Read morph target

            // Morph target takes a sphere, two bools and two lists of vectors
            var morphTarget = new MorphTarget();
            morphTarget.BoundingSphere.Radius = fileAccess.ReadSingle();
            morphTarget.BoundingSphere.Position.X = fileAccess.ReadSingle();
            morphTarget.BoundingSphere.Position.Y = fileAccess.ReadSingle();
            morphTarget.BoundingSphere.Position.Z = fileAccess.ReadSingle();
            morphTarget.HasVertices = fileAccess.ReadUInt32() != 0;
            morphTarget.HasNormals = fileAccess.ReadUInt32() != 0;

            // Read vertices
            if (morphTarget.HasVertices)
                for (var vertexIndex = 0; vertexIndex < VertexCount; vertexIndex++)
                {
                    // Read vertex
                    // Vertex takes three floats
                    var vertex = new Vector3
                    {
                        X = fileAccess.ReadSingle(),
                        Y = fileAccess.ReadSingle(),
                        Z = fileAccess.ReadSingle()
                    };

                    // Add vertex to vertices
                    morphTarget.Vertices.Add(vertex);
                }

            // Read normals
            if (morphTarget.HasNormals)
                for (var normalIndex = 0; normalIndex < VertexCount; normalIndex++)
                {
                    // Read normal
                    // Normal takes three floats
                    var normal = new Vector3
                    {
                        X = fileAccess.ReadSingle(),
                        Y = fileAccess.ReadSingle(),
                        Z = fileAccess.ReadSingle()
                    };

                    // Add normal to normals
                    morphTarget.Normals.Add(normal);
                }

            // Add morph target to morph targets
            MorphTargets.Add(morphTarget);
        }
    }

    public class Triangle
    {
        public int MaterialIndex;
        public int VertexIndex1;
        public int VertexIndex2;
        public int VertexIndex3;
    }

    public class Sphere
    {
        public Vector3 Position = new();
        public float Radius;
    }

    public class MorphTarget
    {
        public Sphere BoundingSphere = new();
        public bool HasNormals; // Remember these are bool32 on the file
        public bool HasVertices; // Remember these are bool32 on the file
        public List<Vector3> Normals = new();
        public List<Vector3> Vertices = new();
    }
}