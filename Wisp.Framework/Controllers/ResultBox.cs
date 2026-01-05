// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Controllers;

public class ResultBox<T> : IResultBox<T>
{
    public ResultBox(T value) => Value = value;

    public ResultBox() {}

    public T? Value { get; set; }

    public Type ValueType => typeof(T);
}

public class ResultBox(object value) : ResultBox<object>(value) {}