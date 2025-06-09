using Microsoft.Win32;
using RWTree.Application.Ports;

namespace RWTree.Infrastructure.Adapters;

/// <summary>
/// WPF-based file dialog adapter
/// </summary>
public sealed class WpfFileDialogAdapter : IFileDialogPort
{
    public async Task<string?> ShowOpenFileDialogAsync(string filter, string title = "Open File")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filter, nameof(filter));
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));

        // Ensure we're on the UI thread for dialog operations
        if (System.Windows.Application.Current?.Dispatcher.CheckAccess() == true)
        {
            return ShowOpenFileDialogCore(filter, title);
        }
        else
        {
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher != null)
            {
                return await dispatcher.InvokeAsync(() => ShowOpenFileDialogCore(filter, title));
            }

            return null;
        }
    }

    private string? ShowOpenFileDialogCore(string filter, string title)
    {
        try
        {
            var dialog = new OpenFileDialog
            {
                Filter = filter,
                Title = title,
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true
            };

            // Get the main window as owner
            var mainWindow = System.Windows.Application.Current?.MainWindow;
            var result = dialog.ShowDialog(mainWindow);
            return result == true ? dialog.FileName : null;
        }
        catch (Exception)
        {
            // Defensive: Return null if dialog fails
            return null;
        }
    }

    public async Task<string?> ShowSaveFileDialogAsync(string filter, string title = "Save File",
        string? defaultFileName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filter, nameof(filter));
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));

        // Ensure we're on the UI thread for dialog operations
        if (System.Windows.Application.Current?.Dispatcher.CheckAccess() == true)
        {
            return ShowSaveFileDialogCore(filter, title, defaultFileName);
        }
        else
        {
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher != null)
            {
                return await dispatcher.InvokeAsync(() => ShowSaveFileDialogCore(filter, title, defaultFileName));
            }

            return null;
        }
    }

    private string? ShowSaveFileDialogCore(string filter, string title, string? defaultFileName)
    {
        try
        {
            var dialog = new SaveFileDialog
            {
                Filter = filter,
                Title = title,
                FileName = defaultFileName ?? string.Empty,
                CheckPathExists = true
            };

            // Get the main window as owner
            var mainWindow = System.Windows.Application.Current?.MainWindow;
            var result = dialog.ShowDialog(mainWindow);
            return result == true ? dialog.FileName : null;
        }
        catch (Exception)
        {
            // Defensive: Return null if dialog fails
            return null;
        }
    }
}