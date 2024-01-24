using System.IO;
using RWTree.Middleware.RenderWare.Stream;

namespace RWTree.Middleware.RenderWare.MaterialEffect;

public abstract class MaterialEffect(MaterialEffectsPlgChunk? parent) : IBinaryReadWrite
{
    public uint EffectType;
    public MaterialEffectsPlgChunk? Parent = parent;

    public virtual void Read(BinaryReader binaryReader)
    {
        // Read effect type
        EffectType = binaryReader.ReadUInt32();
        Console.WriteLine($"Effect type: '{EffectType}'");
    }

    public void Write(BinaryWriter binaryWriter)
    {
        throw new NotImplementedException();
    }
}