using System.Windows.Controls;

namespace RWTree.Middleware.RenderWare.Stream;

public interface ITreeItem
{
    public TreeViewItem ToTreeViewItem();
}