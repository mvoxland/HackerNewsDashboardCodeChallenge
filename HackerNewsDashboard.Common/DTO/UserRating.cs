using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsDashboard.Common.DTO;

public class UserRating
{
    public int ItemId { get; set; }
    public string Username { get; set; } = null!;
    public int RatingStars { get; set; }
    public DateTime RatingDateTime { get; set; }
}
