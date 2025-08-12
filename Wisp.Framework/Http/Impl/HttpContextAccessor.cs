namespace Wisp.Framework.Http.Impl;

public class HttpContextAccessor : IHttpContextAccessor
{

    private readonly AsyncLocal<IHttpContext> _currentContext = new();

    public Task SetContext(IHttpContext httpContext)
    {
        _currentContext.Value = httpContext;
        
        return Task.CompletedTask;
    }

    public Task<IHttpContext?> HttpContext => Task.FromResult(_currentContext.Value);
}