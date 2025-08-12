using Microsoft.Extensions.Logging;
using Wisp.Framework.Http;

namespace Wisp.Framework.Middleware.Sessions;

public class FlashService(IHttpContextAccessor accessor, ILogger<FlashService> log)
{
    private Dictionary<string, List<FlashMessage>> _messages = new();

    public enum FlashMessageType
    {
        Primary,
        Info,
        Warning,
        Success,
        Error
    }

    /// <summary>
    /// Add a flash message with an arbitrary type (defaults to 'info')
    /// </summary>
    /// <param name="message">the message</param>
    /// <param name="type">arbitrary type</param>
    public async Task AddFlashMessage(string message, string type = "info")
    {
        var context = await accessor.HttpContext;
        if (context is null) return;
        
        var session = context.Session;
        if (session is null) return;

        var sessionId = session.Id;

        if (!_messages.TryGetValue(sessionId, out var list))
        {
            list = new();
            _messages[sessionId] = list;
        }
        
        list.Add(new FlashMessage(sessionId, message, type));
    }

    /// <summary>
    /// Add a flash message with a built-in type
    /// </summary>
    /// <param name="message">the message</param>
    /// <param name="type">built-in type</param>
    public async Task AddFlashMessage(string message, FlashMessageType type)
    {
        var typeString = type switch
        {
            FlashMessageType.Primary => "primary",
            FlashMessageType.Info => "info",
            FlashMessageType.Success => "success",
            FlashMessageType.Warning => "warning",
            FlashMessageType.Error => "danger",
        };
        
        await AddFlashMessage(message, typeString);
    }

    public async Task<List<FlashMessage>?> GetAllAndDelete()
    {
        var context = await accessor.HttpContext;
        if (context is null) return null;

        var session = context.Session;
        if (session is null) return null;
        
        log.LogDebug("getting all flashes for {SessionId}", session.Id);

        var sessionId = session.Id;

        if (!_messages.TryGetValue(sessionId, out var list) || list.Count == 0)
        {
            log.LogDebug("nothing found");
            return new List<FlashMessage>();
        }
        
        // Take all messages and clear them
        var toReturn = new List<FlashMessage>(list);
        list.Clear();
        
        log.LogDebug("found {N} messages", toReturn.Count);

        return toReturn;
    }


    
    public record FlashMessage(string SessionId, string Message, string Type);
}