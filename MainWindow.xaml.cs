using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using RWTree.Middleware.RenderWare;

namespace RWTree;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    public void Window_Drop(object sender, DragEventArgs e)
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
    public void Window_DragOver(object sender, DragEventArgs e)
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
    public void LoadFile(string filePath)
    {
        Console.WriteLine($"MainWindow.LoadFile: Loading file: '{filePath}'");
        
        DffReader dffReader = new();
        
        dffReader.Read(filePath);
        
        // Get tree items from dffReader
        var tree = dffReader.Dff.ToTreeViewItem();
        
        // Set tree
        this.NodeTree.ItemsSource = tree.Items;
        
        // Debugger.Break();
        // Set tree
        // this.NodeTree.ItemsSource = tree.Nodes; // NOT YET
    }

    private void btnOpen_Click(object sender, RoutedEventArgs e)
    {
        // Trigger open file dialog
        OpenFileDialog openFileDialog = new() { Filter = "RenderWare DFF Files (*.dff)|*.dff", FilterIndex = 1, Multiselect = false };

        // Show dialog
        bool? result = openFileDialog.ShowDialog();
        
        // Check if result is true
        if (result == true)
        {
            // Get file path
            string filePath = openFileDialog.FileName;
            
            // Call load
            LoadFile(filePath);
        }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void btnEdit_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}