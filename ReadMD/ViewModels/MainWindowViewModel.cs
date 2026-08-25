using CommunityToolkit.Mvvm.ComponentModel;
using ReadMD.Services;
using System;
using System.Threading.Tasks;

namespace ReadMD.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IDocumentService _documentService;
    private readonly IUpdateService _updateService;

    [ObservableProperty] private TitleBarViewModel titleBarViewModel;
    [ObservableProperty] private ViewModelBase currentView;
    [ObservableProperty] private UpdateNotificationViewModel updateNotificationViewModel;

    private readonly MainViewModel _mainViewModel;
    private readonly StartViewModel _startViewModel;

    public MainWindowViewModel(
        TitleBarViewModel titleBarViewModel,
        MainViewModel mainViewModel,
        StartViewModel startViewModel,
        UpdateNotificationViewModel updateNotificationViewModel,
        IDocumentService documentService,
        IThemeService themeService,
        ILocalizationService localizationService,
        IReadingSettingsService readingSettingsService,
        IUpdateService updateService)
    {
        TitleBarViewModel = titleBarViewModel;
        TitleBarViewModel.OnCloseFile = CloseFile;
        _mainViewModel = mainViewModel;
        _startViewModel = startViewModel;
        UpdateNotificationViewModel = updateNotificationViewModel;
        _documentService = documentService;
        _updateService = updateService;

        // Если файл уже загружается/загружен, показываем MainView сразу
        currentView = _documentService.IsLoaded || _documentService.IsLoading
            ? _mainViewModel
            : _startViewModel;

        _documentService.FilePathChanged += OnDocumentStateChanged;

        // Подписываемся на событие обновления
        _updateService.UpdateAvailable += OnUpdateAvailable;

        // Загружаем сохраненные настройки при запуске
        _ = LoadSettingsAsync(themeService, localizationService, readingSettingsService);

        // Проверяем обновления при запуске (без await, не блокируем UI)
        _ = _updateService.CheckForUpdatesAsync();
    }

    private void OnUpdateAvailable(object? sender, Velopack.UpdateInfo e)
    {
        // Показываем popup с уведомлением
        UpdateNotificationViewModel.Show(e);
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
