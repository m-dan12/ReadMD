using Avalonia;
using Avalonia.Platform;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReadMD.Models;
using ReadMD.Services;
using System;
using System.Threading.Tasks;

namespace ReadMD.ViewModels;

public partial class StartViewModel : ViewModelBase, IDisposable
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IDocumentService _documentService;
    private readonly ILocalizationService _localizationService;
    private readonly IThemeService _themeService;

    [ObservableProperty] private bool _isDarkTheme;

    public StartViewModel(
        IFileDialogService fileDialogService,
        IDocumentService documentService,
        ILocalizationService localizationService,
        IThemeService themeService)
    {
        _fileDialogService = fileDialogService;
        _documentService = documentService;
        _localizationService = localizationService;
        _themeService = themeService;

        SyncThemeFromService();
        _themeService.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged() => SyncThemeFromService();

    private void SyncThemeFromService()
    {
        var isDark = _themeService.CurrentTheme == ThemeVariant.Dark ||
                     (_themeService.CurrentTheme == ThemeVariant.Default &&
                      Application.Current?.PlatformSettings?.GetColorValues().ThemeVariant == PlatformThemeVariant.Dark);

#pragma warning disable MVVMTK0034
        _isDarkTheme = isDark;
#pragma warning restore MVVMTK0034
        OnPropertyChanged(nameof(IsDarkTheme));
    }

    public UiStrings Texts => _localizationService.Strings;

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var path = await _fileDialogService.ShowOpenMarkdownFileDialogAsync();
        if (path is null)
            return;

        await _documentService.LoadAsync(path);
    }

    public void Dispose()
    {
        _themeService.ThemeChanged -= OnThemeChanged;
    }
}
