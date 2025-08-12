namespace Wisp.Framework.Http;

public interface IHttpContextAccessor
{
    Task SetContext(IHttpContext httpContext);
    
    Task<IHttpContext?> HttpContext { get; }
}