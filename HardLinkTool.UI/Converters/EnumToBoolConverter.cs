using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace HardLinkTool.UI.Converters;

public class EnumToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Enum enumValue)
            return false;

        return enumValue.GetHashCode() != 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}