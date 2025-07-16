using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsDashboard.Common.DTO;

public class HNComment
{
    public int Id { get; set; }
    public string By { get; set; } = null!;
    public long? Time { get; set; }
    public string? Text { get; set; }
    public int? Parent { get; set; }
    public IEnumerable<int>? Kids { get; set; }
}
