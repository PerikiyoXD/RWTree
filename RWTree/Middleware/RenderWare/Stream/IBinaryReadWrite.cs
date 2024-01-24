using System.IO;

namespace RWTree.Middleware.RenderWare.Stream;

public interface IBinaryReadWrite
{
    public void Read(BinaryReader binaryReader);
    public void Write(BinaryWriter binaryWriter);
}