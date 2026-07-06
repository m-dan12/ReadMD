using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReadMD.Models;
using ReadMD.Services;
using System.Threading.Tasks;

namespace ReadMD.ViewModels;

public partial class StartViewModel : ViewModelBase
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IDocumentService _documentService;
    private readonly ILocalizationService _localizationService;

    public StartViewModel(
        IFileDialogService fileDialogService,
        IDocumentService documentService,
        ILocalizationService localizationService)
    {
        _fileDialogService = fileDialogService;
        _documentService = documentService;
        _localizationService = localizationService;
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
}
