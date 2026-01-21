using System.Runtime.InteropServices;

namespace HardLinkTool.Library.Modules;

[StructLayout(LayoutKind.Auto)]
public readonly struct HardLinkEntry : IEquatable<HardLinkEntry>
{
    public string Target { get; }
    
    public string Output { get; }
    
    public HardLinkEntry(string target, string output)
    {
        Target = target;
        Output = output;
    }
    
    public static bool operator ==(HardLinkEntry left, HardLinkEntry right)
    {
        return left.Target == right.Target && left.Output == right.Output;
    }

    public static bool operator !=(HardLinkEntry left, HardLinkEntry right)
    {
        return !(left == right);
    }
    
    public override bool Equals(object? obj)
    {
        return obj is HardLinkEntry entry &&
               Target == entry.Target &&
               Output == entry.Output;
    }

    public bool Equals(HardLinkEntry other)
    {
        return Target == other.Target && Output == other.Output;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Target, Output);
    }
}