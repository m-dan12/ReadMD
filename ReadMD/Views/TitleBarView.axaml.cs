using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using ReadMD.Services;
using ReadMD.ViewModels;

namespace ReadMD.Views;

public partial class TitleBarView : UserControl
{
    public TitleBarView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<TitleBarViewModel>();
    }

}