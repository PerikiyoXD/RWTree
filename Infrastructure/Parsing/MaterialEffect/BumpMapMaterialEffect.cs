using System.IO;
using RWTree.Infrastructure.Parsing.RenderWare;
using RWTree.Infrastructure.Parsing.RenderWare.Chunks;

namespace RWTree.Infrastructure.Parsing.MaterialEffect;

public class BumpMapMaterialEffect : MaterialEffect
{
    public TextureChunk BumpMapTexture = null!;
    public bool ContainsBumpMap;
    public bool ContainsHeightMap;
    public TextureChunk HeightMapTexture = null!;
    public float Intensity;

    public BumpMapMaterialEffect(MaterialEffectsPlgChunk? parent) : base(parent)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        base.Read(binaryReader);

        // Read intensity
        Intensity = binaryReader.ReadSingle();

        // Read contains bump map
        ContainsBumpMap = binaryReader.ReadUInt32() != 0; // Read bump map texture
        if (ContainsBumpMap) BumpMapTexture = TextureChunk.ReadTexture(binaryReader, Parent)!;

        ContainsHeightMap = binaryReader.ReadUInt32() != 0;

        if (ContainsHeightMap) HeightMapTexture = TextureChunk.ReadTexture(binaryReader, Parent)!;
    }
}