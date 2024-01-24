using System.IO;
using RWTree.Middleware.RenderWare.Stream.Chunks;

namespace RWTree.Middleware.RenderWare.Stream;

/// <summary>
///     Represents the header of a RenderWare stream chunk.
/// </summary>
public class ChunkHeader
{
    public uint Size = 0;
    public ChunkType Type = ChunkType.Unknown;
    public uint Version = 0;

    private void Read(BinaryReader fileAccess)
    {
        Type = (ChunkType)fileAccess.ReadUInt32();
        Size = fileAccess.ReadUInt32();
        Version = fileAccess.ReadUInt32();
    }

    private void Write(BinaryWriter fileAccess)
    {
        fileAccess.Write((uint)Type);
        fileAccess.Write(Size);
        fileAccess.Write(Version);
    }

    public static ChunkHeader ReadHeader(BinaryReader binaryReader)
    {
        Console.WriteLine("ChunkHeader.ReadHeader: Reading chunk header at position: '" + binaryReader.BaseStream.Position + "'");
        var header = new ChunkHeader();
        header.Read(binaryReader);
        Console.WriteLine($"ChunkHeader.ReadHeader: Read chunk header type: '{header.Type}' size: '{header.Size:X}' version: '{LibraryIdUtils.GetVersionString(header.Version)}'");
        return header;
    }

    public static void WriteHeader(BinaryWriter binaryWriter, ChunkHeader chunkHeader)
    {
        Console.WriteLine("ChunkHeader.WriteHeader: Writing chunk header at position: '" + binaryWriter.BaseStream.Position + "'");
        chunkHeader.Write(binaryWriter);
        Console.WriteLine($"ChunkHeader.WriteHeader: Wrote chunk header type: '{chunkHeader.Type}' size: '{chunkHeader.Size:X}' version: '{LibraryIdUtils.GetVersionString(chunkHeader.Version)}'");
    }
}