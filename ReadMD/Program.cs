using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using ReadMD.Services;
using System;
using System.Linq;
using Velopack;

namespace ReadMD;

/// <summary>
/// Главная точка входа приложения.
/// </summary>
internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        VelopackApp.Build().Run();

        System.Diagnostics.Debug.WriteLine($"[Program] Main started with args: {string.Join(", ", args)}");

        // Инициализируем DI контейнер для single-instance проверки
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection()
            .AddSingleton<ISingleInstanceService, SingleInstanceService>()
            .BuildServiceProvider();

        var singleInstance = services.GetRequiredService<ISingleInstanceService>();

        if (!singleInstance.TryAcquireInstance())
        {
            System.Diagnostics.Debug.WriteLine($"[Program] Second instance detected - forwarding to first");
            // Второй экземпляр - отправляем путь к файлу первому и завершаемся
            if (args.Length > 0 && System.IO.File.Exists(args[0]))
            {
                try
                {
                    singleInstance.SendFilePathToRunningInstanceAsync(args[0]).GetAwaiter().GetResult();
                    System.Diagnostics.Debug.WriteLine($"[Program] File path forwarded successfully");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Program] Failed to forward: {ex.Message}");
                    Console.WriteLine($"Failed to send file path to running instance: {ex.Message}");
                }
            }
            System.Diagnostics.Debug.WriteLine($"[Program] Second instance exiting");
            return;
        }

        System.Diagnostics.Debug.WriteLine($"[Program] First instance - starting Avalonia");
        // Первый экземпляр - запускаем Avalonia
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        singleInstance.Release();
    }

    /// <summary>
    /// Инициализация Avalonia и настройка приложения.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
