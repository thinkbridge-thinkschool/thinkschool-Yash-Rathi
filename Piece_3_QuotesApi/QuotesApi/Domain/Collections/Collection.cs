namespace QuotesApi.Domain.Collections;

public class Collection
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int OwnerId { get; private set; }

    private readonly List<CollectionItem> _items = new();
    public IReadOnlyList<CollectionItem> Items => _items.AsReadOnly();

    // Required by EF Core
    private Collection() { }

    public Collection(string name, int ownerId)
    {
        SetName(name);
        OwnerId = ownerId;
    }

    public void Rename(string newName)
    {
        SetName(newName);
    }

    public void AddItem(int quoteId)
    {
        // Invariant 1: Max 50 items
        if (_items.Count >= 50)
            throw new DomainException("Collection cannot have more than 50 items.");

        // Invariant 2: No duplicate QuoteIds
        if (_items.Any(i => i.QuoteId == quoteId))
            throw new DomainException($"Quote {quoteId} is already in this collection.");

        _items.Add(new CollectionItem(quoteId, DateTime.UtcNow));
    }

    public void RemoveItem(int quoteId)
    {
        var item = _items.FirstOrDefault(i => i.QuoteId == quoteId);

        if (item == null)
            throw new DomainException($"Quote {quoteId} is not in this collection.");

        _items.Remove(item);
    }

    private void SetName(string name)
    {
        // Invariant 3: Name is 3-80 chars
        if (string.IsNullOrWhiteSpace(name) || name.Length < 3 || name.Length > 80)
            throw new DomainException("Collection name must be between 3 and 80 characters.");

        Name = name;
    }
}