using System.IO;
using System.Numerics;
using System.Windows.Media.Media3D;

namespace RWTree.Middleware.RenderWare.Stream.Chunks;

public class FrameListStructChunk(FrameListChunk? parent, ChunkHeader header) : Chunk(parent, header)
{
    public int FrameCount;
    public List<FrameData> Frames;

    // Read frame list struct chunk data
    // FrameListStructs does not have sub-chunks
    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine($"FrameListStructChunk.Read: Reading frame list struct chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read frame count
        FrameCount = (int)binaryReader.ReadUInt32();

        Frames = new List<FrameData>();

        ReadFramesData(binaryReader);

        // Print debug message
        Console.WriteLine($"FrameListStructChunk.Read: Read frame list struct chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    private void ReadFrameData(BinaryReader fileAccess)
    {
        // Read 4 vectors: right, up, at, position
        Vector3 right = new Vector3(fileAccess.ReadSingle(), fileAccess.ReadSingle(), fileAccess.ReadSingle());
        Vector3 up = new Vector3(fileAccess.ReadSingle(), fileAccess.ReadSingle(), fileAccess.ReadSingle());
        Vector3 at = new Vector3(fileAccess.ReadSingle(), fileAccess.ReadSingle(), fileAccess.ReadSingle());
        Vector3 position = new Vector3(fileAccess.ReadSingle(), fileAccess.ReadSingle(), fileAccess.ReadSingle());

        // Read parent index, int
        var parentIndex = (int)fileAccess.ReadUInt32();

        // Read matrix flags, int
        var matrixFlags = (int)fileAccess.ReadUInt32();

        FrameData frame = new FrameData();

        // Set transform values
        Matrix3D transform = new()
        {
            OffsetX = position.X,
            OffsetY = position.Y,
            OffsetZ = position.Z,
            M11 = right.X,
            M12 = right.Y,
            M13 = right.Z,
            M21 = up.X,
            M22 = up.Y,
            M23 = up.Z,
            M31 = at.X,
            M32 = at.Y,
            M33 = at.Z,
            M44 = 1
        };

        // Usually, the models are looking up, so we rotate them 90 degrees on the X axis
        // transform = transform.Rotated(new Vector3(1, 0, 0), 90 * 2 * Mathf.Pi / 360);


        frame.Transform = transform;


        frame.ParentIndex = parentIndex;
        frame.Flags = matrixFlags;

        // Add frame to list
        Frames.Add(frame);
    }

    private void ReadFramesData(BinaryReader fileAccess)
    {
        for (var i = 0; i < FrameCount; i++) ReadFrameData(fileAccess);
    }

    // <summary>
    // Read frame list struct chunk from file access.
    // </summary>
    public static FrameListStructChunk ReadFrameListStruct(BinaryReader fileAccess, FrameListChunk? parent)
    {
        // Read frame list struct header
        var header = ChunkHeader.ReadHeader(fileAccess);

        // Handle expected type mismatch
        if (header.Type != ChunkType.Struct)
            throw new Exception($"Expected chunk type '{ChunkType.Struct.ToString()}', got '{header.Type}'");

        // Create frame list struct chunk
        var chunk = new FrameListStructChunk(parent, header);

        // Read frame list struct chunk data
        chunk.Read(fileAccess);

        return chunk;
    }

    public class FrameData
    {
        public int Flags;
        public int ParentIndex;
        public Matrix3D Transform;
    }
}