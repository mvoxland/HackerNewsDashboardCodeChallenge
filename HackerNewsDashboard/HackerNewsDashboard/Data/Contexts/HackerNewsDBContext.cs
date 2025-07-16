using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HackerNewsDashboard.Data.Models;

namespace HackerNewsDashboard.Data.Contexts;

public class HackerNewsDBContext: IdentityDbContext<User>
{
    protected readonly IConfiguration Configuration;

    public HackerNewsDBContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(Configuration.GetConnectionString("APIDashboardDB"));
    }

    public DbSet<TokenInfo> TokenInfo { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Rating> Ratings { get; set; }
}
