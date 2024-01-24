using System.IO;
using System.Windows.Controls;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class ExtensionChunk : Chunk
{
    public List<Chunk?> Chunks;

    public ExtensionChunk(Chunk? parent, ChunkHeader chunkHeader) : base(parent, chunkHeader)
    {
    }

    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine($"ExtensionChunk.Read: Reading extension chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        Chunks = new List<Chunk?>();

        // We don't know beforehand how many chunks there are, so we read until we reach the end of the chunk, which
        // is calculated using the Extension chunk start position + Extension chunk size. Once we're at the end of the
        // chunk, we stop reading.

        // Read chunks until we reach the end of the extension chunk
        var endOfExtensionChunkPosition = StartPosition + Header.Size;

        // Start reading chunks
        while (binaryReader.BaseStream.Position < endOfExtensionChunkPosition)
        {
            var header = ChunkHeader.ReadHeader(binaryReader);

            var chunk = CreateChunkForType(header, this);
            if (chunk == null)
            {
                binaryReader.BaseStream.Seek(StartPosition + Header.Size + 12, SeekOrigin.Begin);
                break;
            }

            chunk.Read(binaryReader);

            Chunks.Add(chunk);
        }


        // Print debug message
        Console.WriteLine($"ExtensionChunk.Read: Read extension chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    public override void Write(BinaryWriter binaryWriter)
    {
        base.Write(binaryWriter);
    }

    public new TreeViewItem ToTreeViewItem()
    {
        var treeViewItem = new TreeViewItem { Header = "Extension" };

        foreach (var chunk in Chunks)
        {
            treeViewItem.Items.Add(chunk == null ? new TreeViewItem { Header = "NULL" } : chunk.ToTreeViewItem());
        }

        return treeViewItem;
    }

    public static ExtensionChunk ReadExtension(BinaryReader fileAccess, Chunk? parent)
    {
        // Read extension header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // If header size is 0, there is no extension, so we skip it
        if (header.Size == 0) return null;

        // Create extension chunk
        var extension = new ExtensionChunk(parent, header);

        // Read extension chunk data
        extension.Read(fileAccess);

        return extension;
    }
}