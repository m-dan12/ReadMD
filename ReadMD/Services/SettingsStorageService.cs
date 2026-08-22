using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ReadMD.Models;

namespace ReadMD.Services;

public class SettingsStorageService
{
    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ReadMD"
    );

    private static readonly string SettingsFilePath = Path.Combine(SettingsDirectory, "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public async Task<AppSettings?> LoadSettingsAsync()
    {
        try
        {
            if (!File.Exists(SettingsFilePath))
                return null;

            var json = await File.ReadAllTextAsync(SettingsFilePath);
            return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load settings: {ex.Message}");
            return null;
        }
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        try
        {
            Directory.CreateDirectory(SettingsDirectory);
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            await File.WriteAllTextAsync(SettingsFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }
}
