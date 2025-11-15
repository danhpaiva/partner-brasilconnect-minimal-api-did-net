using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Partner.BrasilConnect.Did.Api.Data;
using Partner.BrasilConnect.Did.Api.DTO;
using Partner.BrasilConnect.Did.Api.Enum;
using Partner.BrasilConnect.Did.Api.Models;
namespace Partner.BrasilConnect.Did.Api.Endpoints;

public static class DidActivationEndpoints
{
    public static void MapDidActivationEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/DidActivation").WithTags(nameof(DidActivation));

        group.MapPost("/request", async (DidCreationDto didCreationDto, AppDbContext db) =>
        {
            var didActivation = new DidActivation
            {
                DidNumber = didCreationDto.DidNumber,
                Status = DidStatus.Pending,
                CreatedAt = DateTime.UtcNow // Sempre use UTC
            };

            db.DidActivation.Add(didActivation);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/api/DidActivation/{didActivation.Id}", didActivation);
        })
        .WithName("RequestDidActivation")
        .WithSummary("Inicia uma nova ativação de DID apenas com o número.");

        group.MapPatch("/{id}/status", async Task<Results<Ok<DidActivation>, NotFound>> (int id, DidStatusUpdateDto updateDto, AppDbContext db) =>
        {
            var didActivation = await db.DidActivation
                .FirstOrDefaultAsync(model => model.Id == id);

            if (didActivation is null)
            {
                return TypedResults.NotFound();
            }

            didActivation.Status = updateDto.Status;
            didActivation.ErrorMessage = updateDto.ErrorMessage;
            didActivation.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return TypedResults.Ok(didActivation);
        })
        .WithName("UpdateDidActivationStatus")
        .WithSummary("Atualiza o status e mensagem de erro de uma ativação de DID.");

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
