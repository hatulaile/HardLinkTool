using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia.Data.Converters;

namespace HardLinkTool.UI.Converters;

public class StopwatchToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Stopwatch stopwatch) return null;
        TimeSpan elapsed = stopwatch.Elapsed;
        return elapsed.ToString(elapsed.Hours > 0 ? @"hh\:mm\:ss" : @"mm\:ss");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}