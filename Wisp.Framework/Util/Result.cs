// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Util;

public record Result<T, TE>
{
    private T? _value;

    private TE? _error;

    public Result(T value)
    {
        _value = value;
    }

    public Result(TE error)
    {
        _error = error;
    }

    public bool Ok => _error == null;
    
    public T Value => _value ?? throw new InvalidOperationException("tried to access the value of a failed result");
    
    public TE Error => _error ?? throw new InvalidOperationException("tried to access the error of a success result");
    
    public static Result<T, TE> Success(T value) => new(value);
    
    public static Result<T, TE> Failure(TE error) => new(error);
}

public record Result<TE>
{
    private TE? _error;

    public Result() { }

    public Result(TE error)
    {
        _error = error;
    }

    public bool Ok => _error == null;
    
    public TE Error => _error ?? throw new InvalidOperationException("tried to access the error of a success result");
    
    public static Result<TE> Success() => new();
    
    public static Result<TE> Failure(TE error) => new(error);
}