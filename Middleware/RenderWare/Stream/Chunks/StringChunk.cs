using System.IO;
using System.Text;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class StringChunk(Chunk? parent, ChunkHeader header) : Chunk(parent, header)
{
    public string String = "";

    public override void Read(BinaryReader binaryReader)
    {
        Console.WriteLine($"StringChunk.Read: Reading string chunk at position: '{binaryReader.BaseStream.Position}'");

        base.Read(binaryReader);

        String = Encoding.UTF8.GetString(binaryReader.ReadBytes((int)Header.Size)).TrimEnd('\0');

        Console.WriteLine($"StringChunk.Read: Read string chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    public static StringChunk ReadString(BinaryReader fileAccess, Chunk? parent)
    {
        var header = ChunkHeader.ReadHeader(fileAccess);

        if (header.Type != ChunkType.String)
        {
            fileAccess.BaseStream.Seek(fileAccess.BaseStream.Position + header.Size, SeekOrigin.Begin);
            Console.WriteLine($"StringChunk.ReadString: Expected string chunk, but got '{header.Type}' chunk at position: '{fileAccess.BaseStream.Position}'");
            return null;
        }

        var stringChunk = new StringChunk(parent, header);

        stringChunk.Read(fileAccess);

        return stringChunk;
    }
}