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