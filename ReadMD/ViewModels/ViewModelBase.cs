using CommunityToolkit.Mvvm.ComponentModel;

namespace ReadMD.ViewModels;

/// <summary>
/// Базовый класс для всех ViewModel приложения.
/// Обеспечивает поддержку INotifyPropertyChanged через CommunityToolkit.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
}
