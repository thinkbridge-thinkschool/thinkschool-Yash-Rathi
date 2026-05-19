using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using OrderRefactorApi.Services;
using OrderRefactorApi.Repositories;
using OrderRefactorApi.Models;
using OrderRefactorApi.Dtos;
using OrderRefactorApi.Data;
using Microsoft.EntityFrameworkCore;

namespace OrderRefactorApi.Tests;

public class OrderServiceTests
{
    private OrderService CreateService(
        Mock<IOrderRepository>? orderRepo = null,
        Mock<IProductRepository>? productRepo = null,
        Mock<ICustomerRepository>? customerRepo = null,
        Mock<IInventoryService>? inventoryService = null,
        Mock<IDiscountRepository>? discountRepo = null,
        Mock<IPricingService>? pricingService = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(options);

        return new OrderService(
            orderRepo?.Object ?? Mock.Of<IOrderRepository>(),
            productRepo?.Object ?? Mock.Of<IProductRepository>(),
            customerRepo?.Object ?? Mock.Of<ICustomerRepository>(),
            inventoryService?.Object ?? Mock.Of<IInventoryService>(),
            discountRepo?.Object ?? Mock.Of<IDiscountRepository>(),
            pricingService?.Object ?? Mock.Of<IPricingService>(),
            db,
            Mock.Of<ILogger<OrderService>>());
    }

    [Fact]
    public async Task CreateOrderAsync_ThrowsWhenCustomerNotFound()
    {
        var customerRepo = new Mock<ICustomerRepository>();
        customerRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var service = CreateService(customerRepo: customerRepo);
        var items = new List<OrderItemRequest> { new() { ProductId = 1, Quantity = 1 } };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateOrderAsync(99, items, "123 Main St", null));
    }

    [Fact]
    public async Task CreateOrderAsync_ThrowsWhenProductNotFound()
    {
        var customerRepo = new Mock<ICustomerRepository>();
        customerRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer { Id = 1, Type = CustomerType.Standard, IsDeleted = false });

        var productRepo = new Mock<IProductRepository>();
        productRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = CreateService(customerRepo: customerRepo, productRepo: productRepo);
        var items = new List<OrderItemRequest> { new() { ProductId = 999, Quantity = 1 } };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateOrderAsync(1, items, "123 Main St", null));
    }

    [Fact]
    public async Task CreateOrderAsync_ThrowsWhenInventoryInsufficient()
    {
        var customerRepo = new Mock<ICustomerRepository>();
        customerRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer { Id = 1, Type = CustomerType.Standard, IsDeleted = false });

        var productRepo = new Mock<IProductRepository>();
        productRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product { Id = 1, Name = "Laptop", Price = 1000m, Category = ProductCategory.Electronics });

        var inventoryService = new Mock<IInventoryService>();
        inventoryService
            .Setup(i => i.IsAvailableAsync(1, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CreateService(
            customerRepo: customerRepo,
            productRepo: productRepo,
            inventoryService: inventoryService);

        var items = new List<OrderItemRequest> { new() { ProductId = 1, Quantity = 10000 } };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateOrderAsync(1, items, "123 Main St", null));
    }

    [Fact]
    public async Task CreateOrderAsync_ThrowsWhenCustomerIsDeleted()
    {
        var customerRepo = new Mock<ICustomerRepository>();
        customerRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer { Id = 1, Type = CustomerType.Standard, IsDeleted = true });

        var service = CreateService(customerRepo: customerRepo);
        var items = new List<OrderItemRequest> { new() { ProductId = 1, Quantity = 1 } };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateOrderAsync(1, items, "123 Main St", null));
    }

    // Test: validation rejects orders with negative quantity
    [Fact]
    public async Task CreateOrderAsync_ThrowsWhenQuantityIsNegative()
    {
        var customerRepo = new Mock<ICustomerRepository>();
        customerRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer { Id = 1, Type = CustomerType.Standard, IsDeleted = false });

        var productRepo = new Mock<IProductRepository>();
        productRepo
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product { Id = 1, Name = "Laptop", Price = 1000m, Category = ProductCategory.Electronics });

        var inventoryService = new Mock<IInventoryService>();
        inventoryService
            .Setup(i => i.IsAvailableAsync(1, 0, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CreateService(
            customerRepo: customerRepo,
            productRepo: productRepo,
            inventoryService: inventoryService);

        var items = new List<OrderItemRequest> { new() { ProductId = 1, Quantity = 0 } };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateOrderAsync(1, items, "123 Main St", null));
    }

    // Test: validation rejects orders for deleted customer
    [Fact]
    public async Task CreateOrderAsync_ThrowsWhenCustomerAccountIsDeleted()
    {
        var customerRepo = new Mock<ICustomerRepository>();
        customerRepo
            .Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer { Id = 5, IsDeleted = true, Type = CustomerType.Gold });

        var service = CreateService(customerRepo: customerRepo);
        var items = new List<OrderItemRequest> { new() { ProductId = 1, Quantity = 2 } };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateOrderAsync(5, items, "123 Main St", null));
    }

    // Test: validation rejects orders with non-existent customer id
    [Fact]
    public async Task CreateOrderAsync_ThrowsWhenCustomerDoesNotExistInDatabase()
    {
        var customerRepo = new Mock<ICustomerRepository>();
        customerRepo
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var service = CreateService(customerRepo: customerRepo);
        var items = new List<OrderItemRequest> { new() { ProductId = 1, Quantity = 1 } };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateOrderAsync(999, items, "123 Main St", null));
    }
}