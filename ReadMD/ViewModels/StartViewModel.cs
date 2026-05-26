using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReadMD.Models;
using ReadMD.Services;
using System.IO;
using System.Threading.Tasks;

namespace ReadMD.ViewModels;

public partial class StartViewModel : ViewModelBase
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IMarkdownDocumentService _markdownDocumentService;
    private readonly ILocalizationService _localizationService;

    public StartViewModel(
        IFileDialogService fileDialogService,
        IMarkdownDocumentService markdownDocumentService,
        ILocalizationService localizationService)
    {
        _fileDialogService = fileDialogService;
        _markdownDocumentService = markdownDocumentService;
        _localizationService = localizationService;
    }
    public UiStrings Texts => _localizationService.Strings;

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var path = await _fileDialogService.ShowOpenMarkdownFileDialogAsync();
        if (path is null) return;

        _markdownDocumentService.FilePath = path;
        _markdownDocumentService.Markdown = await File.ReadAllTextAsync(path);
    }
}