namespace RWTree.Domain.Exceptions;

/// <summary>
/// Base exception for all domain-related errors
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when a file operation fails
/// </summary>
public sealed class FileOperationException : DomainException
{
    public string? FilePath { get; }

    public FileOperationException(string message, string? filePath = null)
        : base(message)
    {
        FilePath = filePath;
    }

    public FileOperationException(string message, string? filePath, Exception innerException)
        : base(message, innerException)
    {
        FilePath = filePath;
    }
}

/// <summary>
/// Exception thrown when DFF file parsing fails
/// </summary>
public sealed class DffParsingException : DomainException
{
    public string? FilePath { get; }
    public long Position { get; }

    public DffParsingException(string message, string? filePath = null, long position = -1)
        : base(message)
    {
        FilePath = filePath;
        Position = position;
    }

    public DffParsingException(string message, string? filePath, long position, Exception innerException)
        : base(message, innerException)
    {
        FilePath = filePath;
        Position = position;
    }
}

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public sealed class ValidationException : DomainException
{
    public string PropertyName { get; }

    public ValidationException(string message, string propertyName) : base(message)
    {
        PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
    }
}