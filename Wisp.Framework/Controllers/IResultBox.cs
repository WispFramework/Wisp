namespace Wisp.Framework.Controllers;

public interface IResultBox<T>
{
    T? Value { get; }

    Type ValueType { get; }
}