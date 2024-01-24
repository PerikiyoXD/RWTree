using System.IO;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class CollisionModelChunk(Chunk? parent, ChunkHeader header) : Chunk(parent, header)
{
    public override void Read(BinaryReader binaryReader)
    {
        Console.WriteLine($"CollisionModelChunk.Read: Reading collision model chunk at position: '{binaryReader.BaseStream.Position}'");

        base.Read(binaryReader);

        // TODO: Implement Collision Model parsing
        {
            Console.WriteLine("CollisionModelChunk.Read: Collision model chunk data is not implemented yet, skipping...");

            // Advance to the end of the chunk
            binaryReader.BaseStream.Seek(StartPosition + Header.Size, SeekOrigin.Begin);
        }

        Console.WriteLine($"CollisionModelChunk.Read: Read collision model chunk up to position: '{binaryReader.BaseStream.Position}'");
    }
}