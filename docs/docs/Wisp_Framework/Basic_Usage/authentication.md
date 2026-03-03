---
icon: lucide/user
---

# Authentication

Wisp has the functionality to authenticate users and authorize requests.

!!! warning
    The base Wisp framework does not ship with any kind of user management or identity provider.

    You will either need to implement this functionality yourself, or use an extension, such as
    the [OpenID Extension](../../Wisp_Extensions/OpenID/openid) or the [Identity Extension](../../Wisp_Extensions/Identity/about).

## Authenticating Users

To authenticate users, you will need an `IAuthenticator`. Wisp ships with a default implementation
in `BasicAuthenticator`. The basic authenticator handles session storage and principal management.
Credential verification is entirely up to your application.

The basic authenticator needs a session store. You can use the built-in `InMemorySessionStore` from
Wisp, add one with an extension or implement one yourself.

1. Enable the basic authenticator
   ```csharp
   hostBuilder.UseInMemorySession();
   hostBuilder.UseBasicAuth();
   ```

2. Build a principal and authenticate the user
   ```csharp
   [Route("/auth", "POST")]
   public async Task<ViewResult> Authenticate(string username, string password, AuthService authService, IAuthenticator authenticator)
   {
      // Managing users and verifying credentials is up to you!
      var (user, ok) = await authService.Authenticate(username, password);

      if(ok)
      {
         var principal = new UserPrincipal 
         {
            Username = user.Username,
            Roles = user.Roles,
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
         };

         await authenticator.Authenticate(principal);

         return Redirect("/");
      } 
      else {
        return Redirect("/error");
      }
   }
   ```

## Authorize Routes

You can enable authorization for a specific route or an entire controller with the `[Authorize]` attribute.

You can optionally specify a list of allowed roles. By default, any authenticated user will be allowed.
When one or more roles are specified, only principals with at least one matching role will be allowed.

The `[Authorize]` attribute can be applied to individual routes or entire controllers.

```csharp
[Route("/")]
public ViewResult Public() => View("index");

[Authorize]
[Route("/profile")]
public ViewResult AnyLoggedIn() => View("profile");

[Authorize(["admin"])]
[Route("/admin")]
public ViewResult OneRoleOnly() => View("admin");

[Authorize(["admin", "editor"])]
[Route("/settings")]
public ViewResult MultipleRoles() => View("settings");
```

## Get User Principal

You can retrieve the current user principal from the authenticator service using the `GetUser()` method.
It will return `null` if the user is not authenticated.

Keep in mind that mapping the principal to a user object from a database or another source is up to you. 

```csharp
[Authorize]
[Route("/profile")]
public async Task<ViewResult> GetProfile(AuthService authService, UserService userService)
{
    var principal = await auth.GetUser();
    if(principal is null) return Redirect("/login");

    var dbUser = await userService.Find(principal);
    if(dbUser is null) return Redirect("/login");

    return View("profile", new { User = dbUser });
}
```

## De-Authenticate Users

To deauthenticate a user, simply call the `IAuthenticator.Deauthenticate()` method. This removes the current 
principal from the configured session store, effectively ending the user's authenticated session.