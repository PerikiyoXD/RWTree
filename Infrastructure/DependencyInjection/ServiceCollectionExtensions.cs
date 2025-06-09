using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RWTree.Application.Ports;
using RWTree.Application.UseCases;
using RWTree.Infrastructure.Adapters;
using RWTree.Presentation.ViewModels;

namespace RWTree.Infrastructure.DependencyInjection;

/// <summary>
/// Dependency injection container configuration for hexagonal architecture
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRWTreeServices(this IServiceCollection services)
    {
        // Register ports (interfaces)
        services.AddSingleton<ILoggingPort, ConsoleLoggingAdapter>();
        services.AddSingleton<IFileSystemPort, FileSystemAdapter>();
        services.AddSingleton<IFileDialogPort, WpfFileDialogAdapter>();
        services.AddSingleton<INotificationPort, WpfNotificationAdapter>();
        services.AddTransient<IDffParserPort, DffParserAdapter>();

        // Register use cases (application layer)
        services.AddTransient<LoadDffFileUseCase>();
        services.AddTransient<FileDialogUseCase>();
        services.AddTransient<ClearDocumentUseCase>();

        // Register view models (presentation layer)
        services.AddTransient<MainWindowViewModel>();

        return services;
    }

    public static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) => { services.AddRWTreeServices(); });
}