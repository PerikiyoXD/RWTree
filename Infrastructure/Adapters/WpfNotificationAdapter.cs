using System.Windows;
using RWTree.Application.Ports;

namespace RWTree.Infrastructure.Adapters;

/// <summary>
/// WPF-based notification adapter using MessageBox
/// </summary>
public sealed class WpfNotificationAdapter : INotificationPort
{
    public void ShowInformation(string title, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        });
    }

    public void ShowWarning(string title, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        });
    }

    public void ShowError(string title, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        });
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

        return await Task.Run(() =>
        {
            return System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            }) ?? false;
        });
    }
}