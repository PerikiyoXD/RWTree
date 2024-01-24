using System.IO;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class RightToRenderChunk : Chunk
{
    public uint ExtraData;
    public uint PluginId;

    public RightToRenderChunk(Chunk? parent, ChunkHeader header) : base(parent, header)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        Console.WriteLine(
            $"RightToRenderChunk.Read: Reading right to render chunk at position: '{binaryReader.BaseStream.Position}'");

        base.Read(binaryReader);

        {
            PluginId = binaryReader.ReadUInt32();
            ExtraData = binaryReader.ReadUInt32();
        }

        Console.WriteLine(
            $"RightToRenderChunk.Read: Read right to render chunk up to position: '{binaryReader.BaseStream.Position}'");
    }
}