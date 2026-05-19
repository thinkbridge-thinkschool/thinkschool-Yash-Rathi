namespace QuotesApi.Domain.Collections;

public sealed class CollectionItem
{
    public int QuoteId { get; }
    public DateTime AddedAt { get; }

    public CollectionItem(int quoteId, DateTime addedAt)
    {
        if (quoteId <= 0)
            throw new ArgumentException("QuoteId must be positive", nameof(quoteId));

        QuoteId = quoteId;
        AddedAt = addedAt;
    }

    // Value objects are equal if their values are equal
    public override bool Equals(object? obj)
        => obj is CollectionItem other && QuoteId == other.QuoteId;

    public override int GetHashCode()
        => QuoteId.GetHashCode();
}