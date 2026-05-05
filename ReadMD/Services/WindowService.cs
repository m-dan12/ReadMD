using Avalonia.Controls;
using Avalonia.Input;

namespace ReadMD.Services;

public interface IWindowService
{
    Window MainWindow { get; }
    void Minimize();
    void Maximize();
    void Restore();
    void Close();
}

public class WindowService(Window mainWindow) : IWindowService
{
    public Window MainWindow => mainWindow;

    public void Minimize() => mainWindow.WindowState = WindowState.Minimized;

    public void Maximize() => mainWindow.WindowState = WindowState.Maximized;

    public void Restore() => mainWindow.WindowState = WindowState.Normal;

    public void Close() => mainWindow.Close();

    // Метод для перетаскивания окна
    public void BeginMoveDrag(PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(mainWindow).Properties.IsLeftButtonPressed)
            mainWindow.BeginMoveDrag(e);
    }
}
