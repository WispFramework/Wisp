using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wisp.Demo.Data.Models;

public class Post
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public required string Title { get; set; }
    
    public required string Slug { get; set; }
    
    public required string Content { get; set; }
    
    public User Author { get; set; }
    
    public required DateTime PublishDate { get; set; }
    
    public int AuthorId { get; set; }
}