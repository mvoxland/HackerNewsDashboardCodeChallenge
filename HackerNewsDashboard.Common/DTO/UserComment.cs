using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsDashboard.Common.DTO;

public class UserComment
{
    public int ItemId { get; set; }
    public string Username { get; set; } = null!;
    public string CommentText { get; set; } = null!;
    public DateTime CommentDateTime { get; set; }
}
