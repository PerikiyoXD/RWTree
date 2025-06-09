using System.IO;
using RWTree.Infrastructure.Parsing.RenderWare;

namespace RWTree.Infrastructure.Parsing.MaterialEffect;

public class NullMaterialEffect : MaterialEffect
{
    public NullMaterialEffect(MaterialEffectsPlgChunk? parent) : base(parent)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        // Call base class method
        base.Read(binaryReader);

        // Print debug message
        Console.WriteLine($"NullEffect.Read: Read null effect at position: '{binaryReader.BaseStream.Position}'");
    }
}