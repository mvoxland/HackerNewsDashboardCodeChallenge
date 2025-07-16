using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsDashboard.Data.Models;

public class TokenInfo
{
    [Key]
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string ExpiredAt { get; set; } = null!;

}
