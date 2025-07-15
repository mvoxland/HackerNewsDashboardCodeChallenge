using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsDashboard.Common.DTO;

public class ProxyUser
{
    public int Id { get; set; }
    public int Created { get; set; }
    public int Karma { get; set; }
    public string? About { get; set; } = null!;
    public IEnumerable<int>? Submitted { get; set; }
}
