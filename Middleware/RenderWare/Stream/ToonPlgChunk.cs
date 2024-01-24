using System.IO;
using System.Text;
using System.Windows.Controls;

namespace RWTree.Middleware.RenderWare.Stream;

public class ToonPlgChunk(Chunk? parent, ChunkHeader header) : Chunk(parent, header)
{
    public uint Version;
    public uint Enabled;
    public /* String of 32 bytes */ byte[] Name = new byte[32];
    public uint SilhouetteInkId;
    
    public override void Read(BinaryReader binaryReader)
    {
        // Print debug message
        Console.WriteLine($"HAnimPLGChunk.Read: Reading HAnimPLG chunk at position: '{binaryReader.BaseStream.Position}'");

        // Call base class method
        base.Read(binaryReader);

        // Read chunk data
        Version = binaryReader.ReadUInt32();
        Enabled = binaryReader.ReadUInt32();
        Name = binaryReader.ReadBytes(32);
        SilhouetteInkId = binaryReader.ReadUInt32();
        
        // Probably there's more data, seek to the end of the chunk
        binaryReader.BaseStream.Seek(StartPosition + Header.Size + 12, SeekOrigin.Begin);

        // Print debug message
        Console.WriteLine($"HAnimPLGChunk.Read: Read HAnimPLG chunk up to position: '{binaryReader.BaseStream.Position}'");
    }

    public override void Write(BinaryWriter binaryWriter)
    {
        throw new NotImplementedException();
    }
    
    public new TreeViewItem ToTreeViewItem()
    {
        var treeViewItem = base.ToTreeViewItem();
        
            treeViewItem.Items.Add(new TreeViewItem
            {
                Header = $"Version: {Version}"
            });
            
            treeViewItem.Items.Add(new TreeViewItem
            {
                Header = $"Enabled: {Enabled}"
            });
            
            treeViewItem.Items.Add(new TreeViewItem
            {
                Header = $"Name: {Encoding.ASCII.GetString(Name)}"
            });
            
            treeViewItem.Items.Add(new TreeViewItem
            {
                Header = $"SilhouetteInkId: {SilhouetteInkId}"
            });
    
            return treeViewItem;
        }
}