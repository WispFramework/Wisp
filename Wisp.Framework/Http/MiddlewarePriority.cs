namespace Wisp.Framework.Http;

public enum MiddlewarePriority
{
    Highest = 0,

    High = 100,

    Medium = 1_000,

    Low = 10_000,

    Lowest = 100_000
}