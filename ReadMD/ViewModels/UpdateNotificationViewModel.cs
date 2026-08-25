using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReadMD.Services;
using System.Threading.Tasks;
using Velopack;

namespace ReadMD.ViewModels;

public partial class UpdateNotificationViewModel : ViewModelBase
{
    private readonly IUpdateService _updateService;
    private UpdateInfo? _updateInfo;

    [ObservableProperty] private bool _isVisible;
    [ObservableProperty] private string _newVersion = string.Empty;
    [ObservableProperty] private bool _isInstalling;

    public UpdateNotificationViewModel(IUpdateService updateService)
    {
        _updateService = updateService;
    }

    public void Show(UpdateInfo updateInfo)
    {
        _updateInfo = updateInfo;
        NewVersion = updateInfo.TargetFullRelease.Version.ToString();
        IsVisible = true;
    }

    [RelayCommand]
    private async Task InstallUpdateAsync()
    {
        if (_updateInfo == null) return;

        IsInstalling = true;
        var success = await _updateService.DownloadAndInstallUpdateAsync(_updateInfo);

        if (!success)
        {
            IsInstalling = false;
            // Если не удалось установить, скрываем popup
            IsVisible = false;
        }
        // Если успешно, приложение перезапустится автоматически
    }

    [RelayCommand]
    private void Dismiss()
    {
        IsVisible = false;
    }
}
