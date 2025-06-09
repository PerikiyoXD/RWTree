using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using RWTree.Domain.Models;

namespace RWTree.Presentation.ViewModels;

/// <summary>
/// View model for chunk nodes in the tree view
/// </summary>
public sealed class ChunkNodeViewModel : INotifyPropertyChanged
{
    private readonly ChunkNode _model;
    private bool _isSelected;
    private bool _isExpanded;

    public ChunkNodeViewModel(ChunkNode model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));

        // Subscribe to model property changes
        _model.PropertyChanged += OnModelPropertyChanged;

        // Initialize children collection
        Children = new ObservableCollection<ChunkNodeViewModel>(
            _model.Children.Select(child => new ChunkNodeViewModel(child)));

        // Initialize properties collection
        Properties = new ObservableCollection<PropertyViewModel>(
            _model.Properties.Select(kvp => new PropertyViewModel(kvp.Key, kvp.Value)));

        // Sync initial state
        _isSelected = _model.IsSelected;
        _isExpanded = _model.IsExpanded;
    }

    public string Name => _model.Name;
    public string Type => _model.Type;

    // Properties for XAML binding
    public string DisplayName => _model.Name;
    public string ChunkType => _model.Type;
    public string Details => GenerateDetails();

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (SetProperty(ref _isSelected, value))
            {
                _model.IsSelected = value;
            }
        }
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (SetProperty(ref _isExpanded, value))
            {
                _model.IsExpanded = value;
            }
        }
    }

    public ObservableCollection<ChunkNodeViewModel> Children { get; }
    public ObservableCollection<PropertyViewModel> Properties { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ChunkNode.IsSelected):
                if (_isSelected != _model.IsSelected)
                {
                    _isSelected = _model.IsSelected;
                    OnPropertyChanged(nameof(IsSelected));
                }

                break;

            case nameof(ChunkNode.IsExpanded):
                if (_isExpanded != _model.IsExpanded)
                {
                    _isExpanded = _model.IsExpanded;
                    OnPropertyChanged(nameof(IsExpanded));
                }

                break;

            case nameof(ChunkNode.Children):
                UpdateChildren();
                break;
            case nameof(ChunkNode.Properties):
                UpdateProperties();
                OnPropertyChanged(nameof(Details));
                break;
        }
    }

    private void UpdateChildren()
    {
        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            Children.Clear();
            foreach (var child in _model.Children)
            {
                Children.Add(new ChunkNodeViewModel(child));
            }
        });
    }

    private void UpdateProperties()
    {
        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            Properties.Clear();
            foreach (var kvp in _model.Properties)
            {
                Properties.Add(new PropertyViewModel(kvp.Key, kvp.Value));
            }
        });
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string GenerateDetails()
    {
        var details = new System.Text.StringBuilder();
        details.AppendLine($"Chunk: {_model.Name}");
        details.AppendLine($"Type: {_model.Type}");
        details.AppendLine();

        if (_model.Properties.Any())
        {
            details.AppendLine("Properties:");
            foreach (var property in _model.Properties)
            {
                details.AppendLine($"  {property.Key}: {property.Value}");
            }
        }
        else
        {
            details.AppendLine("No properties available");
        }

        return details.ToString();
    }
}

/// <summary>
/// View model for chunk properties
/// </summary>
public sealed class PropertyViewModel
{
    public PropertyViewModel(string name, object value)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        ValueString = value.ToString() ?? string.Empty;
    }

    public string Name { get; }
    public object Value { get; }
    public string ValueString { get; }
}