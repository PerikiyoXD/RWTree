using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace RWTree.Domain.Models;

/// <summary>
/// Domain model representing a RenderWare DFF document
/// </summary>
public sealed class DffDocument : INotifyPropertyChanged
{
    private string? _filePath;
    private string? _fileName;
    private DateTime _lastModified;
    private bool _isLoaded;
    private readonly List<ChunkNode> _chunks = new();

    public string? FilePath
    {
        get => _filePath;
        private set => SetProperty(ref _filePath, value);
    }

    public string? FileName
    {
        get => _fileName;
        private set => SetProperty(ref _fileName, value);
    }

    public DateTime LastModified
    {
        get => _lastModified;
        private set => SetProperty(ref _lastModified, value);
    }

    public bool IsLoaded
    {
        get => _isLoaded;
        private set => SetProperty(ref _isLoaded, value);
    }

    public IReadOnlyList<ChunkNode> Chunks => _chunks.AsReadOnly();

    public event PropertyChangedEventHandler? PropertyChanged;

    public static DffDocument CreateEmpty()
    {
        return new DffDocument();
    }

    public static DffDocument CreateFromFile(string filePath, IEnumerable<ChunkNode> chunks)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));
        ArgumentNullException.ThrowIfNull(chunks, nameof(chunks));

        var document = new DffDocument
        {
            FilePath = filePath,
            FileName = Path.GetFileName(filePath),
            LastModified = DateTime.UtcNow,
            IsLoaded = true
        };

        document._chunks.AddRange(chunks);
        return document;
    }

    public void UpdateContent(IEnumerable<ChunkNode> chunks)
    {
        ArgumentNullException.ThrowIfNull(chunks, nameof(chunks));

        _chunks.Clear();
        _chunks.AddRange(chunks);
        LastModified = DateTime.UtcNow;
        OnPropertyChanged(nameof(Chunks));
    }

    public void ClearContent()
    {
        _chunks.Clear();
        FilePath = null;
        FileName = null;
        IsLoaded = false;
        OnPropertyChanged(nameof(Chunks));
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;

        field = value;
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}