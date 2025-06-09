using System.IO;

namespace RWTree.Infrastructure.Parsing.RenderWare;

public interface IBinaryReadWrite
{
    public void Read(BinaryReader binaryReader);
    public void Write(BinaryWriter binaryWriter);
}