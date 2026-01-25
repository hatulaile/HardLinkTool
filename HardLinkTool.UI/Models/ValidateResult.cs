using System;
using Color = Avalonia.Media.Color;

namespace HardLinkTool.UI.Models;

public readonly struct ValidateResult : IEquatable<ValidateResult>
{
    public bool IsValid { get; }

    public string Message { get; }

    public Color MessageColor { get; }

    public ValidateResult(bool isValid, string message, Color messageColor)
    {
        IsValid = isValid;
        Message = message;
        MessageColor = messageColor;
    }

    public override bool Equals(object? obj)
    {
        return obj is ValidateResult other && Equals(other);
    }

    public bool Equals(ValidateResult other)
    {
        return IsValid == other.IsValid && Message == other.Message && MessageColor.Equals(other.MessageColor);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsValid, Message, MessageColor);
    }

    public static bool operator ==(ValidateResult left, ValidateResult right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ValidateResult left, ValidateResult right)
    {
        return !(left == right);
    }
}