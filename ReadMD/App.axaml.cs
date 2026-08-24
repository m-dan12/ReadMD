using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Markdig;
using MarkView.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using ReadMD.DI;
using ReadMD.Services;
using ReadMD.ViewModels;
using System;
using System.Diagnostics;

namespace ReadMD;

/// <summary>
/// �������� ����� ���������� Avalonia.
/// ����������� ��������� ������������, Markdown-������ � ������� ����.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// ���������� ������-���������, ������������ ��� ���������� Views � ViewModels.
    /// </summary>
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        Services = new ServiceCollection().ConfigureServices().BuildServiceProvider();

        // Регистрируем обработчик MarkdownViewer для открытия ссылок в стандартном браузере.
        MarkdownViewer.LinkClickedEvent.AddClassHandler<MarkdownViewer>((_, e) =>
            Process.Start(new ProcessStartInfo(e.Url) { UseShellExecute = true }));

        MarkdownViewerDefaults.Pipeline = new MarkdownPipelineBuilder()
            .UseSupportedExtensions()
            .UseAlertBlocks()
            .Build();

        MarkdownViewerDefaults.Extensions.AddTextMateHighlighting();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = Services.GetRequiredService<MainWindow>();
            Services.GetRequiredService<IWindowService>().Initialize(window);
            Services.GetRequiredService<IFileDialogService>().Initialize(window);

            var documentService = Services.GetRequiredService<IDocumentService>();
            var singleInstanceService = Services.GetRequiredService<ISingleInstanceService>();
            var trayIconService = Services.GetRequiredService<ITrayIconService>();

            // Инициализируем трей
            trayIconService.Initialize(window, documentService);

            // Слушаем IPC сообщения от других экземпляров
            singleInstanceService.StartListening(filePath =>
            {
                System.Diagnostics.Debug.WriteLine($"[App] IPC callback invoked with file: {filePath}");
                Dispatcher.UIThread.Post(() =>
                {
                    System.Diagnostics.Debug.WriteLine($"[App] Loading file in UI thread: {filePath}");
                    // Показываем окно и загружаем файл
                    window.Show();
                    window.WindowState = Avalonia.Controls.WindowState.Normal;
                    window.Activate();
                    _ = documentService.LoadAsync(filePath);
                });
            });

            // Открыть файл, если путь передан через командную строку
            if (desktop.Args?.Length > 0)
            {
                var filePath = desktop.Args[0];
                if (System.IO.File.Exists(filePath))
                {
                    // Загружаем файл асинхронно (не блокируем UI)
                    _ = documentService.LoadAsync(filePath);
                }
            }

            var viewModel = Services.GetRequiredService<MainWindowViewModel>();
            window.DataContext = viewModel;
            desktop.MainWindow = window;

            // Обработчик закрытия приложения
            desktop.ShutdownRequested += (_, _) =>
            {
                singleInstanceService.Release();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
