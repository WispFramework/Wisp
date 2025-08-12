using Wisp.Demo.Services;
using Wisp.Framework.Controllers;
using Wisp.Framework.Http;
using Wisp.Framework.Middleware.Sessions;
using Wisp.Framework.Views;

namespace Wisp.Demo.Controllers;

[Controller]
public class AuthController(AuthService authService, FlashService flashService) : ControllerBase
{
    [Route("/auth/login")]
    public async Task<IView> GetLogin(IHttpContextAccessor accessor)
    {
        var context = await accessor.HttpContext;
        var error = context?.Request.QueryParams.GetValueOrDefault("error");
        
        return View("auth/login", new { error });
    }
    
    [Route("/auth/login", "POST")]
    public async Task<IView> PostLogin(IHttpContextAccessor accessor)
    {
        var context = await accessor.HttpContext;
        
        var username = context.Request.FormData["username"];
        var password = context.Request.FormData["password"];

        if (await authService.Login(username, password))
        {
            await flashService.AddFlashMessage($"Welcome back, {username}!", FlashService.FlashMessageType.Success);
            return Redirect("/");
        }

        await flashService.AddFlashMessage("Invalid username or password", FlashService.FlashMessageType.Error);
        return Redirect("/auth/login");
    }

    [Route("/auth/signup", "POST")]
    public async Task<IView> Signup(IHttpContextAccessor accessor)
    {
        var context = await accessor.HttpContext;
        
        var username = context.Request.FormData["username"];
        var password = context.Request.FormData["password"];
        var passwordConfirm = context.Request.FormData["password-check"];

        if (password != passwordConfirm) return Redirect("/?error=passwords don't match");
        
        var result = await authService.Register(username, password);
        if(!result) return Redirect("/?error=username taken");
        
        return Redirect("/");
    }
    
    [Route("/auth/logout")]
    public async Task<IView> Logout(IHttpContext context)
    {
        await authService.Logout();
        await flashService.AddFlashMessage("Goodbye!", FlashService.FlashMessageType.Success);
        return Redirect("/");
    }
}