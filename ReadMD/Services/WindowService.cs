using Avalonia.Controls;
using Avalonia.Input;
using System;

namespace ReadMD.Services;

public interface IWindowService
{
    WindowState GetWindowState();
    void Initialize(Window window);
    void Minimize();
    void ToggleMaximize();
    void Close();
    void BeginMoveDrag(PointerPressedEventArgs e);
}

public class WindowService : IWindowService
{
    private Window? _window;
    private Window Window => _window
        ?? throw new InvalidOperationException("WindowService не инициализирован");

    public WindowState GetWindowState() => Window.WindowState;
    public void Initialize(Window window) => _window = window;
    public void Minimize() => Window.WindowState = WindowState.Minimized;
    public void ToggleMaximize() => Window.WindowState = Window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    public void Close() => Window.Close();

    public void BeginMoveDrag(PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(Window).Properties.IsLeftButtonPressed)
            Window.BeginMoveDrag(e);
    }
}
