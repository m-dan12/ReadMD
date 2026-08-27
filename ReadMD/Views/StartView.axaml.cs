using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using ReadMD.ViewModels;
using System;
using System.Linq;

namespace ReadMD.Views;

public partial class StartView : UserControl
{
    public StartView()
    {
        InitializeComponent();

        // Настраиваем drag and drop
        var dropZone = this.FindControl<Border>("DropZoneBorder");
        var button = this.FindControl<Button>("OpenFileButton");

        if (dropZone != null)
        {
            DragDrop.SetAllowDrop(dropZone, true);
            dropZone.AddHandler(DragDrop.DragOverEvent, DropZone_DragOver);
            dropZone.AddHandler(DragDrop.DropEvent, DropZone_Drop);
            dropZone.AddHandler(DragDrop.DragEnterEvent, DropZone_DragEnter);
            dropZone.AddHandler(DragDrop.DragLeaveEvent, DropZone_DragLeave);
            dropZone.PointerEntered += (s, e) =>
            {
                if (s is Border border)
                    border.BorderBrush = Application.Current?.FindResource("SystemControlForegroundBaseMediumBrush") as IBrush;
            };
            dropZone.PointerExited += (s, e) =>
            {
                if (s is Border border)
                    border.BorderBrush = Application.Current?.FindResource("SystemControlForegroundBaseLowBrush") as IBrush;
            };
        }

        // Перенаправляем клики на Border на Button
        if (dropZone != null && button != null)
        {
            dropZone.PointerPressed += (s, e) =>
            {
                if (button.Command?.CanExecute(null) == true)
                    button.Command.Execute(null);
            };
        }
    }

    private void DropZone_DragEnter(object? sender, DragEventArgs e)
    {
        if (sender is Border border && e.DataTransfer.Formats.Contains(DataFormat.File))
        {
            var files = e.DataTransfer.TryGetFiles();
            bool isMd = files?.Any(f =>
                f.Name.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
                f.Name.EndsWith(".markdown", StringComparison.OrdinalIgnoreCase)) ?? false;

            if (isMd)
            {
                border.BorderBrush = Application.Current?.FindResource("SystemControlForegroundBaseMediumBrush") as IBrush;
            }
        }
    }

    private void DropZone_DragLeave(object? sender, DragEventArgs e)
    {
        if (sender is Border border)
        {
            border.BorderBrush = Application.Current?.FindResource("SystemControlForegroundBaseLowBrush") as IBrush;
        }
    }

    private void DropZone_DragOver(object? sender, DragEventArgs e)
    {
        if (e.DataTransfer.Formats.Contains(DataFormat.File))
        {
            var files = e.DataTransfer.TryGetFiles();
            bool isMd = files?.Any(f =>
                f.Name.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
                f.Name.EndsWith(".markdown", StringComparison.OrdinalIgnoreCase)) ?? false;
            e.DragEffects = isMd ? DragDropEffects.Copy : DragDropEffects.None;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
        e.Handled = true;
    }

    private async void DropZone_Drop(object? sender, DragEventArgs e)
    {
        e.Handled = true;

        if (!e.DataTransfer.Formats.Contains(DataFormat.File)) return;

        var mdFile = e.DataTransfer.TryGetFiles()?
            .FirstOrDefault(f =>
                f.Name.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
                f.Name.EndsWith(".markdown", StringComparison.OrdinalIgnoreCase));

        if (mdFile is null) return;

        var path = mdFile.Path.LocalPath;

        if (DataContext is StartViewModel viewModel)
        {
            await viewModel.OpenFileAsync(path);
        }

        // Восстанавливаем обводку после drop
        if (sender is Border border)
        {
            border.BorderBrush = Application.Current?.FindResource("SystemControlForegroundBaseLowBrush") as IBrush;
        }
    }
}