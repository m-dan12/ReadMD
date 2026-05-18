using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Markdig;
using MarkView.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using ReadMD.DI;
using ReadMD.Services;
using ReadMD.ViewModels;
using System;
using System.Diagnostics;

namespace ReadMD;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        Services = new ServiceCollection().ConfigureServices().BuildServiceProvider();

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
            window.DataContext = Services.GetRequiredService<MainWindowViewModel>();
            Services.GetRequiredService<IWindowService>().Initialize(window);
            Services.GetRequiredService<IFileDialogService>().Initialize(window);
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}