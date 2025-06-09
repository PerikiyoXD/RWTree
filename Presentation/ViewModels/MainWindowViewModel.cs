using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RWTree.Application.UseCases;
using RWTree.Domain.Models;
using RWTree.Presentation.Commands;

namespace RWTree.Presentation.ViewModels;

/// <summary>
/// Main window view model implementing MVVM pattern with defensive programming
/// </summary>
public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly LoadDffFileUseCase _loadDffFileUseCase;
    private readonly FileDialogUseCase _fileDialogUseCase;
    private readonly ClearDocumentUseCase _clearDocumentUseCase;

    private DffDocument? _currentDocument;
    private ObservableCollection<ChunkNodeViewModel> _chunkNodes = new();
    private ChunkNodeViewModel? _selectedChunkNode;
    private bool _isLoading;
    private string _statusMessage = "Ready";
    public bool HasDocument => _currentDocument != null;

    public MainWindowViewModel(
        LoadDffFileUseCase loadDffFileUseCase,
        FileDialogUseCase fileDialogUseCase,
        ClearDocumentUseCase clearDocumentUseCase)
    {
        _loadDffFileUseCase = loadDffFileUseCase ?? throw new ArgumentNullException(nameof(loadDffFileUseCase));
        _fileDialogUseCase = fileDialogUseCase ?? throw new ArgumentNullException(nameof(fileDialogUseCase));
        _clearDocumentUseCase = clearDocumentUseCase ?? throw new ArgumentNullException(nameof(clearDocumentUseCase));

        // Initialize commands
        OpenFileCommand = new AsyncRelayCommand(OpenFileAsync, () => !IsLoading);
        CloseFileCommand = new RelayCommand(CloseFile, () => CurrentDocument != null && !IsLoading);
        ExitCommand = new RelayCommand(Exit);
        LoadFileCommand = new AsyncRelayCommand<string>(LoadFileAsync, _ => !IsLoading);
    }

    public DffDocument? CurrentDocument
    {
        get => _currentDocument;
        private set
        {
            if (SetProperty(ref _currentDocument, value))
            {
                OnPropertyChanged(nameof(HasDocument));
                OnPropertyChanged(nameof(DocumentName));
            }
        }
    }

    public ObservableCollection<ChunkNodeViewModel> ChunkNodes
    {
        get => _chunkNodes;
        private set => SetProperty(ref _chunkNodes, value);
    }

    public ChunkNodeViewModel? SelectedChunkNode
    {
        get => _selectedChunkNode;
        set => SetProperty(ref _selectedChunkNode, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (SetProperty(ref _isLoading, value))
            {
                // Refresh command states when loading changes
                ((AsyncRelayCommand)OpenFileCommand).NotifyCanExecuteChanged();
                ((RelayCommand)CloseFileCommand).NotifyCanExecuteChanged();
                ((AsyncRelayCommand<string>)LoadFileCommand).NotifyCanExecuteChanged();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public string? DocumentName => CurrentDocument != null ? Path.GetFileName(CurrentDocument.FilePath) : null;

    public ICommand OpenFileCommand { get; }
    public ICommand CloseFileCommand { get; }
    public ICommand ExitCommand { get; }
    public ICommand LoadFileCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private async Task OpenFileAsync()
    {
        try
        {
            var filePath = await _fileDialogUseCase.SelectDffFileAsync();
            if (!string.IsNullOrEmpty(filePath))
            {
                await LoadFileAsync(filePath);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error opening file: {ex.Message}";
        }
    }

    private async Task LoadFileAsync(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));
        try
        {
            IsLoading = true;
            StatusMessage = $"Loading {Path.GetFileName(filePath)}...";

            var result = await _loadDffFileUseCase.ExecuteAsync(filePath);

            await result.Match(
                onSuccess: async document =>
                {
                    CurrentDocument = document;
                    await UpdateChunkNodesAsync(document);
                    StatusMessage = $"Loaded {Path.GetFileName(filePath)} successfully";
                },
                onFailure: error =>
                {
                    StatusMessage = $"Failed to load file: {error}";
                    return Task.CompletedTask;
                });
        }
        catch (Exception ex)
        {
            StatusMessage = $"Unexpected error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task UpdateChunkNodesAsync(DffDocument document)
    {
        ArgumentNullException.ThrowIfNull(document, nameof(document));

        try
        {
            await Task.Run(() =>
            {
                var viewModels = document.Chunks
                    .Select(chunk => new ChunkNodeViewModel(chunk))
                    .ToList();

                System.Windows.Application.Current?.Dispatcher.Invoke(() =>
                {
                    ChunkNodes.Clear();
                    foreach (var viewModel in viewModels)
                    {
                        ChunkNodes.Add(viewModel);
                    }
                });
            });
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error updating tree view: {ex.Message}";
        }
    }

    private void CloseFile()
    {
        try
        {
            if (CurrentDocument != null)
            {
                _clearDocumentUseCase.Execute(CurrentDocument);
                CurrentDocument = null;
                ChunkNodes.Clear();
                SelectedChunkNode = null;
                StatusMessage = "File closed";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error closing file: {ex.Message}";
        }
    }

    private static void Exit()
    {
        System.Windows.Application.Current?.Shutdown();
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
}