using System.IO;
using System.Windows.Controls;
using RWTree.Middleware.RenderWare.Stream.Chunks;

namespace RWTree.Middleware.RenderWare.Stream;

public abstract class Chunk(Chunk? parent, ChunkHeader header) : IBinaryReadWrite, ITreeItem
{
    public long EndPosition;
    public readonly ChunkHeader Header = header;
    public Chunk? Parent = parent;

    protected long StartPosition;
    public ChunkType Type = ChunkType.Unknown;

    public virtual void Read(BinaryReader binaryReader)
    {
        StartPosition = binaryReader.BaseStream.Position - 12; // 12 bytes for the header
        EndPosition = StartPosition + Header.Size;
    }

    public virtual void Write(BinaryWriter binaryWriter)
    {
        throw new NotImplementedException();
    }

    public static Chunk? ReadChunk(BinaryReader fileAccess)
    {
        // Print debug message
        Console.WriteLine($"Chunk.ReadChunk: Reading chunk at position: '{fileAccess.BaseStream.Position}'");

        var header = ChunkHeader.ReadHeader(fileAccess);

        var chunk = CreateChunkForType(header);

        if (chunk == null)
        {
            // Print debug message
            Console.WriteLine($"Chunk type {header.Type:X} is not a valid chunk, skipping");
            // Skip the chunk, since we don't know how to read it
            fileAccess.BaseStream.Seek(fileAccess.BaseStream.Position + header.Size, SeekOrigin.Begin);
            return null;
        }

        // Print debug message
        Console.WriteLine($"Chunk.ReadChunk: Reading chunk type: {header.Type} at position {fileAccess.BaseStream.Position} with size {header.Size:X}");
        chunk.Read(fileAccess);

        // Print debug message
        Console.WriteLine($"Chunk.ReadChunk: Read chunk type: {header.Type} up to position {fileAccess.BaseStream.Position} with size {header.Size:X}");
        // Print debug message
        Console.WriteLine($"Current position: {fileAccess.BaseStream.Position}");

        // We should've read the entire chunk, so we should be at the end of the chunk. If not, something went wrong
        if (fileAccess.BaseStream.Position != chunk.EndPosition + 12)
            // Print debug message
            Console.WriteLine($"Chunk.ReadChunk: Something went wrong, we should be at position {chunk.EndPosition + 12} (Chunk End) but we are at {fileAccess.BaseStream.Position}");

        // Print debug message
        Console.WriteLine($"Chunk.ReadChunk: Read chunk up to position: '{fileAccess.BaseStream.Position}'");

        return chunk;
    }

