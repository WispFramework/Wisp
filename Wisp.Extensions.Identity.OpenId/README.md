# Wisp.Extensions.Identity.OpenId

This Wisp extension adds support for OIDC authentication to wisp. You only need to
point it to an OIDC-compliant provider and set the client name and secret. The
extension takes care of everything else.

## Installation

To install this extension, run:

```shell
dotnet add package Wisp.Extensions.Identity.OpenId
```

or add the following to your `csproj`

```xml
<ItemGroup>
    <PackageReference Include="Wisp.Extensions.Identity.OpenId" Version="<latest_version>" />
</ItemGroup>
```

please consult the [NuGet Package Website](https://www.nuget.org/packages/Wisp.Extensions.Identity.OpenId) for the latest version.

## Enabling

To add OpenID to your Wisp project, add the following code to your `Program.cs`:

```csharp
var hostBuilder = new WispHostBuilder();

hostBuilder.AddOpenIdConnect(config => 
{
    // You can either get configuration from a config file section
    // {
    //   "Wisp": {
    //     "Extensions": {
    //       "Identity" : {
    //         "OpenId": {
    //           "DiscoveryUrl": "http://localhost:8080/realms/master/.well-known/openid-configuration",
    //           "ClientId": "wisp_demo",
    //           "ClientSecret": "qF6LD3s2qORlIqCEB9LlP1mwuNEReqaa",
    //           "Scopes": "profile openid email"
    //         }
    //       }
    //     }
    //   }
    // }
    var configSection = hostBuilder.Configuration.GetSection("Wisp:Extensions:Identity:OpenId");
    config.FromConfig(configSection);
    
    // or configure things locally
    // config
    //   .SetSuccessRedirectUri("/")
    //   .SetErrorRedirectUri("/")
    //   .SetAuthUrl("/auth/oidc/authenticate")
    //   .SetCallbackUrl("/auth/oidc/callback")
    //   .SetLogoutUrl("/auth/oidc/logout")
    //   .SetDiscoveryUrl("https://oidc-provider.example.com/.well-known/openid-confiuration")
    //   .SetClientId("wisp_demo")
    //   .SetClientSecret("this_is_a_secret")
    //   .SetScopes("openid profile email");
        
    
    config.SetUsernameField(u => u.PreferredUsername);
    config.SetRolesClaimName("roles");
});

var appBuilder = hostBuilder.Build();

appBuilder.MapOpenIdConnect();

var app = appBuilder.Build();

await app.RunAsync();
```

`DiscoveryUrl`, `ClientId`, `ClientSecret` and `Scopes` are required, the rest of the config has sensible default that
should work out of the box (see below for details).

## Configuration

This is a list of available configuration keys and default values

| Configuration        | Description                                            | Required | Default                   |
|----------------------|--------------------------------------------------------|----------|---------------------------|
| `DiscoveryUrl`       | URL of the OIDC discovery document                     | Yes      |                           |
| `ClientId`           | The OpenID Client ID                                   | Yes      |                           |
| `ClientSecret`       | The OpenID Client Secret                               | Yes      |                           |
| `Scopes`             | List of required OpenID Scopes                         | Yes      |                           |
| `RolesClaimName`     | The name of the 'roles' claim                          | No       | `roles`                   |
| `SuccessRedirectUri` | Where the user will be redirected if the auth succeeds | No       | `/`                       |
| `ErroRedirectUri`    | Where the user fill be redirected if the auth fails    | No       | `/`                       |
| `AuthUrl`            | The route that will initiate IODC authentication       | No       | `/auth/oidc/authenticate` |
| `CallbackUrl`        | The route of the OIDC callback                         | No       | `/auth/oidc/callback`     |
| `LogoutUrl`          | The route that will delete the user's session          | No       | `/auth/oidc/logout`       |
