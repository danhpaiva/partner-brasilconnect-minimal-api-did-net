using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Partner.BrasilConnect.Did.Api.Data;
using Partner.BrasilConnect.Did.Api.DTO;
using Partner.BrasilConnect.Did.Api.Enum;
using Partner.BrasilConnect.Did.Api.Models;

namespace Partner.BrasilConnect.Did.Api.Endpoints;

public static class DidActivationEndpoints
{
    public static void MapDidActivationEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/DidActivation").WithTags(nameof(DidActivation));

        group.MapPost("/request", async (DidCreationDto didCreationDto, AppDbContext db) => await MapRequestDidActivation(didCreationDto, db))
        .WithName("RequestDidActivation")
        .WithSummary("Inicia uma nova ativação de DID apenas com o número.");

        group.MapPatch("/{id}/status", async Task<Results<Ok<DidActivation>, NotFound>> (int id, DidStatusUpdateDto updateDto, AppDbContext db) => await MapUpdateActivationStatus(id, updateDto, db))
        .WithName("UpdateDidActivationStatus")
        .WithSummary("Atualiza o status e mensagem de erro de uma ativação de DID.");

        group.MapGet("/", async (AppDbContext db) => await MapGetAllDidActivations(db))
        .WithName("GetAllDidActivations");

        group.MapGet("/{id}", async Task<Results<Ok<DidActivation>, NotFound>> (int id, AppDbContext db) => await MapGetActivationById(id, db))
        .WithName("GetDidActivationById");

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, DidActivation didActivation, AppDbContext db) => await MapUpdateActivation(id, didActivation, db))
        .WithName("UpdateDidActivation");

        group.MapPost("/", async (DidActivation didActivation, AppDbContext db) => await MapCreateDidActivation(didActivation, db))
        .WithName("CreateDidActivation");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) => await MapDeleteActivation(id, db))
        .WithName("DeleteDidActivation");
    }

    public static async Task<List<DidActivation>> MapGetAllDidActivations(AppDbContext db)
    {
        return await db.DidActivation.ToListAsync();
    }

    public static async Task<Results<Ok<DidActivation>, NotFound>> MapGetActivationById(int id, AppDbContext db)
    {
        return await db.DidActivation.AsNoTracking()
            .FirstOrDefaultAsync(model => model.Id == id)
            is DidActivation model
                ? TypedResults.Ok(model)
                : TypedResults.NotFound();
    }

    public static async Task<Created<DidActivation>> MapRequestDidActivation(DidCreationDto didCreationDto, AppDbContext db)
    {
        var didActivation = new DidActivation
        {
            DidNumber = didCreationDto.DidNumber,
            Status = DidStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        db.DidActivation.Add(didActivation);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/api/DidActivation/{didActivation.Id}", didActivation);
    }

    public static async Task<Results<Ok<DidActivation>, NotFound>> MapUpdateActivationStatus(int id, DidStatusUpdateDto updateDto, AppDbContext db)
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
    }

    public static async Task<Results<Ok, NotFound>> MapUpdateActivation(int id, DidActivation didActivation, AppDbContext db)
    {
        var existingActivation = await db.DidActivation.FindAsync(id);
        if (existingActivation is null) return TypedResults.NotFound();

        existingActivation.DidNumber = didActivation.DidNumber;
        existingActivation.Status = didActivation.Status;
        existingActivation.ErrorMessage = didActivation.ErrorMessage;
        existingActivation.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return TypedResults.Ok();
    }

    public static async Task<Created<DidActivation>> MapCreateDidActivation(DidActivation didActivation, AppDbContext db)
    {
        didActivation.CreatedAt = DateTime.UtcNow;
        db.DidActivation.Add(didActivation);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/api/DidActivation/{didActivation.Id}", didActivation);
    }

    public static async Task<Results<Ok, NotFound>> MapDeleteActivation(int id, AppDbContext db)
    {
        var didActivation = await db.DidActivation.FindAsync(id);
        if (didActivation is null) return TypedResults.NotFound();

        db.DidActivation.Remove(didActivation);
        await db.SaveChangesAsync();

        return TypedResults.Ok();
    }
}