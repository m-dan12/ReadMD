using Avalonia.Controls;
using ReadMD.Services;
using System;

namespace ReadMD.Services;

public interface ITrayIconService
{
    void Initialize(Window mainWindow, IDocumentService documentService);
    void Show();
    void Hide();
}

public class TrayIconService : ITrayIconService
{
    private TrayIcon? _trayIcon;
    private Window? _mainWindow;
    private IDocumentService? _documentService;

    public void Initialize(Window mainWindow, IDocumentService documentService)
    {
        _mainWindow = mainWindow;
        _documentService = documentService;

        _trayIcon = new TrayIcon
        {
            Icon = mainWindow.Icon,
            ToolTipText = "ReadMD",
            IsVisible = true
        };

        var menu = new NativeMenu();

        var showItem = new NativeMenuItem { Header = "Показать" };
        showItem.Click += (_, _) => ShowMainWindow();

        var exitItem = new NativeMenuItem { Header = "Выход" };
        exitItem.Click += (_, _) => ExitApplication();

        menu.Items.Add(showItem);
        menu.Items.Add(new NativeMenuItemSeparator());
        menu.Items.Add(exitItem);

        _trayIcon.Menu = menu;

        _trayIcon.Clicked += (_, _) => ShowMainWindow();

        // Подписываемся на событие скрытия окна
        _mainWindow.PropertyChanged += (_, e) =>
        {
            if (e.Property.Name == nameof(Window.IsVisible) && !_mainWindow.IsVisible)
            {
                OnWindowHidden();
            }
        };
    }

    public void Show()
    {
        if (_trayIcon != null)
            _trayIcon.IsVisible = true;
    }

    public void Hide()
    {
        if (_trayIcon != null)
            _trayIcon.IsVisible = false;
    }

    private void OnWindowHidden()
    {
        // Выгружаем файл из памяти при сворачивании в трей
        _documentService?.Close();
        System.Diagnostics.Debug.WriteLine("[TrayIcon] Window hidden, document unloaded from memory");
    }

    private void ShowMainWindow()
    {
        if (_mainWindow == null) return;

        _mainWindow.Show();
        _mainWindow.WindowState = Avalonia.Controls.WindowState.Normal;
        _mainWindow.Activate();
        System.Diagnostics.Debug.WriteLine("[TrayIcon] Window shown");
    }

    private void ExitApplication()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Разрешаем окну закрыться
            if (_mainWindow is MainWindow mainWindow)
            {
                mainWindow.AllowClose();
            }
            desktop.Shutdown();
        }
    }
}
