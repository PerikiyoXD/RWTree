using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class ClumpChunk(ChunkHeader header) : Chunk(null, header)
{
    public List<AtomicChunk> Atomics;
    public ClumpStructChunk ClumpStruct;
    public ExtensionChunk Extension;
    public FrameListChunk FrameList;
    public GeometryListChunk GeometryList;

    // Read clump chunk data
    // Clumps have a Struct, a List of Frames, a Geometry List, and an Extension
    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine($"ClumpChunk.Read: Reading clump chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read Clump Struct Chunk
        ClumpStruct = ClumpStructChunk.ReadClumpStruct(binaryReader, this);

        // Read Frame List Chunk
        FrameList = FrameListChunk.ReadFrameList(binaryReader, this);

        // Read Geometry List Chunk
        GeometryList = GeometryListChunk.ReadGeometryList(binaryReader, this);

        // Read Atomic Chunks
        Atomics = new List<AtomicChunk>();

        for (var atomicChunkIndex = 0; atomicChunkIndex < ClumpStruct.AtomicChunkCount; atomicChunkIndex++)
            Atomics.Add(AtomicChunk.ReadAtomic(binaryReader, this));

        // Read Extension Chunk
        Extension = ExtensionChunk.ReadExtension(binaryReader, this);

        // Print debug message
        Console.WriteLine($"ClumpChunk.Read: Read clump chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    public static ClumpChunk ReadClump(BinaryReader fileAccess)
    {
        // Print debug message
        Console.WriteLine($"ClumpChunk.ReadClump: Reading clump at position: '{fileAccess.BaseStream.Position}'");

        // Read clump header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // If the header type is not clump, log an error, skip the chunk by the header size and return null


        // Create clump chunk
        var clump = new ClumpChunk(header);

        // Read clump chunk data
        clump.Read(fileAccess);

        // Print debug message
        Console.WriteLine($"ClumpChunk.ReadClump: Read clump up to position: '{fileAccess.BaseStream.Position}'");

        // Return clump chunk
        return clump;
    }

    public new TreeViewItem ToTreeViewItem()
    {
        var chunkItem = base.ToTreeViewItem();
        
        // Set image

        chunkItem.Items.Add(ClumpStruct.ToTreeViewItem());
        chunkItem.Items.Add(FrameList.ToTreeViewItem());
        chunkItem.Items.Add(GeometryList.ToTreeViewItem());

        foreach (var atomicChunk in Atomics)
            chunkItem.Items.Add(atomicChunk.ToTreeViewItem());

        if (Extension != null)
            chunkItem.Items.Add(Extension.ToTreeViewItem());
        else
            chunkItem.Items.Add(new TreeViewItem { Header = "Extension: None" });

        return chunkItem;
    }
}