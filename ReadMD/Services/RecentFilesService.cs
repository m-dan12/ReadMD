using ReadMD.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReadMD.Services;

public interface IRecentFilesService
{
    List<RecentFile> RecentFiles { get; }
    event EventHandler? RecentFilesChanged;
    Task AddRecentFileAsync(string filePath);
    Task LoadRecentFilesAsync();
    Task ClearRecentFilesAsync();
}

public class RecentFilesService : IRecentFilesService
{
    private static readonly string RecentFilesPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ReadMD",
        "recent_files.json"
    );

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private const int MaxRecentFiles = 10;
    private const int PreviewLength = 150;

    public List<RecentFile> RecentFiles { get; private set; } = [];

    public event EventHandler? RecentFilesChanged;

    public async Task LoadRecentFilesAsync()
    {
        try
        {
            if (!File.Exists(RecentFilesPath))
            {
                RecentFiles = [];
                return;
            }

            var json = await File.ReadAllTextAsync(RecentFilesPath);
            var files = JsonSerializer.Deserialize<List<RecentFile>>(json, JsonOptions) ?? [];

            // Фильтруем только существующие файлы
            RecentFiles = files.Where(f => File.Exists(f.FilePath)).ToList();

            RecentFilesChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load recent files: {ex.Message}");
            RecentFiles = [];
        }
    }

    public async Task AddRecentFileAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return;

            // Удаляем старую запись, если файл уже был в списке
            RecentFiles.RemoveAll(f => f.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));

            // Читаем содержимое для превью
            var content = await File.ReadAllTextAsync(filePath);
            var previewText = ExtractPreview(content);

            // Добавляем в начало списка
            RecentFiles.Insert(0, new RecentFile
            {
                FilePath = filePath,
                FileName = Path.GetFileName(filePath),
                PreviewText = previewText,
                LastOpenedAt = DateTime.Now
            });

            // Ограничиваем количество файлов
            if (RecentFiles.Count > MaxRecentFiles)
            {
                RecentFiles = RecentFiles.Take(MaxRecentFiles).ToList();
            }

            await SaveRecentFilesAsync();
            RecentFilesChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to add recent file: {ex.Message}");
        }
    }

    public async Task ClearRecentFilesAsync()
    {
        RecentFiles.Clear();
        await SaveRecentFilesAsync();
        RecentFilesChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task SaveRecentFilesAsync()
    {
        try
        {
            var directory = Path.GetDirectoryName(RecentFilesPath);
            if (directory != null)
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(RecentFiles, JsonOptions);
            await File.WriteAllTextAsync(RecentFilesPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save recent files: {ex.Message}");
        }
    }

    private static string ExtractPreview(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        // Убираем заголовки markdown для превью
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var textLines = lines
            .Select(l => l.TrimStart('#', ' ', '\t', '\r'))
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Take(3);

        var preview = string.Join(" ", textLines);

        if (preview.Length > PreviewLength)
        {
            preview = preview.Substring(0, PreviewLength) + "...";
        }

        return preview;
    }
}
