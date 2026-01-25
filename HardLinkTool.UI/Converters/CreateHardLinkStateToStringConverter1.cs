using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using HardLinkTool.Library.Enums;

namespace HardLinkTool.UI.Converters;

public class CreateHardLinkStateToStringConverter1 : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CreateHardLinkState state) return null;
        return state switch
        {
            CreateHardLinkState.None => new ImmutableSolidColorBrush(Colors.Gray),
            CreateHardLinkState.Completed => new ImmutableSolidColorBrush(Colors.DarkGreen),
            CreateHardLinkState.Canceled => new ImmutableSolidColorBrush(Colors.Yellow),
            CreateHardLinkState.Failed => new ImmutableSolidColorBrush(Colors.PaleVioletRed),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}