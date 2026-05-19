using OrderRefactorApi.Models;
using OrderRefactorApi.Constants;

namespace OrderRefactorApi.Strategies;

public class PremiumPricingStrategy : IPricingStrategy
{
    public bool AppliesTo(Customer customer)
        => customer.Type == CustomerType.Premium;

    public decimal CalculateLinePrice(Product product, int quantity)
    {
        decimal unitPrice = product.Price;

        if (quantity < PricingConstants.MinQuantityForSmallDiscount)
            return unitPrice * quantity;

        unitPrice = product.Category switch
        {
            ProductCategory.Electronics =>
                unitPrice * PricingConstants.PremiumCustomerElectronicsDiscount,
            ProductCategory.Books => quantity >= PricingConstants.BulkThreshold1
                ? unitPrice * PricingConstants.PremiumCustomerBooksHighQuantityDiscount
                : unitPrice * PricingConstants.PremiumCustomerBooksDiscount,
            _ => unitPrice * PricingConstants.PremiumCustomerBaseDiscount
        };

        return unitPrice * quantity;
    }
}