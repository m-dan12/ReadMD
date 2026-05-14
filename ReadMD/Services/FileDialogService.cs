using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadMD.Services;

public interface IFileDialogService
{
    void Initialize(Window window);
    Task<string?> ShowOpenMarkdownFileDialogAsync();
}

public class FileDialogService : IFileDialogService
{
    private Window? _window;

    public void Initialize(Window window) => _window = window;

    private Window Window => _window ?? throw new InvalidOperationException("FileDialogService не инициализирован");

    public async Task<string?> ShowOpenMarkdownFileDialogAsync()
    {
        var files = await Window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Открыть Markdown-файл",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("Markdown")
                {
                    Patterns = ["*.md", "*.markdown"]
                },
                FilePickerFileTypes.All
            }
        });

        return files is [var file, ..] ? file.Path.LocalPath : null;
    }
}