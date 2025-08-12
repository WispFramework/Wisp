using Wisp.Demo.Data.Models;
using Wisp.Demo.Services;
using Wisp.Framework.Controllers;
using Wisp.Framework.Http;
using Wisp.Framework.Middleware.Auth;
using Wisp.Framework.Middleware.Sessions;
using Wisp.Framework.Views;

namespace Wisp.Demo.Controllers.Admin;

[Controller]
public class AdminPostsController(PostsService ps, FlashService flashService) : ControllerBase
{
    [Authorize("admin")]
    [Route("/admin/posts")]
    public async Task<ViewResult> GetIndex()
    {
        var allPosts = await ps.GetPostsAsync();
        
        return View("admin/posts/index", new { Posts = allPosts });
    }

    [Authorize("admin")]
    [Route("/admin/posts/new")]
    public async Task<ViewResult> GetNewPost()
    {
        return View("admin/posts/new");
    }

    // [Authorize("admin")]
    // [Route("/admin/posts/new", "POST")]
    // public async Task<IView> SavePost(IHttpContextAccessor accessor, IAuthenticator authenticator)
    // {
    //     var context = await accessor.HttpContext;
    //     var formData = context?.Request.FormData;
    //     
    //     var title = formData?["title"] ?? "";
    //     var slug = formData?["slug"] ?? "";
    //     var content = formData?["content"] ?? "";
    //
    //     if (string.IsNullOrWhiteSpace(title))
    //     {
    //         await flashService.AddFlashMessage("Title is required.", FlashService.FlashMessageType.Error);
    //         return Redirect("/admin/posts/new");
    //     }
    //
    //     if (string.IsNullOrWhiteSpace(slug))
    //     {
    //         await flashService.AddFlashMessage("Slug is required.", FlashService.FlashMessageType.Error);
    //         return Redirect("/admin/posts/new");
    //     }
    //
    //     if (string.IsNullOrWhiteSpace(content))
    //     {
    //         await flashService.AddFlashMessage("Content is required.", FlashService.FlashMessageType.Error);
    //         return Redirect("/admin/posts/new");
    //     }
    //     
    //     var user = await authenticator.GetUser();
    //     if (user == null)
    //     {
    //         await flashService.AddFlashMessage("User is not authenticated.", FlashService.FlashMessageType.Error);
    //         return Redirect("/admin/posts/new");
    //     }
    //     
    //     var post = new Post
    //     {
    //         Title = title,
    //         Slug = slug,
    //         Content = content,
    //         Author = user
    //     }
    // }
}