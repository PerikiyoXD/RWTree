
using System.IO;
using RWTree.Middleware.RenderWare.Stream;
using RWTree.Middleware.RenderWare.Stream.Chunks;

namespace RWTree.Middleware.RenderWare;

public class DffReader
{
    public Dff Dff = new();

    public void Read(string path)
    {
        // Print debug message
        Console.WriteLine($"DffReader.Read: Reading DFF file at path: '{path}'");

        // Open file stream
        using var fileStream = new FileStream(path, FileMode.Open);

        // Create binary reader
        using var binaryReader = new BinaryReader(fileStream);

        // Read file header
        Dff.Read(binaryReader);

        return;
    }
}