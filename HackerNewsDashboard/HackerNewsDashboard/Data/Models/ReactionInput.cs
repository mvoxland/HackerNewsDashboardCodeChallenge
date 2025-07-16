namespace HackerNewsDashboard.Data.Models;

public class ReactionInput
{
    public int ItemId { get; set; }
    public string Username { get; set; } = null!;
    public float Label { get; set; }
}
