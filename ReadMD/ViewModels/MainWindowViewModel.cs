using CommunityToolkit.Mvvm.ComponentModel;
using ReadMD.Services;

namespace ReadMD.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private TitleBarViewModel titleBarViewModel;

    [ObservableProperty]
    private MainViewModel mainViewModel;

    public MainWindowViewModel(
        TitleBarViewModel titleBarViewModel,
        MainViewModel mainViewModel)
    {
        TitleBarViewModel = titleBarViewModel;
        MainViewModel = mainViewModel;
    }
}