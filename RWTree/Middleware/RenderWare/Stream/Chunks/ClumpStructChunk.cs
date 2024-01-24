using System.IO;
using System.Windows.Controls;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class ClumpStructChunk(ClumpChunk? parent, ChunkHeader header) : Chunk(parent, header)
{
    public int AtomicChunkCount;
    public int CameraChunkCount;
    public int LightChunkCount;

    // Read clump struct chunk data
    // ClumpStructs does not have sub-chunks
    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine($"ClumpChunk.StructData.Read: Reading clump struct at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read clump struct chunk data
        AtomicChunkCount = (int)binaryReader.ReadUInt32();

        if (LibraryIdUtils.LibraryIdUnpackVersion(Header.Version) >= 0x33000)
        {
            LightChunkCount = (int)binaryReader.ReadUInt32();
            CameraChunkCount = (int)binaryReader.ReadUInt32();
        }

        // Print debug message
        Console.WriteLine($"ClumpChunk.StructData.Read: Read clump struct up to position: '{binaryReader.BaseStream.Position}'");
    }

    public override void Write(BinaryWriter binaryWriter)
    {
        // Print debug message
        Console.WriteLine($"ClumpChunk.StructData.Write: Writing clump struct at position: '{binaryWriter.BaseStream.Position}'");

        // Call base class method
        base.Write(binaryWriter);
        
        
        binaryWriter.Write((uint)AtomicChunkCount);

        if (LibraryIdUtils.LibraryIdUnpackVersion(Header.Version) >= 0x33000)
        {
            binaryWriter.Write((uint)LightChunkCount);
            binaryWriter.Write((uint)CameraChunkCount);
        }
        
            


        
        

        // Print debug message
        Console.WriteLine($"ClumpChunk.StructData.Write: Wrote clump struct up to position: '{binaryWriter.BaseStream.Position}'");
    }

    public new TreeViewItem ToTreeViewItem()
    {
        var treeViewItem = base.ToTreeViewItem();

        treeViewItem.Items.Add(new TreeViewItem {Header = $"Atomic Chunk Count: {AtomicChunkCount}"});
        treeViewItem.Items.Add(new TreeViewItem {Header = $"Light Chunk Count: {LightChunkCount}"});
        treeViewItem.Items.Add(new TreeViewItem {Header = $"Camera Chunk Count: {CameraChunkCount}"});

        return treeViewItem;
    }

    public static ClumpStructChunk ReadClumpStruct(BinaryReader fileAccess, ClumpChunk? parent)
    {
        // Read clump struct header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // Create clump struct chunk
        var clumpStruct = new ClumpStructChunk(parent, header);

        // Read clump struct chunk data
        clumpStruct.Read(fileAccess);

        return clumpStruct;
    }
}