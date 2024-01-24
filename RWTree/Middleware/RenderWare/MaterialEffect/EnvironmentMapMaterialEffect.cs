using System.IO;
using RWTree.Middleware.RenderWare.Stream;
using RWTree.Middleware.RenderWare.Stream.Chunks;

namespace RWTree.Middleware.RenderWare.MaterialEffect;

public class EnvironmentMapMaterialEffect : MaterialEffect
{
    public TextureChunk EnvironmentMapTexture;
    public bool HasEnvironmentMap;
    public float ReflectionCoefficient;
    public bool UseFrameBufferAlpha;

    public EnvironmentMapMaterialEffect(MaterialEffectsPlgChunk? parent) : base(parent)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        base.Read(binaryReader);

        // Read reflection coefficient
        ReflectionCoefficient = binaryReader.ReadSingle();
        Console.WriteLine($"Reflection coefficient: '{ReflectionCoefficient}' @ '{binaryReader.BaseStream.Position}'");

        // Read use frame buffer alpha
        UseFrameBufferAlpha = binaryReader.ReadUInt32() != 0;
        Console.WriteLine($"Use frame buffer alpha: '{UseFrameBufferAlpha}' @ '{binaryReader.BaseStream.Position}'");

        // Read has environment map
        HasEnvironmentMap = binaryReader.ReadUInt32() != 0;
        Console.WriteLine($"Has environment map: '{HasEnvironmentMap}' @ '{binaryReader.BaseStream.Position}'");

        // Read environment map texture
        if (HasEnvironmentMap) EnvironmentMapTexture = TextureChunk.ReadTexture(binaryReader, Parent);

        // Print debug message
        Console.WriteLine(
            $"EnvironmentMapMaterialEffect.Read: Read environment map material effect up to position: '{binaryReader.BaseStream.Position}'");
    }
}