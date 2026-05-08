using GexPlatform.Infrastructure.Data;
using GexPlatform.Infrastructure.Repositories;
using GexPlatform.Infrastructure.Services;
using GexPlatform.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GexPlatform.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring infrastructure services.
/// </summary>
public static class InfrastructureExtensions
{
    /// <summary>
    /// Adds infrastructure services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // Configure DbContext with SQLite
        services.AddDbContext<GexPlatformDbContext>(options =>
            options.UseSqlite(connectionString)
        );

        // Register repositories
        services.AddScoped<IOptionChainRepository, OptionChainRepository>();
        services.AddScoped<IOptionContractRepository, OptionContractRepository>();

        // Configure HttpClient for external API calls
        // Polly retry policies are implemented for resilience
        services.AddHttpClient<IOptionsDataProvider, YahooFinanceClient>()
            .ConfigureHttpClient(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("User-Agent", "GexPlatform/1.0");
            });

        return services;
    }
}
