using OrderRefactorApi.Models;

namespace OrderRefactorApi.Strategies;

public interface IPricingStrategy
{
    bool AppliesTo(Customer customer);
    decimal CalculateLinePrice(Product product, int quantity);
}