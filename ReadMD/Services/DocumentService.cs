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
    bool IsLoading { get; }
    event EventHandler? DocumentChanged;
    event EventHandler? FilePathChanged;

    Task LoadAsync(string path);
    Task SaveAsync();
    void Close();
}

public class DocumentService : IDocumentService, IDisposable
{
    private readonly IErrorHandlingService _errorHandlingService;
    private readonly IRecentFilesService? _recentFilesService;
    private string _content = string.Empty;
    private string? _filePath;
    private FileSystemWatcher? _watcher;
    private bool _isLoading;

    public DocumentService(
        IErrorHandlingService errorHandlingService,
        IRecentFilesService? recentFilesService = null)
    {
        _errorHandlingService = errorHandlingService;
        _recentFilesService = recentFilesService;
    }

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
    public bool IsLoading => _isLoading;

    public event EventHandler? DocumentChanged;
    public event EventHandler? FilePathChanged;

    public async Task LoadAsync(string path)
    {
        try
        {
            _isLoading = true;

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("Файл не найден.", path);

            var text = await File.ReadAllTextAsync(path).ConfigureAwait(false);

            FilePath = path;
            Content = text;
            SetupWatcher(path);

            // Добавляем файл в список недавних
            if (_recentFilesService is not null)
            {
                _ = _recentFilesService.AddRecentFileAsync(path);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            _errorHandlingService.ShowError("Ошибка доступа", $"Недостаточно прав для открытия файла: {ex.Message}");
            throw;
        }
        catch (FileNotFoundException ex)
        {
            _errorHandlingService.ShowError("Файл не найден", $"Файл не существует: {ex.FileName}");
            throw;
        }
        catch (IOException ex)
        {
            _errorHandlingService.ShowError("Ошибка чтения", $"Не удалось прочитать файл: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _errorHandlingService.ShowError("Неизвестная ошибка", $"Не удалось открыть файл: {ex.Message}");
            throw;
        }
        finally
        {
            _isLoading = false;
        }
    }

    public async Task SaveAsync()
    {
        try
        {
            if (FilePath is null)
                return;

            await File.WriteAllTextAsync(FilePath, Content).ConfigureAwait(false);
        }
        catch (UnauthorizedAccessException ex)
        {
            _errorHandlingService.ShowError("Ошибка доступа", $"Недостаточно прав для сохранения файла: {ex.Message}");
            throw;
        }
        catch (IOException ex)
        {
            _errorHandlingService.ShowError("Ошибка записи", $"Не удалось сохранить файл: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _errorHandlingService.ShowError("Неизвестная ошибка", $"Не удалось сохранить файл: {ex.Message}");
            throw;
        }
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
            catch (IOException ex)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                    _errorHandlingService.ShowWarning("Предупреждение", $"Файл изменён внешне, но не удалось перезагрузить: {ex.Message}"));
            }
            catch (Exception ex)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                    _errorHandlingService.ShowWarning("Предупреждение", $"Ошибка при отслеживании изменений: {ex.Message}"));
            }
        };

        _watcher.EnableRaisingEvents = true;
    }

    public void Dispose() => _watcher?.Dispose();
}
