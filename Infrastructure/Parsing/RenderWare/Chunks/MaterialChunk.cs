using System.IO;
using RWTree.Domain.Models;

namespace RWTree.Infrastructure.Parsing.RenderWare.Chunks;

public class MaterialChunk(MaterialListChunk? materialListChunk, ChunkHeader header)
    : Chunk(materialListChunk, header), IBinaryReadWrite
{
    public ExtensionChunk Extension = null!;
    public MaterialStructChunk MaterialStruct = null!;
    private List<TextureChunk> _textures = [];

    public override void Read(BinaryReader binaryReader)
    {
        Console.WriteLine(
            $"MaterialChunk.Read: Reading material chunk at position: '{binaryReader.BaseStream.Position}'");
        base.Read(binaryReader);

        MaterialStruct = MaterialStructChunk.ReadMaterialStruct(binaryReader, this);
        for (var textureIndex = 0; textureIndex < MaterialStruct.TextureCount; textureIndex++)
        {
            var texture = TextureChunk.ReadTexture(binaryReader, this);
            if (texture != null)
                _textures.Add(texture);
        }

        Extension = ExtensionChunk.ReadExtension(binaryReader, this)!;

        Console.WriteLine(
            $"MaterialChunk.Read: Read material chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    protected override void AddChildChunks(ChunkNode node)
    {
        // Add MaterialStruct as child
        if (MaterialStruct != null)
        {
            node.AddChild(MaterialStruct.ToChunkNode());
        }

        // Add all Textures as children
        foreach (var texture in _textures)
        {
            node.AddChild(texture.ToChunkNode());
        }

        // Add Extension as child
        if (Extension != null)
        {
            node.AddChild(Extension.ToChunkNode());
        }
    }
}