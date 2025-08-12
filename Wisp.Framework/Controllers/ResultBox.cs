namespace Wisp.Framework.Controllers;

public class ResultBox<T> : IResultBox<T>
{
    public ResultBox(T value) => Value = value;

    public ResultBox() {}

    public T? Value { get; set; }

    public Type ValueType => typeof(T);
}