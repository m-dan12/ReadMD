using Avalonia.Threading;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ReadMD.Services;

public interface IDocumentService
{
    string Content { get; set; }
    string? FilePath { get; }
    bool IsLoaded { get; }
    event EventHandler? DocumentChanged;
    event EventHandler? FilePathChanged;

    Task LoadAsync(string path);
    Task SaveAsync();
    void Close();
}

public class DocumentService : IDocumentService, IDisposable
{
    private string _content = string.Empty;
    private string? _filePath;
    private FileSystemWatcher? _watcher;

    public string Content
    {
        get => _content;
        set
        {
            if (_content == value) return;
            _content = value;
            DocumentChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public string? FilePath
    {
        get => _filePath;
        private set
        {
            if (_filePath == value) return;
            _filePath = value;
            FilePathChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsLoaded => FilePath is not null;

    public event EventHandler? DocumentChanged;
    public event EventHandler? FilePathChanged;

    public async Task LoadAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(path));

        var text = await File.ReadAllTextAsync(path).ConfigureAwait(false);

        FilePath = path;
        Content = text;
        SetupWatcher(path);
    }

    public async Task SaveAsync()
    {
        if (FilePath is null)
            return;

        await File.WriteAllTextAsync(FilePath, Content).ConfigureAwait(false);
    }

    public void Close()
    {
        Content = string.Empty;
        FilePath = null;
        _watcher?.Dispose();
        _watcher = null;
    }

    private void SetupWatcher(string path)
    {
        _watcher?.Dispose();
        _watcher = null;

        _watcher = new FileSystemWatcher
        {
            Path = Path.GetDirectoryName(path)!,
            Filter = Path.GetFileName(path),
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
        };

        _watcher.Changed += async (_, _) =>
        {
            await Task.Delay(300).ConfigureAwait(false);

            try
            {
                var text = await File.ReadAllTextAsync(path).ConfigureAwait(false);
                await Dispatcher.UIThread.InvokeAsync(() => Content = text);
            }
            catch
            {
                // Файл временно заблокирован или чтение прошло неуспешно.
            }
        };

        _watcher.EnableRaisingEvents = true;
    }

    public void Dispose() => _watcher?.Dispose();
}
