using System.Drawing;
using System.IO;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class MaterialStructChunk(MaterialChunk? parent, ChunkHeader header) : Chunk(parent, header)
{
    public float Ambient;
    public Color Color;
    public float Diffuse;
    public int Flags;
    public float Specular;
    public int TextureCount;
    public int Unused;

    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine($"MaterialStructChunk.Read: Reading material struct chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read flags
        Flags = (int)binaryReader.ReadUInt32();

        var r = binaryReader.ReadByte() / 255;
        var g = binaryReader.ReadByte() / 255;
        var b = binaryReader.ReadByte() / 255;
        var a = binaryReader.ReadByte() / 255;

        Color = Color.FromArgb((int)(a * 255), (int)(r * 255), (int)(g * 255), (int)(b * 255));

        // Read unused
        Unused = (int)binaryReader.ReadUInt32();

        // Read texture count
        TextureCount = (int)binaryReader.ReadUInt32();

        // Read ambient
        Ambient = binaryReader.ReadSingle();

        // Read specular
        Specular = binaryReader.ReadSingle();

        // Read diffuse
        Diffuse = binaryReader.ReadSingle();

        // Print debug message
        Console.WriteLine($"MaterialStructChunk.Read: Read material struct chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    /// <summary>
    ///     Read material struct chunk from file access.
    /// </summary>
    public static MaterialStructChunk ReadMaterialStruct(BinaryReader fileAccess, MaterialChunk? parent)
    {
        // Read material struct header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // Handle expected type mismatch
        if (header.Type != ChunkType.Struct)
            throw new Exception($"Expected chunk type '{ChunkType.Struct.ToString()}', got '{header.Type}'");

        // Create material struct chunk
        var chunk = new MaterialStructChunk(parent, header);

        // Read material struct chunk data
        chunk.Read(fileAccess);

        return chunk;
    }
}