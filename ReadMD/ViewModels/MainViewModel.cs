using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ReadMD.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string title = "ReadMD";
}
