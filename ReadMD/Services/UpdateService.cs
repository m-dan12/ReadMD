using System;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace ReadMD.Services;

public interface IUpdateService
{
    event EventHandler<UpdateInfo>? UpdateAvailable;
    Task CheckForUpdatesAsync();
    Task<bool> DownloadAndInstallUpdateAsync(UpdateInfo updateInfo);
    string CurrentVersion { get; }
}

public class UpdateService : IUpdateService
{
    private UpdateManager? _updateManager;

    public event EventHandler<UpdateInfo>? UpdateAvailable;

    public string CurrentVersion => _updateManager?.CurrentVersion?.ToString() ?? "Unknown";

    public UpdateService()
    {
        try
        {
            // Инициализируем UpdateManager - он сам проверит, установлено ли приложение через Velopack
            _updateManager = new UpdateManager(
                new GithubSource("https://github.com/m-dan12/ReadMD", null, false)
            );
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UpdateService] Failed to initialize: {ex.Message}");
            _updateManager = null;
        }
    }

    public async Task CheckForUpdatesAsync()
    {
        if (_updateManager == null)
        {
            System.Diagnostics.Debug.WriteLine("[UpdateService] Not installed via Velopack, skipping update check");
            return;
        }

        try
        {
            var updateInfo = await _updateManager.CheckForUpdatesAsync();
            if (updateInfo != null)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateService] Update available: {updateInfo.TargetFullRelease.Version}");
                UpdateAvailable?.Invoke(this, updateInfo);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[UpdateService] No updates available");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UpdateService] Check for updates failed: {ex.Message}");
        }
    }

    public async Task<bool> DownloadAndInstallUpdateAsync(UpdateInfo updateInfo)
    {
        if (_updateManager == null)
            return false;

        try
        {
            System.Diagnostics.Debug.WriteLine("[UpdateService] Downloading update...");
            await _updateManager.DownloadUpdatesAsync(updateInfo);

            System.Diagnostics.Debug.WriteLine("[UpdateService] Applying update and restarting...");
            _updateManager.ApplyUpdatesAndRestart(updateInfo);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UpdateService] Update failed: {ex.Message}");
            return false;
        }
    }
}
