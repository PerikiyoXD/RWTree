using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using RWTree.Middleware.RenderWare;

namespace RWTree;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public Dff? Dff { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new TreeViewModel();
    }

    public void WindowDrop(object sender, DragEventArgs e)
    {
        // Check if file is dragged
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            // Get file path
            object? dropData = e.Data.GetData(DataFormats.FileDrop);
            if (dropData is string[] files)
            {
                // Get file path
                string filePath = files[0];

                // Call load
                LoadFile(filePath);
            }
        }

        // Mark event as handled
        e.Handled = true;
    }

    // On drag on window
    private void Window_DragOver(object sender, DragEventArgs e)
    {
        // Check if file is dragged
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            // Allow copy
            e.Effects = DragDropEffects.Copy;

            // Get file path
            object? dropData = e.Data.GetData(DataFormats.FileDrop);
            if (dropData is string[] files)
            {
                // Get file path
                string filePath = files[0];

                // Call load
                LoadFile(filePath);
            }
        }
        else
        {
            // Deny drop
            e.Effects = DragDropEffects.None;
        }

        // Mark event as handled
        e.Handled = true;
    }

    // Load file
    private void LoadFile(string filePath)
    {
        Console.WriteLine($"MainWindow.LoadFile: Loading file: '{filePath}'");

        DffReader dffReader = new();

        dffReader.Read(filePath);

        Dff = dffReader.Dff;
        
        var tree = Dff.ToTreeViewItem();

        this.NodeTree.ItemsSource = tree.Items;
    }

    private void OpenClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new() { Filter = "RenderWare DFF Files (*.dff)|*.dff", FilterIndex = 1, Multiselect = false };

        bool? result = openFileDialog.ShowDialog();

        if (result == true)
        {
            string filePath = openFileDialog.FileName;

            LoadFile(filePath);
        }
    }
    
    private void ExitClick(object sender, RoutedEventArgs e)
    {
        // Clear tree
        this.NodeTree.ItemsSource = null;
            
        // Cleanup
        Dff = null;
        
        Application.Current.Shutdown();
    }

    private void CloseClick(object sender, RoutedEventArgs e)
    {
        // Clear tree
        this.NodeTree.ItemsSource = null;
        
        // Cleanup
        Dff = null;
    }

    private void NodeTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        Console.WriteLine($"MainWindow.NodeTree_SelectedItemChanged: Selected item changed to: '{e.NewValue}'");
        
        if (e.OldValue is TreeViewItem oldItem)
            oldItem.Background = null;
        
        if (e.NewValue is TreeViewItem newItem)
            newItem.Background = Brushes.LightBlue;
    }
}

public class StreamChunkTree
{
}