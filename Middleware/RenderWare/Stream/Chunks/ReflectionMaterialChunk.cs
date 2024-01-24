using System.IO;
using System.Numerics;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class ReflectionMaterialChunk : Chunk
{
    public float Level;

    public Vector2 Offset;
    /*
    4b - FLOAT    - Environment Map Scale X
    4b - FLOAT    - Environment Map Scale Y
    4b - FLOAT    - Environment Map Offset X
    4b - FLOAT    - Environment Map Offset Y
    4b - FLOAT    - Reflection Intensity (Shininess, 0.0-1.0)
    4b - DWORD    - Environment Texture Ptr, always 0 (zero)
*/

    public Vector2 Scale;
    public uint TexturePtr;

    public ReflectionMaterialChunk(Chunk? parent, ChunkHeader header) : base(parent, header)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        Console.WriteLine($"ReflectionMaterialChunk.Read: Reading reflection material chunk at position: '{binaryReader.BaseStream.Position}'");

        base.Read(binaryReader);

        Scale = new Vector2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
        Offset = new Vector2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
        Level = binaryReader.ReadSingle();
        TexturePtr = binaryReader.ReadUInt32();

        if (TexturePtr != 0)
            Console.WriteLine($"ReflectionMaterialChunk.Read: Expected texture pointer to be 0, but got 0x{TexturePtr:X8} instead");

        Console.WriteLine($"ReflectionMaterialChunk.Read: Read reflection material chunk up to position: '{binaryReader.BaseStream.Position}'");
    }
}