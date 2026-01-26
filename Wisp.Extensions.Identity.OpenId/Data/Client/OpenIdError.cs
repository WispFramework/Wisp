// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Extensions.Identity.OpenId.Data.Client;

public class OpenIdError
{
    
    public string ErrorMessage { get; set; } = "";

    public ErrorType ErrorType { get; set; } = ErrorType.Unknown;

    public int HttpStatusCode { get; set; } = 0;
    
    public string? Context { get; set; }
    
    public Exception? Exception { get; set; }
}