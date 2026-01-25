using System;
using System.Globalization;
using Avalonia.Data.Converters;
using HardLinkTool.Library.Enums;

namespace HardLinkTool.UI.Converters;

public class CreateHardLinkStateToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CreateHardLinkState state) return null;
        return state switch
        {
            CreateHardLinkState.None => "未完成",
            CreateHardLinkState.Completed => "已完成",
            CreateHardLinkState.Canceled => "已被取消",
            CreateHardLinkState.Failed => "发生严重错误",
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}