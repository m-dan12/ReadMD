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
        IDocumentService documentService)
    {
        TitleBarViewModel = titleBarViewModel;
        TitleBarViewModel.OnCloseFile = CloseFile;
        _mainViewModel = mainViewModel;
        _startViewModel = startViewModel;
        _documentService = documentService;

        currentView = _startViewModel;
        _documentService.FilePathChanged += OnDocumentStateChanged;

        UpdateView();
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
