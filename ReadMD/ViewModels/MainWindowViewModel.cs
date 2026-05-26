using CommunityToolkit.Mvvm.ComponentModel;
using ReadMD.Services;

namespace ReadMD.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IMarkdownDocumentService _markdownDocumentService;

    [ObservableProperty] private TitleBarViewModel titleBarViewModel;
    [ObservableProperty] private ViewModelBase currentView;

    private readonly MainViewModel _mainViewModel;
    private readonly StartViewModel _startViewModel;

    public MainWindowViewModel(
        TitleBarViewModel titleBarViewModel,
        MainViewModel mainViewModel,
        StartViewModel startViewModel,
        IMarkdownDocumentService markdownDocumentService)
    {
        TitleBarViewModel = titleBarViewModel;
        TitleBarViewModel.OnCloseFile = CloseFile;
        _mainViewModel = mainViewModel;
        _startViewModel = startViewModel;
        _markdownDocumentService = markdownDocumentService;

        // Начинаем со стартового экрана
        currentView = startViewModel;

        // Переключаемся на MainView когда загружен файл
        _markdownDocumentService.MarkdownChanged += OnMarkdownChanged;
    }

    private void OnMarkdownChanged()
    {
        CurrentView = string.IsNullOrEmpty(_markdownDocumentService.FilePath)
            ? _startViewModel
            : _mainViewModel;
    }

    // Вызывается из TitleBarViewModel при закрытии файла
    public void CloseFile()
    {
        _markdownDocumentService.FilePath = null;
        _markdownDocumentService.Markdown = string.Empty;
        CurrentView = _startViewModel;
    }
}