using System.IO;
using System.Windows.Controls;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class GeometryListChunk : Chunk
{
    public List<GeometryChunk> Geometries;
    private GeometryListStructChunk geometryListStruct;

    public GeometryListChunk(ClumpChunk? parent, ChunkHeader header) : base(parent, header)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine(
            $"GeometryListChunk.Read: Reading geometry list chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read geometry list struct
        ReadGeometryListStruct(binaryReader);

        // Read geometry chunks
        ReadGeometryChunks(binaryReader);

        // Print debug message
        Console.WriteLine(
            $"GeometryListChunk.Read: Read geometry list chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    private void ReadGeometryListStruct(BinaryReader fileAccess)
    {
        // Read geometry list struct header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // Create geometry list struct chunk
        geometryListStruct = new GeometryListStructChunk(this, header);

        // Read geometry list struct chunk data
        geometryListStruct.Read(fileAccess);
    }

    private void ReadGeometryChunks(BinaryReader fileAccess)
    {
        Geometries = new List<GeometryChunk>();

        for (var geometryIndex = 0; geometryIndex < geometryListStruct.GeometryCount; geometryIndex++)
            ReadGeometryChunk(fileAccess);
    }

    private void ReadGeometryChunk(BinaryReader fileAccess)
    {
        // Read geometry header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // Create geometry chunk
        var geometry = new GeometryChunk(this, header);

        // Read geometry chunk data
        geometry.Read(fileAccess);

        // Add geometry chunk to geometry list struct
        Geometries.Add(geometry);
    }

    public override void Write(BinaryWriter binaryWriter)
    {
        throw new NotImplementedException();
    }

    public static GeometryListChunk ReadGeometryList(BinaryReader fileAccess,
        ClumpChunk? clumpChunk)
    {
        // Print debug message
        Console.WriteLine(
            $"GeometryListChunk.ReadGeometryList: Reading geometry list at position: '{fileAccess.BaseStream.Position}'");

        // Read geometry list header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // Create geometry list chunk
        var geometryList = new GeometryListChunk(clumpChunk, header);

        // Read geometry list chunk data
        geometryList.Read(fileAccess);

        // Print debug message
        Console.WriteLine(
            $"GeometryListChunk.ReadGeometryList: Read geometry list up to position: '{fileAccess.BaseStream.Position}'");

        // Return geometry list
        return geometryList;
    }
    
    public new TreeViewItem ToTreeViewItem()
    {
        var treeViewItem = base.ToTreeViewItem();

        treeViewItem.Items.Add(geometryListStruct.ToTreeViewItem());

        foreach (var geometry in Geometries)
        {
            treeViewItem.Items.Add(geometry.ToTreeViewItem());
        }

        return treeViewItem;
    }
}