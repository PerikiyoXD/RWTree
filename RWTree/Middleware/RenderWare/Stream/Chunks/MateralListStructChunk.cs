using System.IO;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class MateralListStructChunk : Chunk
{
    public int MaterialCount;
    public List<int> MaterialReferences = new();

    public MateralListStructChunk(MaterialListChunk? parent, ChunkHeader header) : base(parent, header)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine($"MateralListStructChunk.Read: Reading material list struct chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read material count
        MaterialCount = (int)binaryReader.ReadUInt32();

        // Read material references
        for (var materialIndex = 0; materialIndex < MaterialCount; materialIndex++)
            MaterialReferences.Add((int)binaryReader.ReadUInt32());

        // Print debug message
        Console.WriteLine($"MateralListStructChunk.Read: Read material list struct chunk up to position: '{binaryReader.BaseStream.Position}'");
    }
}