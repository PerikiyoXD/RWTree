using RWTree.Application.Ports;
using RWTree.Domain.Models;

namespace RWTree.Application.UseCases;

/// <summary>
/// Use case for file dialog operations
/// </summary>
public sealed class FileDialogUseCase
{
    private readonly IFileDialogPort _fileDialogPort;
    private readonly ILoggingPort _loggingPort;

    public FileDialogUseCase(IFileDialogPort fileDialogPort, ILoggingPort loggingPort)
    {
        _fileDialogPort = fileDialogPort ?? throw new ArgumentNullException(nameof(fileDialogPort));
        _loggingPort = loggingPort ?? throw new ArgumentNullException(nameof(loggingPort));
    }

    public async Task<string?> SelectDffFileAsync()
    {
        try
        {
            _loggingPort.LogDebug("Opening file dialog for DFF file selection");

            const string filter = "RenderWare DFF Files (*.dff)|*.dff|All Files (*.*)|*.*";
            var result = await _fileDialogPort.ShowOpenFileDialogAsync(filter, "Open DFF File");

            if (result != null)
            {
                _loggingPort.LogInformation("User selected file: {FilePath}", result);
            }
            else
            {
                _loggingPort.LogDebug("User cancelled file selection");
            }

            return result;
        }
        catch (Exception ex)
        {
            _loggingPort.LogError("Error opening file dialog", ex);
            return null;
        }
    }
}

/// <summary>
/// Use case for clearing/closing document
/// </summary>
public sealed class ClearDocumentUseCase
{
    private readonly ILoggingPort _loggingPort;

    public ClearDocumentUseCase(ILoggingPort loggingPort)
    {
        _loggingPort = loggingPort ?? throw new ArgumentNullException(nameof(loggingPort));
    }

    public void Execute(DffDocument document)
    {
        ArgumentNullException.ThrowIfNull(document, nameof(document));

        try
        {
            _loggingPort.LogInformation("Clearing document: {FilePath}", document.FilePath ?? "Untitled");

            document.ClearContent();

            _loggingPort.LogInformation("Document cleared successfully");
        }
        catch (Exception ex)
        {
            _loggingPort.LogError("Error clearing document", ex);
            throw;
        }
    }
}