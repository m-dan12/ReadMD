using Microsoft.Extensions.DependencyInjection;
using ReadMD.Services;
using ReadMD.ViewModels;
using ReadMD.Views;

namespace ReadMD.DI;

/// <summary>
/// ���������� ��� ������������ ����� ����������.
/// ����� �������������� ��� �������, ViewModel � Views.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<SettingsStorageService>();
        services.AddSingleton<IErrorHandlingService, ErrorHandlingService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddSingleton<IReadingSettingsService, ReadingSettingsService>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IFileDialogService, FileDialogService>();
        services.AddSingleton<IDocumentService, DocumentService>();
        services.AddSingleton<ISingleInstanceService, SingleInstanceService>();
        services.AddSingleton<ITrayIconService, TrayIconService>();
        services.AddSingleton<IUpdateService, UpdateService>();

        services.AddTransient<MainWindowViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<TitleBarViewModel>();
        services.AddSingleton<StartViewModel>();
        services.AddSingleton<UpdateNotificationViewModel>();

        services.AddTransient<MainWindow>();
        services.AddTransient<MainView>();
        services.AddTransient<TitleBarView>();
        services.AddTransient<StartView>();
        services.AddTransient<UpdateNotificationView>();

        return services;
    }
}
