using System.IO;

namespace RWTree.Middleware.RenderWare.Stream;

public class BinMeshPlgStructChunk : Chunk
{
    public uint Flags;
    public List<BinMesh> Meshes = null;
    public uint NumMeshes;
    public uint TotalIndices;

    public BinMeshPlgStructChunk(Chunk? parent, ChunkHeader header) : base(parent, header)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine(
            $"BinMeshPLGStructChunk.Read: Reading bin mesh PLG struct chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read bin mesh PLG struct chunk data
        Flags = binaryReader.ReadUInt32();
        NumMeshes = binaryReader.ReadUInt32();
        TotalIndices = binaryReader.ReadUInt32();

        // Read data
        for (var meshIndex = 0; meshIndex < NumMeshes; meshIndex++)
        {
            uint indexCount = binaryReader.ReadUInt32();
            uint materialIndex = binaryReader.ReadUInt32();
        }


        // Print debug message
        Console.WriteLine(
            $"BinMeshPLGStructChunk.Read: Read bin mesh PLG struct chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    public class BinMesh
    {
    }
}