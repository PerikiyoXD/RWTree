using System.IO;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class MaterialChunk(MaterialListChunk? materialListChunk, ChunkHeader header) : Chunk(materialListChunk, header), IBinaryReadWrite
{
    public ExtensionChunk Extension;
    public MaterialStructChunk MaterialStruct;
    private List<TextureChunk> _textures = [];

    public override void Read(BinaryReader binaryReader)
    {
        Console.WriteLine($"MaterialChunk.Read: Reading material chunk at position: '{binaryReader.BaseStream.Position}'");
        base.Read(binaryReader);
        
        MaterialStruct = MaterialStructChunk.ReadMaterialStruct(binaryReader, this);
        
        for (var textureIndex = 0; textureIndex < MaterialStruct.TextureCount; textureIndex++)
        {
            _textures.Add(TextureChunk.ReadTexture(binaryReader, this));
        }

        Extension = ExtensionChunk.ReadExtension(binaryReader, this);

        Console.WriteLine($"MaterialChunk.Read: Read material chunk up to position: '{binaryReader.BaseStream.Position}'");
    }
}