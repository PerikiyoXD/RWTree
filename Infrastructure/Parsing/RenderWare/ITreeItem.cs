using System.Windows.Controls;

namespace RWTree.Infrastructure.Parsing.RenderWare;

public interface ITreeItem
{
    public TreeViewItem ToTreeViewItem();
}