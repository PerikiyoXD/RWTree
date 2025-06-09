using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RWTree.Domain.Models;

/// <summary>
/// Domain model representing a hierarchical chunk node in a DFF file
/// </summary>
public sealed class ChunkNode : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _type = string.Empty;
    private bool _isSelected;
    private bool _isExpanded;
    private readonly List<ChunkNode> _children = new();
    private readonly Dictionary<string, object> _properties = new();

    public required string Name
    {
        get => _name;
        init => SetProperty(ref _name, value ?? throw new ArgumentNullException(nameof(value)));
    }

    public required string Type
    {
        get => _type;
        init => SetProperty(ref _type, value ?? throw new ArgumentNullException(nameof(value)));
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetProperty(ref _isExpanded, value);
    }

    public ChunkNode? Parent { get; private set; }

    public IReadOnlyList<ChunkNode> Children => _children.AsReadOnly();

    public IReadOnlyDictionary<string, object> Properties => _properties.AsReadOnly();

    public event PropertyChangedEventHandler? PropertyChanged;

    public void AddChild(ChunkNode child)
    {
        ArgumentNullException.ThrowIfNull(child, nameof(child));

        if (child.Parent is not null)
            throw new InvalidOperationException("Child already has a parent");

        child.Parent = this;
        _children.Add(child);
        OnPropertyChanged(nameof(Children));
    }

    public void RemoveChild(ChunkNode child)
    {
        ArgumentNullException.ThrowIfNull(child, nameof(child));

        if (_children.Remove(child))
        {
            child.Parent = null;
            OnPropertyChanged(nameof(Children));
        }
    }

    public void AddProperty(string key, object value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        _properties[key] = value;
        OnPropertyChanged(nameof(Properties));
    }

    public T? GetProperty<T>(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

        return _properties.TryGetValue(key, out var value) && value is T typedValue
            ? typedValue
            : default;
    }

    public bool HasProperty(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        return _properties.ContainsKey(key);
    }

    public IEnumerable<ChunkNode> GetDescendants()
    {
        foreach (var child in _children)
        {
            yield return child;
            foreach (var descendant in child.GetDescendants())
            {
                yield return descendant;
            }
        }
    }

    public ChunkNode? FindChild(Predicate<ChunkNode> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        foreach (var child in _children)
        {
            if (predicate(child))
                return child;

            var found = child.FindChild(predicate);
            if (found is not null)
                return found;
        }

        return null;
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