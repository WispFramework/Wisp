// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Extensions.Identity.OpenId.Data.Client;

public class OpenIdAuthCall
{
    public required string AuthUrl { get; set; }
    
    public required string Verifier { get; set; }
    
    public required string VerifierHash { get; set; }
    
    public required string Nonce { get; set; }
}