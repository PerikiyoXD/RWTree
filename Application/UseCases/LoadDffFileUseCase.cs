using RWTree.Application.Ports;
using RWTree.Domain.Exceptions;
using RWTree.Domain.Models;

namespace RWTree.Application.UseCases;

/// <summary>
/// Use case for loading DFF files with defensive programming practices
/// </summary>
public sealed class LoadDffFileUseCase
{
    private readonly IFileSystemPort _fileSystemPort;
    private readonly IDffParserPort _dffParserPort;
    private readonly ILoggingPort _loggingPort;
    private readonly INotificationPort _notificationPort;

    public LoadDffFileUseCase(
        IFileSystemPort fileSystemPort,
        IDffParserPort dffParserPort,
        ILoggingPort loggingPort,
        INotificationPort notificationPort)
    {
        _fileSystemPort = fileSystemPort ?? throw new ArgumentNullException(nameof(fileSystemPort));
        _dffParserPort = dffParserPort ?? throw new ArgumentNullException(nameof(dffParserPort));
        _loggingPort = loggingPort ?? throw new ArgumentNullException(nameof(loggingPort));
        _notificationPort = notificationPort ?? throw new ArgumentNullException(nameof(notificationPort));
    }

    public async Task<Result<DffDocument>> ExecuteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            // Defensive programming: Validate input
            if (string.IsNullOrWhiteSpace(filePath))
            {
                var error = "File path cannot be null or empty";
                _loggingPort.LogWarning(error);
                return Result<DffDocument>.Failure(error);
            }

            // Defensive programming: Validate file path format
            if (!_fileSystemPort.IsValidPath(filePath))
            {
                var error = $"Invalid file path format: {filePath}";
                _loggingPort.LogWarning(error);
                return Result<DffDocument>.Failure(error);
            }

            _loggingPort.LogInformation("Starting to load DFF file: {FilePath}", filePath);

            // Defensive programming: Check if file exists
            if (!await _fileSystemPort.FileExistsAsync(filePath))
            {
                var error = $"File not found: {filePath}";
                _loggingPort.LogWarning(error);
                return Result<DffDocument>.Failure(error);
            }

            // Defensive programming: Check file size (prevent loading huge files)
            var fileSize = await _fileSystemPort.GetFileSizeAsync(filePath);
            if (fileSize > 100 * 1024 * 1024) // 100MB limit
            {
                var error = $"File too large: {fileSize} bytes. Maximum allowed: 100MB";
                _loggingPort.LogWarning(error);
                return Result<DffDocument>.Failure(error);
            }

            // Load file content
            using var stream = await _fileSystemPort.OpenReadStreamAsync(filePath);

            // Defensive programming: Validate stream
            if (stream == null || !stream.CanRead)
            {
                var error = "Unable to read file stream";
                _loggingPort.LogError(error);
                return Result<DffDocument>.Failure(error);
            }

            // Defensive programming: Validate DFF format
            try
            {
                _dffParserPort.ValidateFormat(stream);
            }
            catch (DffParsingException ex)
            {
                _loggingPort.LogError("Invalid DFF file format", ex);
                return Result<DffDocument>.Failure($"Invalid DFF file format: {ex.Message}");
            }

            // Reset stream position after validation
            stream.Position = 0;

            // Parse file content
            var chunks = await _dffParserPort.ParseAsync(stream, cancellationToken);

            // Defensive programming: Validate parsed content
            if (chunks == null)
            {
                var error = "Failed to parse file content";
                _loggingPort.LogError(error);
                return Result<DffDocument>.Failure(error);
            }

            var chunkList = chunks.ToList();
            if (chunkList.Count == 0)
            {
                var error = "No chunks found in file";
                _loggingPort.LogWarning(error);
                return Result<DffDocument>.Failure(error);
            }

            // Create document
            var document = DffDocument.CreateFromFile(filePath, chunkList);

            _loggingPort.LogInformation("Successfully loaded DFF file: {FilePath} with {ChunkCount} chunks",
                filePath, chunkList.Count);

            return Result<DffDocument>.Success(document);
        }
        catch (OperationCanceledException)
        {
            _loggingPort.LogInformation("File loading was cancelled: {FilePath}", filePath);
            return Result<DffDocument>.Failure("Operation was cancelled");
        }
        catch (FileOperationException ex)
        {
            _loggingPort.LogError("File operation failed", ex, filePath);
            return Result<DffDocument>.Failure($"File operation failed: {ex.Message}");
        }
        catch (DffParsingException ex)
        {
            _loggingPort.LogError("DFF parsing failed", ex, filePath);
            return Result<DffDocument>.Failure($"DFF parsing failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _loggingPort.LogError("Unexpected error while loading file", ex, filePath);
            return Result<DffDocument>.Failure($"Unexpected error: {ex.Message}");
        }
    }
}

/// <summary>
/// Result pattern for defensive programming
/// </summary>
public sealed class Result<T>
{
    public bool IsSuccess { get; private init; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; private init; }
    public string Error { get; private init; } = string.Empty;

    private Result()
    {
    }

    public static Result<T> Success(T value) => new()
    {
        IsSuccess = true,
        Value = value
    };

    public static Result<T> Failure(string error) => new()
    {
        IsSuccess = false,
        Error = error ?? throw new ArgumentNullException(nameof(error))
    };

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));

        return IsSuccess && Value is not null
            ? onSuccess(Value)
            : onFailure(Error);
    }
}