using System.ComponentModel.DataAnnotations;

namespace HackerNewsDashboard.Data.Models;

public class Rating
{
    [Key]
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string Username { get; set; } = null!;
    public int RatingStars { get; set; }
    public string RatingDateTime { get; set; } = null!;
}
