using Avalonia.Platform;
using ReadMD.Models;
using System;
using System.Globalization;
using System.Text.Json;

namespace ReadMD.Services;

public interface ILocalizationService
{
    UiStrings Strings { get; }
    AppLanguage CurrentLanguage { get; }
    void SetLanguage(AppLanguage language);
    event EventHandler? LanguageChanged;
}

public sealed class LocalizationService : ILocalizationService
{
    private const string ResourceBase = "avares://ReadMD/Assets/Localization";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public UiStrings Strings { get; } = new();

    public AppLanguage CurrentLanguage { get; private set; } = AppLanguage.System;

    public event EventHandler? LanguageChanged;

    public LocalizationService() => SetLanguage(AppLanguage.System);

    public void SetLanguage(AppLanguage language)
    {
        var cultureCode = ResolveCultureCode(language);
        if (CurrentLanguage == language && Strings.Menu.Length > 0)
            return;

        CurrentLanguage = language;
        LoadStrings(cultureCode);
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    private static string ResolveCultureCode(AppLanguage language) => language switch
    {
        AppLanguage.Russian => "ru",
        AppLanguage.English => "en",
        _ => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("ru", StringComparison.OrdinalIgnoreCase)
            ? "ru"
            : "en"
    };

    private void LoadStrings(string cultureCode)
    {
        var uri = new Uri($"{ResourceBase}/{cultureCode}.json");
        using var stream = AssetLoader.Open(uri);
        var data = JsonSerializer.Deserialize<UiStringsData>(stream, JsonOptions)
                   ?? throw new InvalidOperationException($"Failed to load localization: {cultureCode}");
        Strings.UpdateFrom(data);
    }
}
