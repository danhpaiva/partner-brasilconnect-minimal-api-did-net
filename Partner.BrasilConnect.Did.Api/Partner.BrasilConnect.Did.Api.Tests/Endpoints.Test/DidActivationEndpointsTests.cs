using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Partner.BrasilConnect.Did.Api.DTO;
using Partner.BrasilConnect.Did.Api.Endpoints;
using Partner.BrasilConnect.Did.Api.Enum;
using Partner.BrasilConnect.Did.Api.Models;

namespace Partner.BrasilConnect.Did.Api.Tests.Endpoints.Test;

[Collection("DidActivationTests")]
public class DidActivationEndpointsTests
{
    private readonly DidActivationTestsFixture _fixture;

    public DidActivationEndpointsTests(DidActivationTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAllDidActivations_ShouldReturnAllItems()
    {
        using var db = _fixture.CreateContext();
        var result = await DidActivationEndpoints.MapGetAllDidActivations(db);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetDidActivationById_ShouldReturnItem_WhenFound()
    {
        using var db = _fixture.CreateContext();
        var id = 1;

        var result = await DidActivationEndpoints.MapGetActivationById(id, db);

        var okResult = Assert.IsType<Ok<DidActivation>>(result.Result);
        Assert.Equal(id, okResult.Value!.Id);
        Assert.Equal("5511987654321", okResult.Value.DidNumber);
    }

    [Fact]
    public async Task GetDidActivationById_ShouldReturnNotFound_WhenNotFound()
    {
        using var db = _fixture.CreateContext();
        var nonExistentId = 99;

        var result = await DidActivationEndpoints.MapGetActivationById(nonExistentId, db);

        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task RequestDidActivation_ShouldCreateNewActivation_WithPendingStatus()
    {
        using var db = _fixture.CreateContext();
        var newDidNumber = "5541777766666";
        var creationDto = new DidCreationDto { DidNumber = newDidNumber };
        var initialCount = db.DidActivation.Count();

        var result = await DidActivationEndpoints.MapRequestDidActivation(creationDto, db);

        var createdResult = Assert.IsType<Created<DidActivation>>(result);
        var createdActivation = createdResult.Value;

        Assert.Equal(initialCount + 1, db.DidActivation.Count());
        Assert.Equal(newDidNumber, createdActivation!.DidNumber);
        Assert.Equal(DidStatus.Pending, createdActivation.Status);
        Assert.True(createdActivation.Id > 3);
    }

    [Theory]
    [InlineData(2, DidStatus.Active, null)]
    [InlineData(3, DidStatus.Failed, "Novo erro de callback")]
    public async Task UpdateDidActivationStatus_ShouldUpdateStatusAndMessage(int id, DidStatus newStatus, string? newErrorMessage)
    {
        using var db = _fixture.CreateContext();
        var updateDto = new DidStatusUpdateDto { Status = newStatus, ErrorMessage = newErrorMessage };

        var result = await DidActivationEndpoints.MapUpdateActivationStatus(id, updateDto, db);

        var okResult = Assert.IsType<Ok<DidActivation>>(result.Result);
        var updatedActivation = okResult.Value;

        Assert.Equal(newStatus, updatedActivation!.Status);
        Assert.Equal(newErrorMessage, updatedActivation.ErrorMessage);
        Assert.NotNull(updatedActivation.UpdatedAt);
    }

    [Fact]
    public async Task UpdateDidActivationStatus_ShouldReturnNotFound_WhenIdIsInvalid()
    {
        using var db = _fixture.CreateContext();
        var nonExistentId = 99;
        var updateDto = new DidStatusUpdateDto { Status = DidStatus.Active };

        var result = await DidActivationEndpoints.MapUpdateActivationStatus(nonExistentId, updateDto, db);

        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task UpdateDidActivation_ShouldUpdateAllFields()
    {
        using var db = _fixture.CreateContext();
        var idToUpdate = 2;
        var newDidNumber = "5521912345678";
        var updateData = new DidActivation
        {
            Id = idToUpdate,
            DidNumber = newDidNumber,
            Status = DidStatus.InProgress,
            ErrorMessage = "Em re-processamento",
            CreatedAt = DateTime.UtcNow.AddYears(-1)
        };

        var result = await DidActivationEndpoints.MapUpdateActivation(idToUpdate, updateData, db);

        Assert.IsType<Ok>(result.Result);

        var updatedItem = await db.DidActivation.FindAsync(idToUpdate);
        Assert.NotNull(updatedItem);
        Assert.Equal(newDidNumber, updatedItem.DidNumber);
        Assert.Equal(DidStatus.InProgress, updatedItem.Status);
        Assert.Equal("Em re-processamento", updatedItem.ErrorMessage);
        Assert.NotNull(updatedItem.UpdatedAt);
    }

    [Fact]
    public async Task UpdateDidActivation_ShouldReturnNotFound_WhenIdIsInvalid()
    {
        using var db = _fixture.CreateContext();
        var nonExistentId = 99;
        var updateData = new DidActivation
        {
            Id = nonExistentId,
            DidNumber = "5511000000000",
            Status = DidStatus.Active
        };

        var result = await DidActivationEndpoints.MapUpdateActivation(nonExistentId, updateData, db);

        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task DeleteDidActivation_ShouldRemoveItem_WhenFound()
    {
        using var db = _fixture.CreateContext();
        var idToDelete = 1;
        var initialCount = db.DidActivation.Count();

        var result = await DidActivationEndpoints.MapDeleteActivation(idToDelete, db);

        Assert.IsType<Ok>(result.Result);

        Assert.Equal(initialCount - 1, db.DidActivation.Count());
        Assert.Null(await db.DidActivation.FindAsync(idToDelete));
    }

    [Fact]
    public async Task DeleteDidActivation_ShouldReturnNotFound_WhenNotFound()
    {
        using var db = _fixture.CreateContext();
        var nonExistentId = 99;

        var result = await DidActivationEndpoints.MapDeleteActivation(nonExistentId, db);

        Assert.IsType<NotFound>(result.Result);
    }
}