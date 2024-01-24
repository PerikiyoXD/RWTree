using System.IO;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class TextureStructChunk(TextureChunk? parent, ChunkHeader header) : Chunk(parent, header)
{
    public uint FilterFlags;


    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine($"TextureStructChunk.Read: Reading texture struct chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read filter flags
        FilterFlags = binaryReader.ReadUInt32();

        // TODO: Properly decode the filter flags

        // Print debug message
        Console.WriteLine($"TextureStructChunk.Read: Read texture struct chunk up to position: '{binaryReader.BaseStream.Position}'");
    }


    public static TextureStructChunk ReadTextureStruct(BinaryReader fileAccess, TextureChunk? parent)
    {
        // Read texture struct header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // If the header type is not texture struct, log an error, skip the chunk by the header size and return null
        if (header.Type != ChunkType.Struct)
        {
            Console.WriteLine($"TextureStructChunk.ReadTextureStruct: Expected chunk type '{ChunkType.Struct.ToString()}', but got '{header.Type}' at position: '{fileAccess.BaseStream.Position}'");
            fileAccess.BaseStream.Seek(fileAccess.BaseStream.Position + header.Size, SeekOrigin.Begin);
            return null;
        }

        // Create texture struct chunk
        var textureStruct = new TextureStructChunk(parent, header);

        // Read texture struct chunk data
        textureStruct.Read(fileAccess);

        return textureStruct;
    }
}