using System.IO;
using RWTree.Middleware.RenderWare.Stream.Chunks;

namespace RWTree.Middleware.RenderWare.Stream;

public class BinMeshPlgChunk : Chunk
{
    public uint Flags;
    private List<BinMeshPlgMesh> _meshes;
    public uint NumIndices;
    public uint NumMeshes;

    public BinMeshPlgChunk(Chunk? parent, ChunkHeader header) : base(parent, header)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine($"BinMeshPLGChunk.Read: Reading bin mesh PLG chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read bin mesh PLG chunk data
        Flags = binaryReader.ReadUInt32();
        NumMeshes = binaryReader.ReadUInt32();
        NumIndices = binaryReader.ReadUInt32();

        _meshes = new List<BinMeshPlgMesh>();
        for (var meshIndex = 0; meshIndex < NumMeshes; meshIndex++)
        {
            var mesh = new BinMeshPlgMesh();
            mesh.IndexCount = binaryReader.ReadUInt32();
            mesh.MaterialIndex = binaryReader.ReadUInt32();

            for (var j = 0; j < mesh.IndexCount; j++)
                // If OpenGL, uint16, else uint32
                mesh.Indices.Add(binaryReader.ReadUInt32());

            _meshes.Add(mesh);
        }

        // Print debug message
        Console.WriteLine($"BinMeshPLGChunk.Read: Finished reading bin mesh PLG chunk at position: '{binaryReader.BaseStream.Position}'");
    }

    public static BinMeshPlgChunk ReadBinMeshPlgStruct(BinaryReader fileAccess, ExtensionChunk? parent)
    {
        // Read chunk header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // Create bin mesh PLG chunk
        var chunk = new BinMeshPlgChunk(parent, header);

        // Read bin mesh PLG chunk data
        chunk.Read(fileAccess);

        // Return bin mesh PLG chunk
        return chunk;
    }

    private class BinMeshPlgMesh
    {
        public uint IndexCount;
        public readonly List<uint> Indices = new();
        public uint MaterialIndex;
    }
}