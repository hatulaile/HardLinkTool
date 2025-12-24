using HardLinkTool.Features.Interfaces;

namespace HardLinkTool.Features;

public class OverwriteDisplay : IOverwriteDisplay
{
    private bool _isAlternate;

    private int _messageCount;

    public void Overwrite(string message)
    {
        if (!_isAlternate)
        {
            Console.Write("\e[?1049h");
            Console.SetCursorPosition(0,0);
        }
        Console.Write(message.Length < _messageCount ? new string(' ', _messageCount - message.Length) : message);
        _messageCount = message.Length;
    }

    public void Repetition()
    {
        Console.Write("\e[?1049l");
        _isAlternate = false;
    }
}