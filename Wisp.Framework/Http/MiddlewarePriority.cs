namespace Wisp.Framework.Http;

public struct MiddlewarePriority
{
    public int Value { get; }
    
    public MiddlewarePriority(int value) => Value = value;
    
    public bool Equals(MiddlewarePriority other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is MiddlewarePriority other && Equals(other);
    public override int GetHashCode() => Value;
    
    public int CompareTo(MiddlewarePriority other) => Value.CompareTo(other.Value);
    public static bool operator <(MiddlewarePriority left, MiddlewarePriority right) => left.Value < right.Value;
    public static bool operator >(MiddlewarePriority left, MiddlewarePriority right) => left.Value > right.Value;
    public static bool operator <=(MiddlewarePriority left, MiddlewarePriority right) => left.Value <= right.Value;
    public static bool operator >=(MiddlewarePriority left, MiddlewarePriority right) => left.Value >= right.Value;
    
    public static bool operator ==(MiddlewarePriority left, MiddlewarePriority right) => left.Equals(right);
    public static bool operator !=(MiddlewarePriority left, MiddlewarePriority right) => !left.Equals(right);
    
    public override string ToString() => Value.ToString();
    
    public static MiddlewarePriority Highest { get; } = new (0);
    public static MiddlewarePriority High {get;} = new(100);
    public static MiddlewarePriority Medium {get;} = new(1_000);
    public static MiddlewarePriority Low {get;} = new(10_000);
    public static MiddlewarePriority Lowest {get;} = new(100_000);

}