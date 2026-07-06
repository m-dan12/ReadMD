using Microsoft.Extensions.DependencyInjection;
using ReadMD.Services;
using ReadMD.ViewModels;
using ReadMD.Views;

namespace ReadMD.DI;

/// <summary>
/// Расширение для конфигурации служб приложения.
/// Здесь регистрируются все сервисы, ViewModel и Views.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddSingleton<IReadingSettingsService, ReadingSettingsService>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IFileDialogService, FileDialogService>();
        services.AddSingleton<IDocumentService, DocumentService>();

        services.AddTransient<MainWindowViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<TitleBarViewModel>();
        services.AddSingleton<StartViewModel>();

        services.AddTransient<MainWindow>();
        services.AddTransient<MainView>();
        services.AddTransient<TitleBarView>();
        services.AddTransient<StartView>();

        return services;
    }
}
