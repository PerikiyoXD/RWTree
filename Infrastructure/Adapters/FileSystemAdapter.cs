using System.IO;
using RWTree.Application.Ports;
using RWTree.Domain.Exceptions;

namespace RWTree.Infrastructure.Adapters;

/// <summary>
/// File system adapter implementing defensive programming
/// </summary>
public sealed class FileSystemAdapter : IFileSystemPort
{
    public async Task<bool> FileExistsAsync(string filePath)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

            return await Task.Run(() => File.Exists(filePath));
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            // Defensive: Don't let file system errors bubble up as domain exceptions but log them
            return false;
        }
    }

    public async Task<Stream> OpenReadStreamAsync(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

        try
        {
            if (!await FileExistsAsync(filePath))
            {
                throw new FileOperationException($"File not found: {filePath}", filePath);
            }

            return await Task.Run(() =>
            {
                try
                {
                    return (Stream)new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new FileOperationException($"Access denied to file: {filePath}", filePath, ex);
                }
                catch (IOException ex)
                {
                    throw new FileOperationException($"IO error reading file: {filePath}", filePath, ex);
                }
            });
        }
        catch (Exception ex) when (ex is not FileOperationException)
        {
            throw new FileOperationException($"Unexpected error opening file: {filePath}", filePath, ex);
        }
    }

    public async Task<DateTime> GetLastModifiedAsync(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

        try
        {
            if (!await FileExistsAsync(filePath))
            {
                throw new FileOperationException($"File not found: {filePath}", filePath);
            }

            return await Task.Run(() => File.GetLastWriteTimeUtc(filePath));
        }
        catch (Exception ex) when (ex is not FileOperationException)
        {
            throw new FileOperationException($"Error getting file modified time: {filePath}", filePath, ex);
        }
    }

    public async Task<long> GetFileSizeAsync(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

        try
        {
            if (!await FileExistsAsync(filePath))
            {
                throw new FileOperationException($"File not found: {filePath}", filePath);
            }

            return await Task.Run(() => new FileInfo(filePath).Length);
        }
        catch (Exception ex) when (ex is not FileOperationException)
        {
            throw new FileOperationException($"Error getting file size: {filePath}", filePath, ex);
        }
    }

    public bool IsValidPath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        try
        {
            // Check for invalid characters
            var invalidChars = Path.GetInvalidPathChars();
            if (filePath.IndexOfAny(invalidChars) >= 0)
                return false;

            // Try to get full path to validate format
            var fullPath = Path.GetFullPath(filePath);
            return !string.IsNullOrEmpty(fullPath);
        }
        catch
        {
            return false;
        }
    }
}