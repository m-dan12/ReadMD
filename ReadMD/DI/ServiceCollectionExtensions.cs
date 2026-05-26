using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using ReadMD.Services;
using ReadMD.ViewModels;
using ReadMD.Views;
using System;

namespace ReadMD.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        //Services
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddSingleton<IReadingSettingsService, ReadingSettingsService>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IFileDialogService, FileDialogService>();
        services.AddSingleton<IMarkdownDocumentService, MarkdownDocumentService>();

        // ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<TitleBarViewModel>();
        services.AddSingleton<StartViewModel>();

        // Views
        services.AddTransient<MainWindow>();
        services.AddTransient<MainView>();
        services.AddTransient<TitleBarView>();
        services.AddTransient<StartView>();

        return services;
    }
}