using Microsoft.EntityFrameworkCore;
using Partner.BrasilConnect.Did.Api.Data;
using Partner.BrasilConnect.Did.Api.Models;

namespace Partner.BrasilConnect.Did.Api.Tests.Endpoints.Test;

public class DidActivationTestsFixture : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _contextOptions;

    public List<DidActivation> SeedData => new List<DidActivation>
    {
        new DidActivation { Id = 1, DidNumber = "5511987654321", Status = Did.Api.Enum.DidStatus.Active, CreatedAt = DateTime.UtcNow.AddDays(-10) },
        new DidActivation { Id = 2, DidNumber = "5521999998888", Status = Did.Api.Enum.DidStatus.Pending, CreatedAt = DateTime.UtcNow.AddDays(-5) },
        new DidActivation { Id = 3, DidNumber = "5531888877777", Status = Did.Api.Enum.DidStatus.Failed, ErrorMessage = "Provisionamento falhou", CreatedAt = DateTime.UtcNow.AddDays(-2) }
    };

    public DidActivationTestsFixture()
    {
        _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"DidActivationTestDB_{Guid.NewGuid()}")
            .Options;

        using var context = new AppDbContext(_contextOptions);
        context.Database.EnsureCreated();
        context.SaveChanges();
    }

    public AppDbContext CreateContext()
    {
        var context = new AppDbContext(_contextOptions);

        context.DidActivation.RemoveRange(context.DidActivation);
        context.DidActivation.AddRange(SeedData);
        context.SaveChanges();

        return context;
    }

    public void Dispose()
    {
        using var context = new AppDbContext(_contextOptions);
        context.Database.EnsureDeleted();
        GC.SuppressFinalize(this);
    }
}