    protected static Chunk? CreateChunkForType(ChunkHeader header, Chunk? parent = null)
    {
        switch (header.Type)
        {
            case ChunkType.String:
            {
                return new StringChunk(parent, header);
            }
            case ChunkType.Clump:
            {
                return new ClumpChunk(header);
            }
            case ChunkType.FrameList:
            {
                if (parent is ClumpChunk chunk)
                    return new FrameListChunk(chunk, header);
                throw new Exception($"! Chunk type {header.Type} is not a valid chunk !");
            }
            case ChunkType.HAnimPlg:
            {
                if (parent is ExtensionChunk chunk)
                    return new HAnimPlgChunk(chunk, header);
                throw new Exception($"! Chunk type {header.Type} is not a valid chunk !");
            }
            case ChunkType.MaterialEffectsPlg:
            {
                if (parent is ExtensionChunk chunk)
                    return new MaterialEffectsPlgChunk(chunk, header);
                throw new Exception($"! Chunk type {header.Type} is not a valid chunk !");
            }
            case ChunkType.BinMeshPlg:
            {
                if (parent is ExtensionChunk chunk)
                    return new BinMeshPlgChunk(chunk, header);
                throw new Exception($"Chunk type {header.Type} is not a valid chunk");
            }
            case ChunkType.UserDataPlg:
            {
                if (parent is ExtensionChunk chunk)
                    return new UserDataPlgChunk(chunk, header);
                throw new Exception($"Chunk type {header.Type} is not a valid chunk");
            }
            case ChunkType.NodeName:
            {
                if (parent is ExtensionChunk chunk)
                    return new NodeNameChunk(chunk, header);
                throw new Exception($"Chunk type {header.Type} is not a valid chunk");
            }
            case ChunkType.Breakable:
            {
                if (parent is ExtensionChunk chunk)
                    return new BreakableChunk(chunk, header);
                throw new Exception($"Chunk type {header.Type} is not a valid chunk");
            }
            case ChunkType.SpecularMaterial:
            {
                if (parent is ExtensionChunk chunk)
                    return new SpecularMaterialChunk(chunk, header);
                throw new Exception($"Chunk type {header.Type} is not a valid chunk");
            }
            case ChunkType.ReflectionMaterial:
            {
                if (parent is ExtensionChunk chunk)
                    return new ReflectionMaterialChunk(chunk, header);
                throw new Exception($"Chunk type {header.Type} is not a valid chunk");
            }
            case ChunkType.RightToRender:
            {
                if (parent is ExtensionChunk chunk)
                    return new RightToRenderChunk(chunk, header);
                throw new Exception($"Chunk type {header.Type} is not a valid chunk");
            }
            case ChunkType.CollisionModel:
            {
                if (parent is ExtensionChunk chunk)
                    return new CollisionModelChunk(chunk, header);
                throw new Exception($"Chunk type {header.Type} is not a valid chunk");
            }
            case ChunkType.ToonPlg:
            {
                if (parent is ExtensionChunk chunk)
                    return new ToonPlgChunk(chunk, header);
                throw new Exception($"Chunk type {header.Type} is not a valid chunk");
            }
            case ChunkType.AtomicSection:
            case ChunkType.PlaneSection:
            case ChunkType.World:
            case ChunkType.Spline:
            case ChunkType.Matrix:
            case ChunkType.Geometry:
            case ChunkType.Light:
            case ChunkType.UnicodeString:
            case ChunkType.Atomic:
            case ChunkType.Raster:
            case ChunkType.TextureDictionary:
            case ChunkType.AnimationDatabase:
            case ChunkType.Image:
            case ChunkType.SkinAnimation:
            case ChunkType.GeometryList:
            case ChunkType.AnimAnimation:
            case ChunkType.Team:
            case ChunkType.Crowd:
            case ChunkType.DeltaMorphAnimation:
            case ChunkType.MultiTextureEffectNative:
            case ChunkType.MultiTextureEffectDictionary:
            case ChunkType.TeamDictionary:
            case ChunkType.PlatformIndependentTextureDictionary:
            case ChunkType.TableOfContents:
            case ChunkType.ParticleStandardGlobalData:
            case ChunkType.AltPipe:
            case ChunkType.PlatformIndependentPeds:
            case ChunkType.PatchMesh:
            case ChunkType.ChunkGroupStart:
            case ChunkType.ChunkGroupEnd:
            case ChunkType.UvAnimationDictionary:
            case ChunkType.CollTree:
            case ChunkType.MetricsPlg:
            case ChunkType.SplinePlg:
            case ChunkType.StereoPlg:
            case ChunkType.Vrmlplg:
            case ChunkType.MorphPlg:
            case ChunkType.Pvsplg:
            case ChunkType.MemoryLeakPlg:
            case ChunkType.AnimationPlg:
            case ChunkType.GlossPlg:
            case ChunkType.LogoPlg:
            case ChunkType.MemoryInfoPlg:
            case ChunkType.RandomPlg:
            case ChunkType.PngImagePlg:
            case ChunkType.BonePlg:
            case ChunkType.VrmlAnimPlg:
            case ChunkType.SkyMipmapVal:
            case ChunkType.Mrmplg:
            case ChunkType.LodAtomicPlg:
            case ChunkType.Meplg:
            case ChunkType.LightmapPlg:
            case ChunkType.RefinePlg:
            case ChunkType.SkinPlg:
            case ChunkType.LabelPlg:
            case ChunkType.ParticlesPlg:
            case ChunkType.GeomTxplg:
            case ChunkType.SynthCorePlg:
            case ChunkType.Stqppplg:
            case ChunkType.PartPpplg:
            case ChunkType.CollisionPlg:
            case ChunkType.ParticleSystemPlg:
            case ChunkType.DeltaMorphPlg:
            case ChunkType.PatchPlg:
            case ChunkType.TeamPlg:
            case ChunkType.CrowdPpplg:
            case ChunkType.MipSplitPlg:
            case ChunkType.AnisotropyPlg:
            case ChunkType.GcnMaterialPlg:
            case ChunkType.GeometricPvsplg:
            case ChunkType.XboxMaterialPlg:
            case ChunkType.MultiTexturePlg:
            case ChunkType.ChainPlg:
            case ChunkType.PTankPlg:
            case ChunkType.ParticleStandardPlg:
            case ChunkType.Pdsplg:
            case ChunkType.PrtAdvPlg:
            case ChunkType.NormalMapPlg:
            case ChunkType.Adcplg:
            case ChunkType.UvAnimationPlg:
            case ChunkType.CharacterSetPlg:
            case ChunkType.NohsWorldPlg:
            case ChunkType.ImportUtilPlg:
            case ChunkType.SlerpPlg:
            case ChunkType.OptimPlg:
            case ChunkType.TlWorldPlg:
            case ChunkType.DatabasePlg:
            case ChunkType.RaytracePlg:
            case ChunkType.RayPlg:
            case ChunkType.LibraryPlg:
            case ChunkType._2DPLG:
            case ChunkType.TileRenderPlg:
            case ChunkType.JpegImagePlg:
            case ChunkType.TgaImagePlg:
            case ChunkType.GifImagePlg:
            case ChunkType.QuatPlg:
            case ChunkType.SplinePvsplg:
            case ChunkType.MipmapPlg:
            case ChunkType.MipmapKplg:
            case ChunkType._2DFont:
            case ChunkType.IntersectionPlg:
            case ChunkType.TiffImagePlg:
            case ChunkType.PickPlg:
            case ChunkType.BmpImagePlg:
            case ChunkType.RasImagePlg:
            case ChunkType.SkinFxplg:
            case ChunkType.Vcatplg:
            case ChunkType._2DPath:
            case ChunkType._2DBrush:
            case ChunkType._2DObject:
            case ChunkType._2DShape:
            case ChunkType._2DScene:
            case ChunkType._2DPickRegion:
            case ChunkType._2DObjectString:
            case ChunkType._2DAnimationPLG:
            case ChunkType._2DAnimation:
            case ChunkType._2DKeyframe:
            case ChunkType._2DMaestro:
            case ChunkType.Barycentric:
            case ChunkType.PlatformIndependentTextureDictionaryTk:
            case ChunkType.Toctk:
            case ChunkType.Tpltk:
            case ChunkType.AltPipeTk:
            case ChunkType.AnimationTk:
            case ChunkType.SkinSplitTookit:
            case ChunkType.CompressedKeyTk:
            case ChunkType.GeometryConditioningPlg:
            case ChunkType.WingPlg:
            case ChunkType.GenericPipelineTk:
            case ChunkType.LightmapConversionTk:
            case ChunkType.FilesystemPlg:
            case ChunkType.DictionaryTk:
            case ChunkType.UvAnimationLinear:
            case ChunkType.UvAnimationParameter:
            case ChunkType.NativeDataPlg:
            case ChunkType.ZModelerLock:
            case ChunkType.Unknown1:
            case ChunkType.Unknown2:
            case ChunkType.AtomicVisibilityDistance:
            case ChunkType.ClumpVisibilityDistance:
            case ChunkType.FrameVisibilityDistance:
            case ChunkType.PipelineSet:
            case ChunkType.TexDictionaryLink:
            case ChunkType._2DEffect:
            case ChunkType.ExtraVertColour:
            case ChunkType.GtaHAnim:
            case ChunkType.Unknown:
            case ChunkType.Struct:
            case ChunkType.Extension:
            case ChunkType.Camera:
            case ChunkType.Texture:
            case ChunkType.Material:
            case ChunkType.MaterialList:
                Console.WriteLine($"Chunk type {header.Type} not implemented");
                break;
            default:
                Console.WriteLine($"Chunk type {header.Type:X} not implemented");
                break;
        }

        return null;
    }

    public TreeViewItem ToTreeViewItem()
    {
        // Use the name of the child instance, if it exists, otherwise use the name of the parent instance
        var chunkItem = new TreeViewItem {Header = GetType().Name};
        return chunkItem;
    }
}