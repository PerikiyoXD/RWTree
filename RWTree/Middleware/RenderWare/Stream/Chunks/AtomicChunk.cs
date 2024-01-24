using System.IO;
using System.Windows.Controls;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class AtomicChunk : Chunk
{
    public AtomicStructChunk AtomicStruct;
    public ExtensionChunk Extension;

    public AtomicChunk(ClumpChunk? parent, ChunkHeader header) : base(parent, header)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine($"AtomicChunk.Read: Reading atomic chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read atomic struct
        AtomicStruct = AtomicStructChunk.ReadAtomicStruct(binaryReader, this);

        // Read extension
        Extension = ExtensionChunk.ReadExtension(binaryReader, this);

        // Print debug message
        Console.WriteLine($"AtomicChunk.Read: Read atomic chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    public new TreeViewItem ToTreeViewItem()
    {
        var chunkItem = base.ToTreeViewItem();

        chunkItem.Items.Add(AtomicStruct.ToTreeViewItem());
        if (Extension != null)
            chunkItem.Items.Add(Extension.ToTreeViewItem());
        else
            chunkItem.Items.Add(new TreeViewItem { Header = "Extension: None" });

        return chunkItem;
    }

    public static AtomicChunk ReadAtomic(BinaryReader fileAccess, ClumpChunk? parent)
    {
        // Read atomic struct header
        var header = ChunkHeader.ReadHeader(fileAccess);

        if (header.Type != ChunkType.Atomic)
            Console.WriteLine(
                $"AtomicChunk.ReadAtomic: Expected atomic struct chunk, but got '{header.Type}' chunk instead");

        // Create atomic struct chunk
        var atomic = new AtomicChunk(parent, header);

        // Read atomic struct chunk data
        atomic.Read(fileAccess);

        return atomic;
    }
}