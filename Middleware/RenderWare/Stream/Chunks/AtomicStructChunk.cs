using System.IO;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class AtomicStructChunk : Chunk
{
    public int Flags;
    public int FrameIndex;
    public int GeometryIndex;
    public int Unused;

    public AtomicStructChunk(AtomicChunk? parent, ChunkHeader header) : base(parent, header)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        Console.WriteLine(
            $"AtomicStructChunk.Read: Reading atomic struct chunk at position: '{binaryReader.BaseStream.Position}'");

        base.Read(binaryReader);

        {
            FrameIndex = (int)binaryReader.ReadUInt32();
            GeometryIndex = (int)binaryReader.ReadUInt32();
            Flags = (int)binaryReader.ReadUInt32();
            Unused = (int)binaryReader.ReadUInt32();
        }

        Console.WriteLine(
            $"AtomicStructChunk.Read: Read atomic struct chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    public static AtomicStructChunk ReadAtomicStruct(BinaryReader fileAccess, AtomicChunk? parent)
    {
        // Read atomic struct header
        var header = ChunkHeader.ReadHeader(fileAccess);

        if (header.Type != ChunkType.Struct)
            Console.WriteLine(
                $"AtomicStructChunk.ReadAtomic: Expected atomic struct chunk, but got '{header.Type}' chunk instead");

        // Create atomic struct chunk
        var atomicStruct = new AtomicStructChunk(parent, header);

        // Read atomic struct chunk data
        atomicStruct.Read(fileAccess);

        return atomicStruct;
    }
}