using System.IO;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

internal class BreakableChunk(Chunk? parent, ChunkHeader header) : Chunk(parent, header)
{
    private uint _magic;

    public override void Read(BinaryReader binaryReader)
    {
        Console.WriteLine($"BreakableChunk.Read: Reading breakable chunk at position: '{binaryReader.BaseStream.Position}'");

        base.Read(binaryReader);
        
        _magic = binaryReader.ReadUInt32();

        if (_magic != 0x00000000)
        {
            Console.WriteLine($"BreakableChunk.Read: Expected breakable chunk magic to be 0x00000000, but got 0x{_magic:X8} instead");

            // Advance to the end of the chunk
            binaryReader.BaseStream.Seek(StartPosition + Header.Size, SeekOrigin.Begin);
        }

        Console.WriteLine($"BreakableChunk.Read: Read breakable chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    public override void Write(BinaryWriter binaryWriter)
    {
        Console.WriteLine($"BreakableChunk.Write: Writing breakable chunk at position: '{binaryWriter.BaseStream.Position}'");

        base.Write(binaryWriter);
        
        binaryWriter.Write(_magic);

        Console.WriteLine($"BreakableChunk.Write: Wrote breakable chunk up to position: '{binaryWriter.BaseStream.Position}'");
    }
}