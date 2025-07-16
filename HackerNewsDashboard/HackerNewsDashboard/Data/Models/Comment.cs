using System.ComponentModel.DataAnnotations;

namespace HackerNewsDashboard.Data.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string Email { get; set; } = null!;
    public string CommentText { get; set; } = null!;
}
