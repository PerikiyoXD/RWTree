using System.Collections.ObjectModel;

namespace RWTree;

public class TreeNode
{
    public string Nombre { get; set; }
    public ObservableCollection<TreeNode> Hijos { get; set; }

    public TreeNode(string nombre)
    {
        Nombre = nombre;
        Hijos = new ObservableCollection<TreeNode>();
    }
}