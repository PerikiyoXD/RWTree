using System.IO;
using System.Text;
using RWTree.Middleware.RenderWare.Stream.Chunks;

namespace RWTree.Middleware.RenderWare.Stream;

public class NodeNameChunk(ExtensionChunk? parent, ChunkHeader header) : Chunk(parent, header)
{
    public string NodeName = "";

    public override void Read(BinaryReader binaryReader)
    {
        Console.WriteLine($"NodeNameChunk.Read: Reading node name chunk at position: '{binaryReader.BaseStream.Position}'");

        base.Read(binaryReader);

        NodeName = Encoding.UTF8.GetString(binaryReader.ReadBytes((int)Header.Size)).TrimEnd('\0');

        Console.WriteLine($"NodeNameChunk.Read: Read node name chunk up to position: '{binaryReader.BaseStream.Position}'");
    }
}