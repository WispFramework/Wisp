namespace Wisp.Framework.Middleware;

public class CorsMiddlewareConfig
{
    public string AccessControlAllowOrigin { get; set; } = "*";
    
    public List<string>? AccessControlExposeHeaders { get; set; }
    
    public int? AccessControlMaxAge { get; set; }
    
    public bool? AccessControlAllowCredentials { get; set; }
    
    public List<string>? AccessControlAllowMethods { get; set; }
    
    public List<string>? AccessControlAllowHeaders { get; set; }
}