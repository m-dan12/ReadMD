using CommunityToolkit.Mvvm.ComponentModel;

namespace ReadMD.Models;

public partial class UiStrings : ObservableObject
{
    [ObservableProperty] private string _appName = string.Empty;
    [ObservableProperty] private string _fileNamePlaceholder = string.Empty;
    [ObservableProperty] private string _menu = string.Empty;
    [ObservableProperty] private string _openFile = string.Empty;
    [ObservableProperty] private string _closeFile = string.Empty;
    [ObservableProperty] private string _language = string.Empty;
    [ObservableProperty] private string _languageSystem = string.Empty;
    [ObservableProperty] private string _languageRussian = string.Empty;
    [ObservableProperty] private string _languageEnglish = string.Empty;
    [ObservableProperty] private string _reading = string.Empty;
    [ObservableProperty] private string _font = string.Empty;
    [ObservableProperty] private string _fontSerif = string.Empty;
    [ObservableProperty] private string _fontSansSerif = string.Empty;
    [ObservableProperty] private string _fontSize = string.Empty;
    [ObservableProperty] private string _lineSpacing = string.Empty;
    [ObservableProperty] private string _lineWidth = string.Empty;
    [ObservableProperty] private string _recentFiles = string.Empty;
    [ObservableProperty] private string _noRecentFiles = string.Empty;

    public void UpdateFrom(UiStringsData data)
    {
        AppName = data.AppName;
        FileNamePlaceholder = data.FileNamePlaceholder;
        Menu = data.Menu;
        OpenFile = data.OpenFile;
        CloseFile = data.CloseFile;
        Language = data.Language;
        LanguageSystem = data.LanguageSystem;
        LanguageRussian = data.LanguageRussian;
        LanguageEnglish = data.LanguageEnglish;
        Reading = data.Reading;
        Font = data.Font;
        FontSerif = data.FontSerif;
        FontSansSerif = data.FontSansSerif;
        FontSize = data.FontSize;
        LineSpacing = data.LineSpacing;
        LineWidth = data.LineWidth;
        RecentFiles = data.RecentFiles;
        NoRecentFiles = data.NoRecentFiles;
    }
}
