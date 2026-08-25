using Avalonia.Controls;
using System.ComponentModel;

namespace ReadMD;

public partial class MainWindow : Window
{
    private bool _allowClose = false;

    public MainWindow()
    {
        InitializeComponent();
    }

    public void AllowClose()
    {
        _allowClose = true;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!_allowClose)
        {
            // Отменяем закрытие и скрываем окно в трей
            e.Cancel = true;

            // Сначала закрываем файл и возвращаемся на начальную страницу
            if (DataContext is ViewModels.MainWindowViewModel mainWindowViewModel)
            {
                mainWindowViewModel.CloseFile();
            }

            // Даем UI время обновиться, затем скрываем окно
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                Hide();
            }, Avalonia.Threading.DispatcherPriority.Background);
        }
        base.OnClosing(e);
    }
}