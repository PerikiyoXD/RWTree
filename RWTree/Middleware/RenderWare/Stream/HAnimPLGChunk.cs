using System.IO;

namespace RWTree.Middleware.RenderWare.Stream;

public class HAnimPlgChunk(Chunk? parent, ChunkHeader header) : Chunk(parent, header)
{
    public uint AnimationVersion;
    public uint NodeCount;
    public uint NodeId;

    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine($"HAnimPLGChunk.Read: Reading HAnimPLG chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read HAnimPLG chunk data
        AnimationVersion = binaryReader.ReadUInt32();
        NodeId = binaryReader.ReadUInt32();
        NodeCount = binaryReader.ReadUInt32();
        
        // Probably there's more data, seek to the end of the chunk
        binaryReader.BaseStream.Seek(StartPosition + Header.Size + 12, SeekOrigin.Begin);

        // Print debug message
        Console.WriteLine($"HAnimPLGChunk.Read: Read HAnimPLG chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    public override void Write(BinaryWriter binaryWriter)
    {
        base.Write(binaryWriter);
    }
}