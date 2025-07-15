using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsDashboard.Common.DTO;

public class ProxyItem
{
    public int Id { get; set; }
    public string By { get; set; } = null!;
    public string Type { get; set; } = null!;
    public bool? Deleted { get; set; }
    public int? Time { get; set; }
    public string? Text { get; set; }
    public bool? Dead { get; set; }
    public int? Parent { get; set; }
    public int? Poll { get; set; }
    public IEnumerable<int>? Kids { get; set; }
    public string? Url { get; set; }
    public int? Score { get; set; }
    public string? Title { get; set; }
    public IEnumerable<int>? Parts { get; set; }
    public IEnumerable<int>? Descendants { get; set; }

}
