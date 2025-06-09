using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RWTree;

/// <summary>
/// Interaction logic for MainWindow.xaml - Now follows proper MVVM pattern
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // DataContext will be set by App.xaml.cs through dependency injection
    }

    /// <summary>
    /// Handle file drop events and delegate to ViewModel
    /// </summary>
    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
            e.Data.GetData(DataFormats.FileDrop) is string[] files &&
            files.Length > 0)
        {
            // Delegate to ViewModel command
            if (DataContext is Presentation.ViewModels.MainWindowViewModel viewModel &&
                viewModel.LoadFileCommand.CanExecute(files[0]))
            {
                viewModel.LoadFileCommand.Execute(files[0]);
            }
        }

        e.Handled = true;
    }

    /// <summary>
    /// Handle drag over events for visual feedback
    /// </summary>
    private void Window_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }

        e.Handled = true;
    }

    /// <summary>
    /// Handle tree view selection changes and delegate to ViewModel
    /// </summary>
    private void NodeTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        // Delegate to ViewModel
        if (DataContext is Presentation.ViewModels.MainWindowViewModel viewModel &&
            e.NewValue is Presentation.ViewModels.ChunkNodeViewModel selectedNode)
        {
            viewModel.SelectedChunkNode = selectedNode;
        }

        e.Handled = true;
    }
}