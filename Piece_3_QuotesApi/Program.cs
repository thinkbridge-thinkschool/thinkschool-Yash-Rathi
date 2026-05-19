using Microsoft.EntityFrameworkCore;
using QuotesApi.Data;
using QuotesApi.Domain;
using QuotesApi.Domain.Collections;
using QuotesApi.Endpoints;
using QuotesApi.Extensions;
using QuotesApi.Middleware;
using QuotesApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddProblemDetails();

// Register collection repository
builder.Services.AddScoped<ICollectionRepository, CollectionRepository>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.MapQuoteEndpoints();

// Create collection
app.MapPost("/collections", async (
    CreateCollectionRequest request,
    ICollectionRepository repo,
    CancellationToken ct) =>
{
    try
    {
        var collection = new Collection(request.Name, request.OwnerId);
        await repo.AddAsync(collection, ct);
        return Results.Created($"/collections/{collection.Id}", collection);
    }
    catch (DomainException ex)
    {
        return Results.Problem(ex.Message, statusCode: 400);
    }
});

// Add quote to collection
app.MapPost("/collections/{id}/items", async (
    int id,
    AddItemRequest request,
    ICollectionRepository repo,
    CancellationToken ct) =>
{
    try
    {
        var collection = await repo.GetByIdAsync(id, ct);
        if (collection == null)
            return Results.NotFound($"Collection {id} not found");

        collection.AddItem(request.QuoteId);
        await repo.UpdateAsync(collection, ct);
        return Results.Ok(collection);
    }
    catch (DomainException ex)
    {
        return Results.Problem(ex.Message, statusCode: 400);
    }
});

// Remove quote from collection
app.MapDelete("/collections/{id}/items/{quoteId}", async (
    int id,
    int quoteId,
    ICollectionRepository repo,
    CancellationToken ct) =>
{
    try
    {
        var collection = await repo.GetByIdAsync(id, ct);
        if (collection == null)
            return Results.NotFound($"Collection {id} not found");

        collection.RemoveItem(quoteId);
        await repo.UpdateAsync(collection, ct);
        return Results.Ok(collection);
    }
    catch (DomainException ex)
    {
        return Results.Problem(ex.Message, statusCode: 400);
    }
});

app.Run();

public record CreateCollectionRequest(string Name, int OwnerId);
public record AddItemRequest(int QuoteId);