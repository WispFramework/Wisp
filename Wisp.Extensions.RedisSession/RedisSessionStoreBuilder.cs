// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Extensions.RedisSession;

public class RedisSessionStoreBuilder
{
    private readonly RedisSessionStoreConfig _config = new();

    public RedisSessionStoreBuilder WithRedis(string connectionString)
    {
        _config.ConnectionString = connectionString;
        return this;
    }

    public RedisSessionStoreConfig Build()
    {
        return _config;
    }
}