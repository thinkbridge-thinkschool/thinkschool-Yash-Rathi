using Microsoft.EntityFrameworkCore;
using QuotesApi.Data;
using QuotesApi.Domain.Collections;

namespace QuotesApi.Repositories;

public class CollectionRepository : ICollectionRepository
{
    private readonly AppDbContext _db;

    public CollectionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Collection?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Collections
            .Include("Items")
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task AddAsync(Collection collection, CancellationToken ct = default)
    {
        _db.Collections.Add(collection);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Collection collection, CancellationToken ct = default)
    {
        _db.Collections.Update(collection);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var collection = await GetByIdAsync(id, ct);
        if (collection != null)
        {
            _db.Collections.Remove(collection);
            await _db.SaveChangesAsync(ct);
        }
    }
}