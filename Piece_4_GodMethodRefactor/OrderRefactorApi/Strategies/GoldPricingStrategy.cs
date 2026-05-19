using OrderRefactorApi.Models;
using OrderRefactorApi.Constants;

namespace OrderRefactorApi.Strategies;

public class GoldPricingStrategy : IPricingStrategy
{
    public bool AppliesTo(Customer customer)
        => customer.Type == CustomerType.Gold;

    public decimal CalculateLinePrice(Product product, int quantity)
    {
        decimal unitPrice = product.Price;

        if (quantity > PricingConstants.MinQuantityForGoldDiscount)
            unitPrice *= PricingConstants.GoldCustomerDiscount;

        return unitPrice * quantity;
    }
}