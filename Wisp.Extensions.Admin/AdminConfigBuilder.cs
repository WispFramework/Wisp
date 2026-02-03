using Microsoft.EntityFrameworkCore;

namespace Wisp.Extensions.Admin;

public class AdminConfigBuilder
{
    public class AdminConfig
    {
        public Type DbContextType { get; set; }
    }

    private AdminConfig _config = new();

    public AdminConfigBuilder WithDbContextType(Type dbContextType)
    {
        _config.DbContextType = dbContextType;
        return this;
    }
    
    public AdminConfig Build() => _config;
}