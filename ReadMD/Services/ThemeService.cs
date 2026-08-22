using Avalonia;
using Avalonia.Platform;
using Avalonia.Styling;
using System;
using System.Threading.Tasks;
using ReadMD.Models;

namespace ReadMD.Services;

public interface IThemeService
{
    ThemeVariant CurrentTheme { get; }
    event Action? ThemeChanged;
    void SetTheme(ThemeVariant theme);
    Task LoadThemeAsync();
    Task SaveThemeAsync();
}

public class ThemeService : IThemeService
{
    private readonly SettingsStorageService _storageService;

    public ThemeVariant CurrentTheme { get; private set; } = ThemeVariant.Default;

    public event Action? ThemeChanged;

    public ThemeService(SettingsStorageService storageService)
    {
        _storageService = storageService;

        // Определяем текущую системную тему при инициализации
        if (Application.Current != null)
        {
            var platformSettings = Application.Current.PlatformSettings;
            if (platformSettings != null)
            {
                var platformTheme = platformSettings.GetColorValues().ThemeVariant;
                CurrentTheme = platformTheme == PlatformThemeVariant.Dark ? ThemeVariant.Dark : ThemeVariant.Light;
            }
        }
    }

    public async Task LoadThemeAsync()
    {
        var settings = await _storageService.LoadSettingsAsync();
        if (settings != null && !string.IsNullOrEmpty(settings.Theme))
        {
            var theme = settings.Theme switch
            {
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };
            SetTheme(theme);
        }
    }

    public async Task SaveThemeAsync()
    {
        var settings = await _storageService.LoadSettingsAsync() ?? new AppSettings();
        settings.Theme = CurrentTheme.Key?.ToString() ?? "Default";
        await _storageService.SaveSettingsAsync(settings);
    }

    public void SetTheme(ThemeVariant theme)
    {
        CurrentTheme = theme;
        Application.Current?.RequestedThemeVariant = theme;
        ThemeChanged?.Invoke();

        // Автосохранение при изменении темы
        _ = SaveThemeAsync();
    }
}