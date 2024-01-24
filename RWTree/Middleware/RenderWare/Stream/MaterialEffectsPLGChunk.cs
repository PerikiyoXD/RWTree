using System.IO;
using RWTree.Middleware.RenderWare.MaterialEffect;

namespace RWTree.Middleware.RenderWare.Stream;

public class MaterialEffectsPlgChunk(Chunk? parent, ChunkHeader header) : Chunk(parent, header)
{
    public uint StoredEffectType;
    public List<MaterialEffect.MaterialEffect> MaterialEffects = [];

    public override void Read(BinaryReader binaryReader)
    {
        base.Read(binaryReader);

        StoredEffectType = binaryReader.ReadUInt32();
        Console.WriteLine($"Stored effect type: '{StoredEffectType}'");

        switch ((int)StoredEffectType)
        {
            case 0:
                Console.WriteLine("NullMaterialEffect");
                MaterialEffects.Add(new NullMaterialEffect(this));
                break;
            case 1:
                Console.WriteLine("BumpMapMaterialEffect");
                MaterialEffects.Add(new BumpMapMaterialEffect(this));
                break;
            case 2:
                Console.WriteLine("EnvironmentMapMaterialEffect");
                MaterialEffects.Add(new EnvironmentMapMaterialEffect(this));
                break;
            case 3:
                Console.WriteLine("BumpMapEnvironmentMapMaterialEffect");
                MaterialEffects.Add(new BumpMapMaterialEffect(this));
                MaterialEffects.Add(new EnvironmentMapMaterialEffect(this));
                break;
            case 4:
                Console.WriteLine("DualTextureMaterialEffect");
                MaterialEffects.Add(new DualTextureMaterialEffect(this));
                break;
            case 5:
                Console.WriteLine("UVTransformMaterialEffect");
                MaterialEffects.Add(new UvTransformMaterialEffect(this));
                break;
            case 6:
                Console.WriteLine("DualTextureUVTransformMaterialEffect");
                MaterialEffects.Add(new DualTextureMaterialEffect(this));
                MaterialEffects.Add(new UvTransformMaterialEffect(this));
                break;
            default:
                Console.WriteLine(
                    $"MaterialEffectsPLGChunk.Read: Unknown stored effect type: '{StoredEffectType}'");
                // Skip the chunk
                break;
        }

        foreach (var t in MaterialEffects)
            t.Read(binaryReader);

        // Check if we are at the end position
        if (binaryReader.BaseStream.Position != StartPosition + 12 + Header.Size)
            // Print debug message
            Console.WriteLine(
                $"MaterialEffectsPLGChunk.Read: Some data wasn't read, position: '{binaryReader.BaseStream.Position}', end position: '{StartPosition + Header.Size}'");

        binaryReader.BaseStream.Seek(StartPosition + 12 + Header.Size, SeekOrigin.Begin);

        Console.WriteLine(
            $"MaterialEffectsPLGChunk.Read: Read material effects PLG chunk up to position: '{binaryReader.BaseStream.Position}'");
    }
}