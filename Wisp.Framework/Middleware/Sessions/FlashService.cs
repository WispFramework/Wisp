// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Microsoft.Extensions.Logging;
using Wisp.Framework.Http;

namespace Wisp.Framework.Middleware.Sessions;

public class FlashService(ISessionAccessor sessionAccessor, ILogger<FlashService> log)
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
    public void AddFlashMessage(string message, string type = "info")
    {
        // var context = accessor.HttpContext;
        // if (context is null) return;
        //
        // var session = context.Session;
        // if (session is null) return;
        //
        // var sessionId = session.Id;
        
        var sessionId = sessionAccessor.GetSessionId().ConfigureAwait(true).GetAwaiter().GetResult();
        if (sessionId is null) return;

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
    public void AddFlashMessage(string message, FlashMessageType type)
    {
        var typeString = type switch
        {
            FlashMessageType.Primary => "primary",
            FlashMessageType.Info => "info",
            FlashMessageType.Success => "success",
            FlashMessageType.Warning => "warning",
            FlashMessageType.Error => "danger",
        };
        
        AddFlashMessage(message, typeString);
    }

    public List<FlashMessage>? GetAllAndDelete()
    {
        var sessionId = sessionAccessor.GetSessionId().ConfigureAwait(true).GetAwaiter().GetResult();
        if(sessionId is null) return null;

        if (!_messages.TryGetValue(sessionId, out var list) || list.Count == 0)
        {
            log.LogDebug("nothing found");
            return [];
        }
        
        // Take all messages and clear them
        var toReturn = new List<FlashMessage>(list);
        list.Clear();

        return toReturn;
    }


    
    public record FlashMessage(string SessionId, string Message, string Type);
}