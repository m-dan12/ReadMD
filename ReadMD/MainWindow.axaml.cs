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
            Hide();
        }
        base.OnClosing(e);
    }
}