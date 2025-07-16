using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsDashboard.Common.DTO;

public class StoriesWithCount
{
    public IEnumerable<HNStory> Stories { get; set; } = null!;
    public int Count { get; set; }
}
