using System.IO;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class MaterialListChunk : Chunk
{
    public MateralListStructChunk MaterialListStruct;
    public List<MaterialChunk> Materials;

    public MaterialListChunk(GeometryChunk? parent, ChunkHeader header) : base(parent, header)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine(
            $"MaterialListChunk.Read: Reading material list chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read material list struct
        ReadMaterialListStruct(binaryReader);

        // Read materials
        ReadMaterials(binaryReader);
    }

    private void ReadMaterialListStruct(BinaryReader fileAccess)
    {
        // Read material list struct header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // Create material list struct chunk
        MaterialListStruct = new MateralListStructChunk(this, header);

        // Read material list struct chunk data
        MaterialListStruct.Read(fileAccess);
    }

    private void ReadMaterials(BinaryReader fileAccess)
    {
        Materials = new List<MaterialChunk>();

        for (var materialIndex = 0; materialIndex < MaterialListStruct.MaterialCount; materialIndex++)
            ReadMaterial(fileAccess);
    }

    private void ReadMaterial(BinaryReader fileAccess)
    {
        // Read material header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // Create material chunk
        var material = new MaterialChunk(this, header);

        // Read material chunk data
        material.Read(fileAccess);

        // Add material chunk to material list struct
        Materials.Add(material);
    }
}