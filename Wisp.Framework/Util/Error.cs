using System.Text.Json.Serialization;

namespace Wisp.Framework.Util;

public class Error
{
    public string Message { get; set; } = "";
    
    public int Code { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Exception? Exception { get; set; }

    public Error() {}

    public Error(int code, string message, string? description = null, Exception? exception = null)
    {
        Code = code;
        Message = message;
        Description = description;
        Exception = exception;
    }
}