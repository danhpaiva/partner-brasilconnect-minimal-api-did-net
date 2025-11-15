using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Partner.BrasilConnect.Did.Api.Data;
using Partner.BrasilConnect.Did.Api.Models;
namespace Partner.BrasilConnect.Did.Api.Endpoints;

public static class DidActivationEndpoints
{
    public static void MapDidActivationEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/DidActivation").WithTags(nameof(DidActivation));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.DidActivation.ToListAsync();
        })
        .WithName("GetAllDidActivations");

        group.MapGet("/{id}", async Task<Results<Ok<DidActivation>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.DidActivation.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is DidActivation model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetDidActivationById");

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, DidActivation didActivation, AppDbContext db) =>
        {
            var affected = await db.DidActivation
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, didActivation.Id)
                    .SetProperty(m => m.DidNumber, didActivation.DidNumber)
                    .SetProperty(m => m.Status, didActivation.Status)
                    .SetProperty(m => m.ErrorMessage, didActivation.ErrorMessage)
                    .SetProperty(m => m.CreatedAt, didActivation.CreatedAt)
                    .SetProperty(m => m.UpdatedAt, didActivation.UpdatedAt)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateDidActivation");

        group.MapPost("/", async (DidActivation didActivation, AppDbContext db) =>
        {
            db.DidActivation.Add(didActivation);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/DidActivation/{didActivation.Id}",didActivation);
        })
        .WithName("CreateDidActivation");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.DidActivation
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteDidActivation");
    }
}
