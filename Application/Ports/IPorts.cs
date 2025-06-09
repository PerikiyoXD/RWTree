using System.IO;
using RWTree.Domain.Models;

namespace RWTree.Application.Ports;

/// <summary>
/// Port for file loading operations (Inbound Port)
/// </summary>
public interface IFileLoaderPort
{
    Task<DffDocument> LoadDffFileAsync(string filePath, CancellationToken cancellationToken = default);
    bool CanLoadFile(string filePath);
    IEnumerable<string> GetSupportedExtensions();
}

/// <summary>
/// Port for file system operations (Outbound Port)
/// </summary>
public interface IFileSystemPort
{
    Task<bool> FileExistsAsync(string filePath);
    Task<Stream> OpenReadStreamAsync(string filePath);
    Task<DateTime> GetLastModifiedAsync(string filePath);
    Task<long> GetFileSizeAsync(string filePath);
    bool IsValidPath(string filePath);
}

/// <summary>
/// Port for DFF parsing operations (Outbound Port)
/// </summary>
public interface IDffParserPort
{
    Task<IEnumerable<ChunkNode>> ParseAsync(Stream stream, CancellationToken cancellationToken = default);
    bool CanParse(Stream stream);
    void ValidateFormat(Stream stream);
}

/// <summary>
/// Port for logging operations (Outbound Port)
/// </summary>
public interface ILoggingPort
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(string message, Exception? exception = null, params object[] args);
    void LogDebug(string message, params object[] args);
}

/// <summary>
/// Port for UI notifications (Outbound Port)
/// </summary>
public interface INotificationPort
{
    void ShowInformation(string title, string message);
    void ShowWarning(string title, string message);
    void ShowError(string title, string message);
    Task<bool> ShowConfirmationAsync(string title, string message);
}

/// <summary>
/// Port for file dialog operations (Outbound Port)
/// </summary>
public interface IFileDialogPort
{
    Task<string?> ShowOpenFileDialogAsync(string filter, string title = "Open File");
    Task<string?> ShowSaveFileDialogAsync(string filter, string title = "Save File", string? defaultFileName = null);
}