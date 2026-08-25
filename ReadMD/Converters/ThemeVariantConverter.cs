using Avalonia.Data.Converters;
using Avalonia.Styling;
using System;
using System.Globalization;

namespace ReadMD.Converters;

public class ThemeVariantConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ThemeVariant themeVariant && parameter is string expectedTheme)
        {
            return expectedTheme.ToLower() switch
            {
                "light" => themeVariant == ThemeVariant.Light,
                "dark" => themeVariant == ThemeVariant.Dark,
                _ => false
            };
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
