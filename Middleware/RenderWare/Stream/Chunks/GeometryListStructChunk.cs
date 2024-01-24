using System.IO;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class GeometryListStructChunk : Chunk
{
    public int GeometryCount;

    public GeometryListStructChunk(GeometryListChunk? parent, ChunkHeader header) : base(parent,
        header)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine(
            $"GeometryListStructChunk.Read: Reading geometry list struct chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read geometry count
        GeometryCount = (int)binaryReader.ReadUInt32();

        // Print debug message
        Console.WriteLine(
            $"GeometryListStructChunk.Read: Read geometry list struct chunk up to position: '{binaryReader.BaseStream.Position}'");
    }
}