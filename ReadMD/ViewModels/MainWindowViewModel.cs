using CommunityToolkit.Mvvm.ComponentModel;
using ReadMD.Services;
using System;

namespace ReadMD.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IDocumentService _documentService;

    [ObservableProperty] private TitleBarViewModel titleBarViewModel;
    [ObservableProperty] private ViewModelBase currentView;

    private readonly MainViewModel _mainViewModel;
    private readonly StartViewModel _startViewModel;

    public MainWindowViewModel(
        TitleBarViewModel titleBarViewModel,
        MainViewModel mainViewModel,
        StartViewModel startViewModel,
        IDocumentService documentService,
        IThemeService themeService,
        ILocalizationService localizationService,
        IReadingSettingsService readingSettingsService)
    {
        TitleBarViewModel = titleBarViewModel;
        TitleBarViewModel.OnCloseFile = CloseFile;
        _mainViewModel = mainViewModel;
        _startViewModel = startViewModel;
        _documentService = documentService;

        currentView = _startViewModel;
        _documentService.FilePathChanged += OnDocumentStateChanged;

        UpdateView();

        // Загружаем сохраненные настройки при запуске
        _ = LoadSettingsAsync(themeService, localizationService, readingSettingsService);
    }

    private async System.Threading.Tasks.Task LoadSettingsAsync(
        IThemeService themeService,
        ILocalizationService localizationService,
        IReadingSettingsService readingSettingsService)
    {
        try
        {
            await themeService.LoadThemeAsync();
            await localizationService.LoadLanguageAsync();
            await readingSettingsService.LoadSettingsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load settings: {ex.Message}");
        }
    }

    private void OnDocumentStateChanged(object? sender, EventArgs e) => UpdateView();

    private void UpdateView()
    {
        CurrentView = _documentService.FilePath is null
            ? _startViewModel
            : _mainViewModel;
    }

    public void CloseFile()
    {
        _documentService.Close();
        CurrentView = _startViewModel;
    }
}
