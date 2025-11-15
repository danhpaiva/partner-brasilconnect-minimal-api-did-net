using Microsoft.EntityFrameworkCore;

namespace Partner.BrasilConnect.Did.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext (DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Models.DidActivation> DidActivation { get; set; } = default!;
}
