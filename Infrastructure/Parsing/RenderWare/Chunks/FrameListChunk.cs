using System.IO;
using RWTree.Domain.Models;

namespace RWTree.Infrastructure.Parsing.RenderWare.Chunks;

public class FrameListChunk(ClumpChunk? parent, ChunkHeader header) : Chunk(parent, header)
{
    public List<ExtensionChunk> Extensions = [];
    public FrameListStructChunk FrameListStruct = null!;

    public NodeNameChunk? GetNodeNameChunk(int extensionIndex = 0)
    {
        var extension = Extensions[extensionIndex];

        if (extension.Header.Type == ChunkType.Extension)
            for (var chunkIndex = 0; chunkIndex < extension.Chunks.Count; chunkIndex++)
            {
                var chunk = extension.Chunks[chunkIndex];

                if (chunk?.Header.Type == ChunkType.NodeName) return (NodeNameChunk)chunk;
            }

        return null;
    }

    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine(
            $"FrameListChunk.Read: Reading frame list chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read frame list struct
        FrameListStruct = FrameListStructChunk.ReadFrameListStruct(binaryReader, this);

        // Read frame list extensions
        Extensions = new List<ExtensionChunk>();
        for (var extensionIndex = 0; extensionIndex < FrameListStruct.FrameCount; extensionIndex++)
        {
            var extension = ExtensionChunk.ReadExtension(binaryReader, this);
            if (extension != null)
                Extensions.Add(extension);
        }

        // Print debug message
        Console.WriteLine(
            $"FrameListChunk.Read: Read frame list chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    // <summary>
    // Reads frame list chunk from file access.
    // </summary>
    public static FrameListChunk ReadFrameList(BinaryReader fileAccess, ClumpChunk? parent)
    {
        // Read frame list header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // Handle expected type mismatch
        if (header.Type != ChunkType.FrameList)
            throw new Exception($"Expected chunk type '{ChunkType.FrameList}', got '{header.Type}'");

        // Create frame list chunk
        var frameList = new FrameListChunk(parent, header);

        // Read frame list chunk data
        frameList.Read(fileAccess);

        // Return frame list chunk
        return frameList;
    }

    protected override void AddChildChunks(ChunkNode node)
    {
        // Add frame list struct
        if (FrameListStruct != null)
        {
            var frameListStructNode = FrameListStruct.ToChunkNode();
            node.AddChild(frameListStructNode);
        }

        // Add extensions
        if (Extensions != null)
        {
            foreach (var extension in Extensions)
            {
                var extensionNode = extension.ToChunkNode();
                node.AddChild(extensionNode);
            }
        }
    }
}