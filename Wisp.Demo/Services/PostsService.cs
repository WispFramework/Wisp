using Microsoft.EntityFrameworkCore;
using Wisp.Demo.Data;
using Wisp.Demo.Data.Models;

namespace Wisp.Demo.Services;

public class PostsService(AppDbContext db)
{
    public async Task<List<Post>> GetPostsAsync() => await db.Posts.ToListAsync();

    public async Task<(bool, string?)> CreatePostAsync(Post post)
    {
        if(db.Posts.Any(p => p.Slug == post.Slug)) return (false, "a post with this slug already exists");
        db.Posts.Add(post);
        await db.SaveChangesAsync();
        return (true, null);
    }
}