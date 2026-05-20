using Microsoft.EntityFrameworkCore;
using QuotesApi.Abstractions;
using QuotesApi.Data;
using QuotesApi.Infrastructure;
using QuotesApi.Repositories;
namespace QuotesApi.Extensions;
public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite("Data Source=quotes.db");
        });
        // Scoped — one instance per request (already existed)
        services.AddScoped<IQuoteRepository, QuoteRepository>();

        // Singleton — one instance for app lifetime
        services.AddSingleton<IClock, SystemClock>();

        // Transient — new instance every time
        services.AddTransient<IRequestLogger, RequestLogger>();

        return services;
    }
}