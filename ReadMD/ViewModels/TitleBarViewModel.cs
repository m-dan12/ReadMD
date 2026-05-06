using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentIcons.Common;
using ReadMD.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReadMD.ViewModels;

public partial class TitleBarViewModel : ViewModelBase
{
    private readonly IThemeService _themeService;

    [ObservableProperty] private string _appTitle = "MyApp";
    [ObservableProperty] private bool _isMenuOpen = false;
    [ObservableProperty] private bool _isDarkTheme;
    [ObservableProperty] private bool _isEnglish = true;
    [ObservableProperty] private bool _isRussian;
    [ObservableProperty] private Icon _maximizeIcon = Icon.Maximize;
    

    public TitleBarViewModel(IThemeService themeService)
    {
        _themeService = themeService;
        _isDarkTheme = themeService.CurrentTheme == ThemeVariant.Dark;
    }

    private static Window? GetWindow() =>
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
        ?.MainWindow;
    [RelayCommand]
    private void CloseMenu() => IsMenuOpen = false;

    [RelayCommand]
    private void Minimize() => GetWindow()?.WindowState = WindowState.Minimized;

    [RelayCommand]
    private void Maximize()
    {
        var w = GetWindow();
        if (w is null) return;
        if (w.WindowState == WindowState.Maximized)
        {
            w.WindowState = WindowState.Normal;
            MaximizeIcon = Icon.Maximize;
        }
        else
        {
            w.WindowState = WindowState.Maximized;
            MaximizeIcon = Icon.SquareMultiple;
        }
    }

    [RelayCommand]
    private void Close() => GetWindow()?.Close();

    partial void OnIsDarkThemeChanged(bool value)
    {
        _themeService.SetTheme(value ? ThemeVariant.Dark : ThemeVariant.Light);
    }

    [RelayCommand]
    private void SetLanguage(string lang)
    {
        IsEnglish = lang == "en";
        IsRussian = lang == "ru";
        IsMenuOpen = false;
    }
}
