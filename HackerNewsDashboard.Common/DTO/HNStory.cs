using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsDashboard.Common.DTO;

public class HNStory
{
    public int Id { get; set; }
    public string By { get; set; } = null!;
    public long? Time { get; set; }
    public IEnumerable<int>? Kids { get; set; }
    public string? Url { get; set; }
    public int? Score { get; set; }
    public string? Title { get; set; }
    public int? Descendants { get; set; }

    public IEnumerable<HNComment>? HNComments { get; set; }
    public IEnumerable<UserComment>? UserComments { get; set; }
    public UserRating? UserRating { get; set; }
}
