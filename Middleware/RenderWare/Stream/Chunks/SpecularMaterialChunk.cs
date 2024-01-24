using System.IO;
using System.Text;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

internal class SpecularMaterialChunk(Chunk? parent, ChunkHeader header) : Chunk(parent, header)
{
    public float Level;
    public string TextureName = "";

    public override void Read(BinaryReader binaryReader)
    {
        Console.WriteLine(
            $"SpecularMaterialChunk.Read: Reading specular material chunk at position: '{binaryReader.BaseStream.Position}'");

        base.Read(binaryReader);

        Level = binaryReader.ReadSingle();

        TextureName = Encoding.UTF8.GetString(binaryReader.ReadBytes(24)).TrimEnd('\0');

        Console.WriteLine(
            $"SpecularMaterialChunk.Read: Read specular material chunk up to position: '{binaryReader.BaseStream.Position}'");
    }
}