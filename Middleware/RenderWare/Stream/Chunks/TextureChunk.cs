using System.IO;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class TextureChunk(Chunk? parent, ChunkHeader header) : Chunk(parent, header)
{
    public StringChunk AlphaTextureName;
    public ExtensionChunk Extension;
    public StringChunk TextureName;
    public TextureStructChunk TextureStruct;

    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine(
            $"TextureChunk.Read: Reading texture chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read texture struct
        TextureStruct = TextureStructChunk.ReadTextureStruct(binaryReader, this);

        // Read color texture name
        TextureName = StringChunk.ReadString(binaryReader, this);

        // Read alpha texture name
        AlphaTextureName = StringChunk.ReadString(binaryReader, this);


        // Print debug message
        Console.WriteLine($"TextureChunk.Read: Texture name: '{TextureName.String}'");
        Console.WriteLine($"TextureChunk.Read: Alpha texture name: '{AlphaTextureName.String}'");

        // Read extension
        Extension = ExtensionChunk.ReadExtension(binaryReader, this);

        // Print debug message
        Console.WriteLine(
            $"TextureChunk.Read: Read texture chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    public static TextureChunk ReadTexture(BinaryReader fileAccess, Chunk? parent)
    {
        // Read texture header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // If the header type is not texture, log an error, skip the chunk by the header size and return null
        if (header.Type != ChunkType.Texture)
        {
            Console.WriteLine(
                $"TextureChunk.ReadTexture: Expected chunk type '{ChunkType.Texture.ToString()}', but got '{header.Type}' at position: '{fileAccess.BaseStream.Position}'");
            fileAccess.BaseStream.Seek(fileAccess.BaseStream.Position + header.Size, SeekOrigin.Begin);
            return null;
        }

        // Create texture chunk
        var texture = new TextureChunk(parent, header);

        // Read texture chunk data
        texture.Read(fileAccess);

        return texture;
    }
}