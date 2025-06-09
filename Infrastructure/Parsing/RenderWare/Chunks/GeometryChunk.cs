using System.IO;
using RWTree.Domain.Models;

namespace RWTree.Infrastructure.Parsing.RenderWare.Chunks;

public class GeometryChunk : Chunk
{
    public ExtensionChunk Extension = null!;
    public GeometryStructChunk GeometryStruct = null!;

    public MaterialListChunk MaterialList = null!;

    public GeometryChunk(GeometryListChunk? parent, ChunkHeader header) : base(parent, header)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine(
            $"GeometryChunk.Read: Reading geometry chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read geometry struct
        ReadGeometryStruct(binaryReader);

        // Read material list
        ReadMaterialList(binaryReader); // Read extension
        Extension = ExtensionChunk.ReadExtension(binaryReader, this)!;

        // Print debug message
        Console.WriteLine(
            $"GeometryChunk.Read: Read geometry chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    private void ReadGeometryStruct(BinaryReader fileAccess)
    {
        // Read geometry struct header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // Create geometry struct chunk
        GeometryStruct = new GeometryStructChunk(this, header);

        // Read geometry struct chunk data
        GeometryStruct.Read(fileAccess);
    }

    private void ReadMaterialList(BinaryReader fileAccess)
    {
        // Read material list header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // Create material list chunk
        MaterialList = new MaterialListChunk(this, header);

        // Read material list chunk data
        MaterialList.Read(fileAccess);
    }

    protected override void AddChildChunks(ChunkNode node)
    {
        // Add geometry struct
        if (GeometryStruct != null)
        {
            var geometryStructNode = GeometryStruct.ToChunkNode();
            node.AddChild(geometryStructNode);
        }

        // Add material list
        if (MaterialList != null)
        {
            var materialListNode = MaterialList.ToChunkNode();
            node.AddChild(materialListNode);
        }

        // Add extension
        if (Extension != null)
        {
            var extensionNode = Extension.ToChunkNode();
            node.AddChild(extensionNode);
        }
    }
}