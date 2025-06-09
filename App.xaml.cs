using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RWTree.Infrastructure.DependencyInjection;
using RWTree.Presentation.ViewModels;

namespace RWTree;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Configure dependency injection
        var services = new ServiceCollection();
        services.AddRWTreeServices();
        _serviceProvider = services.BuildServiceProvider();

        // Create and show main window with dependency injection
        var mainWindow = new MainWindow();
        var viewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
        mainWindow.DataContext = viewModel;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}