using System.Collections.ObjectModel;

namespace RWTree;

public class TreeViewModel
{
    public ObservableCollection<TreeNode> ArbolRaiz { get; set; }

    public TreeViewModel()
    {
        // Inicializa y carga tus nodos en ArbolRaiz
    }
}