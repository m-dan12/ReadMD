using Avalonia;
using Avalonia.Styling;

namespace ReadMD.Services;

public interface IThemeService
{
    ThemeVariant CurrentTheme { get; }
    void SetTheme(ThemeVariant theme);
}

public class ThemeService : IThemeService
{
    public ThemeVariant CurrentTheme { get; private set; } = ThemeVariant.Default;

    public void SetTheme(ThemeVariant theme)
    {
        CurrentTheme = theme;
        Application.Current?.RequestedThemeVariant = theme;
    }
}