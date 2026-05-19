using OrderRefactorApi.Models;
using OrderRefactorApi.Constants;

namespace OrderRefactorApi.Strategies;

public class StandardPricingStrategy : IPricingStrategy
{
    public bool AppliesTo(Customer customer)
        => customer.Type == CustomerType.Standard;

    public decimal CalculateLinePrice(Product product, int quantity)
    {
        decimal unitPrice = product.Price;

        if (quantity >= PricingConstants.MinQuantityForLargeDiscount)
            unitPrice *= PricingConstants.LargeDiscountRate;
        else if (quantity >= PricingConstants.MinQuantityForMediumDiscount)
            unitPrice *= PricingConstants.MediumDiscountRate;
        else if (quantity >= PricingConstants.MinQuantityForSmallDiscount)
            unitPrice *= PricingConstants.SmallDiscountRate;

        return unitPrice * quantity;
    }
}