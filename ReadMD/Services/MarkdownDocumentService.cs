using System;
using System.IO;
using System.Threading.Tasks;

namespace ReadMD.Services;

public interface IMarkdownDocumentService
{
    string Markdown { get; set; }
    string? FilePath { get; set; }
    event Action? MarkdownChanged;
}

public class MarkdownDocumentService : IMarkdownDocumentService, IDisposable
{
    private string _markdown = string.Empty;
    private string? _filePath;
    private FileSystemWatcher? _watcher;

    public string Markdown
    {
        get => _markdown;
        set
        {
            if (_markdown == value) return;
            _markdown = value;
            MarkdownChanged?.Invoke();
        }
    }

    public string? FilePath
    {
        get => _filePath;
        set
        {
            _filePath = value;
            SetupWatcher(value);
        }
    }

    public event Action? MarkdownChanged;

    private void SetupWatcher(string? path)
    {
        _watcher?.Dispose();
        _watcher = null;

        if (path is null) return;

        _watcher = new FileSystemWatcher
        {
            Path = Path.GetDirectoryName(path)!,
            Filter = Path.GetFileName(path),
            NotifyFilter = NotifyFilters.LastWrite,
            EnableRaisingEvents = true
        };

        _watcher.Changed += async (_, _) =>
        {
            // Небольшая задержка — файл может быть ещё заблокирован редактором
            await Task.Delay(300);

            try
            {
                var text = await File.ReadAllTextAsync(path);
                // FileSystemWatcher стреляет не в UI-потоке
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    Markdown = text);
            }
            catch { /* файл временно заблокирован — пропускаем */ }
        };
    }

    public void Dispose() => _watcher?.Dispose();
}