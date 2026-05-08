using GexPlatform.Domain.Entities;
using GexPlatform.Infrastructure.Data;
using GexPlatform.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

namespace GexPlatform.Tests;

/// <summary>
/// Unit tests for the generic and specific repository implementations.
/// </summary>
public class RepositoryTests
{
    private GexPlatformDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GexPlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GexPlatformDbContext(options);
    }

    [Fact]
    public async Task GenericRepository_GetAllAsync_ReturnsAllEntities()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new Repository<OptionChain>(context);

        var chains = new[]
        {
            new OptionChain { Ticker = "AAPL", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 150m },
            new OptionChain { Ticker = "MSFT", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 380m }
        };

        context.OptionChains.AddRange(chains);
        await context.SaveChangesAsync();

        // Act
        var result = await repo.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GenericRepository_GetByIdAsync_ReturnsCorrectEntity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new Repository<OptionChain>(context);
        var chain = new OptionChain { Ticker = "TSLA", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 200m };

        context.OptionChains.Add(chain);
        await context.SaveChangesAsync();

        // Act
        var result = await repo.GetByIdAsync(chain.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TSLA", result.Ticker);
    }

    [Fact]
    public async Task GenericRepository_GetByIdAsync_ReturnsNullWhenNotFound()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new Repository<OptionChain>(context);

        // Act
        var result = await repo.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GenericRepository_FindAsync_ReturnMatchingEntities()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new Repository<OptionChain>(context);

        var chains = new[]
        {
            new OptionChain { Ticker = "AAPL", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 150m },
            new OptionChain { Ticker = "AAPL", ExpirationDate = new DateTime(2026, 6, 19), UnderlyingPrice = 155m },
            new OptionChain { Ticker = "MSFT", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 380m }
        };

        context.OptionChains.AddRange(chains);
        await context.SaveChangesAsync();

        // Act
        var result = await repo.FindAsync(c => c.Ticker == "AAPL");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, c => Assert.Equal("AAPL", c.Ticker));
    }

    [Fact]
    public async Task GenericRepository_AddAsync_InsertsEntity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new Repository<OptionChain>(context);
        var chain = new OptionChain { Ticker = "GOOGL", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 2800m };

        // Act
        await repo.AddAsync(chain);

        // Assert
        var result = await context.OptionChains.FirstOrDefaultAsync(c => c.Ticker == "GOOGL");
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GenericRepository_AddRangeAsync_InsertsMultipleEntities()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new Repository<OptionChain>(context);
        var chains = new[]
        {
            new OptionChain { Ticker = "AMD", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 150m },
            new OptionChain { Ticker = "NVDA", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 1000m }
        };

        // Act
        await repo.AddRangeAsync(chains);

        // Assert
        var count = await context.OptionChains.CountAsync();
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GenericRepository_UpdateAsync_ModifiesEntity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new Repository<OptionChain>(context);
        var chain = new OptionChain { Ticker = "META", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 350m };

        context.OptionChains.Add(chain);
        await context.SaveChangesAsync();

        // Act
        chain.UnderlyingPrice = 360m;
        await repo.UpdateAsync(chain);

        // Assert
        var result = await context.OptionChains.FirstOrDefaultAsync(c => c.Id == chain.Id);
        Assert.NotNull(result);
        Assert.Equal(360m, result.UnderlyingPrice);
    }

    [Fact]
    public async Task GenericRepository_DeleteAsync_RemovesEntityById()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new Repository<OptionChain>(context);
        var chain = new OptionChain { Ticker = "IBM", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 180m };

        context.OptionChains.Add(chain);
        await context.SaveChangesAsync();

        // Act
        var result = await repo.DeleteAsync(chain.Id);

        // Assert
        Assert.True(result);
        var remaining = await context.OptionChains.FirstOrDefaultAsync(c => c.Id == chain.Id);
        Assert.Null(remaining);
    }

    [Fact]
    public async Task GenericRepository_DeleteAsync_RemovesEntity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new Repository<OptionChain>(context);
        var chain = new OptionChain { Ticker = "INTC", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 45m };

        context.OptionChains.Add(chain);
        await context.SaveChangesAsync();

        // Act
        await repo.DeleteAsync(chain);

        // Assert
        var count = await context.OptionChains.CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task GenericRepository_CountAsync_WithoutPredicate_ReturnsTotal()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new Repository<OptionChain>(context);
        var chains = new[]
        {
            new OptionChain { Ticker = "A", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 100m },
            new OptionChain { Ticker = "B", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 200m },
            new OptionChain { Ticker = "C", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 300m }
        };

        context.OptionChains.AddRange(chains);
        await context.SaveChangesAsync();

        // Act
        var count = await repo.CountAsync();

        // Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task GenericRepository_CountAsync_WithPredicate_ReturnsMatchingCount()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new Repository<OptionChain>(context);
        var chains = new[]
        {
            new OptionChain { Ticker = "AAPL", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 150m },
            new OptionChain { Ticker = "AAPL", ExpirationDate = new DateTime(2026, 6, 19), UnderlyingPrice = 155m },
            new OptionChain { Ticker = "MSFT", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 380m }
        };

        context.OptionChains.AddRange(chains);
        await context.SaveChangesAsync();

        // Act
        var count = await repo.CountAsync(c => c.Ticker == "AAPL");

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GenericRepository_AnyAsync_ReturnsTrueWhenMatch()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new Repository<OptionChain>(context);
        var chain = new OptionChain { Ticker = "AAPL", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 150m };

        context.OptionChains.Add(chain);
        await context.SaveChangesAsync();

        // Act
        var result = await repo.AnyAsync(c => c.Ticker == "AAPL");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GenericRepository_AnyAsync_ReturnsFalseWhenNoMatch()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new Repository<OptionChain>(context);

        // Act
        var result = await repo.AnyAsync(c => c.Ticker == "NONEXISTENT");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task OptionChainRepository_GetByTickerAndExpirationAsync_ReturnsCorrectChain()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new OptionChainRepository(context);
        var expiration = new DateTime(2026, 5, 15);
        var chain = new OptionChain { Ticker = "AAPL", ExpirationDate = expiration, UnderlyingPrice = 150m };

        context.OptionChains.Add(chain);
        await context.SaveChangesAsync();

        // Act
        var result = await repo.GetByTickerAndExpirationAsync("AAPL", expiration);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("AAPL", result.Ticker);
        Assert.Equal(expiration, result.ExpirationDate);
    }

    [Fact]
    public async Task OptionChainRepository_GetByTickerAsync_ReturnsAllChainsForTicker()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new OptionChainRepository(context);
        var chains = new[]
        {
            new OptionChain { Ticker = "MSFT", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 380m },
            new OptionChain { Ticker = "MSFT", ExpirationDate = new DateTime(2026, 6, 19), UnderlyingPrice = 385m },
            new OptionChain { Ticker = "AAPL", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 150m }
        };

        context.OptionChains.AddRange(chains);
        await context.SaveChangesAsync();

        // Act
        var result = await repo.GetByTickerAsync("MSFT");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, c => Assert.Equal("MSFT", c.Ticker));
    }

    [Fact]
    public async Task OptionContractRepository_GetByOptionChainIdAsync_ReturnsAllContractsForChain()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new OptionContractRepository(context);
        var chain = new OptionChain { Ticker = "SPY", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 450m };
        context.OptionChains.Add(chain);
        await context.SaveChangesAsync();

        var contracts = new[]
        {
            new OptionContract { OptionChainId = chain.Id, StrikePrice = 445m, OptionType = "Call", BidPrice = 5m, AskPrice = 6m, OpenInterest = 100, Volume = 50, ImpliedVolatility = 0.25m },
            new OptionContract { OptionChainId = chain.Id, StrikePrice = 450m, OptionType = "Call", BidPrice = 4m, AskPrice = 5m, OpenInterest = 150, Volume = 75, ImpliedVolatility = 0.24m }
        };
        context.OptionContracts.AddRange(contracts);
        await context.SaveChangesAsync();

        // Act
        var result = await repo.GetByOptionChainIdAsync(chain.Id);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, c => Assert.Equal(chain.Id, c.OptionChainId));
    }

    [Fact]
    public async Task OptionContractRepository_GetByChainStrikeAndTypeAsync_ReturnsMatchingContracts()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new OptionContractRepository(context);
        var chain = new OptionChain { Ticker = "QQQ", ExpirationDate = new DateTime(2026, 5, 15), UnderlyingPrice = 450m };
        context.OptionChains.Add(chain);
        await context.SaveChangesAsync();

        var contracts = new[]
        {
            new OptionContract { OptionChainId = chain.Id, StrikePrice = 445m, OptionType = "Call", BidPrice = 5m, AskPrice = 6m, OpenInterest = 100, Volume = 50, ImpliedVolatility = 0.25m },
            new OptionContract { OptionChainId = chain.Id, StrikePrice = 445m, OptionType = "Put", BidPrice = 3m, AskPrice = 4m, OpenInterest = 80, Volume = 40, ImpliedVolatility = 0.26m },
            new OptionContract { OptionChainId = chain.Id, StrikePrice = 450m, OptionType = "Call", BidPrice = 4m, AskPrice = 5m, OpenInterest = 150, Volume = 75, ImpliedVolatility = 0.24m }
        };
        context.OptionContracts.AddRange(contracts);
        await context.SaveChangesAsync();

        // Act
        var result = await repo.GetByChainStrikeAndTypeAsync(chain.Id, 445m, "Call");

        // Assert
        Assert.Single(result);
        Assert.Equal(445m, result.First().StrikePrice);
        Assert.Equal("Call", result.First().OptionType);
    }
}
