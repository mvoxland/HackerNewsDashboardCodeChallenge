using Microsoft.EntityFrameworkCore;
using HackerNewsDashboard.Common.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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
}
