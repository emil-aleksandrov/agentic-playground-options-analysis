using GexPlatform.Domain.Entities;
using GexPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GexPlatform.Tests;

/// <summary>
/// Integration tests for database migrations and schema verification.
/// </summary>
public class MigrationIntegrationTests : IDisposable
{
    private readonly string _tempDatabasePath;

    public MigrationIntegrationTests()
    {
        _tempDatabasePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
    }

    public void Dispose()
    {
        try
        {
            // Give SQLite time to release locks
            System.Threading.Thread.Sleep(100);
            
            if (File.Exists(_tempDatabasePath))
            {
                File.Delete(_tempDatabasePath);
            }

            // Also delete WAL and SHM files if they exist
            var walFile = _tempDatabasePath + "-wal";
            var shmFile = _tempDatabasePath + "-shm";
            
            if (File.Exists(walFile))
                File.Delete(walFile);
            if (File.Exists(shmFile))
                File.Delete(shmFile);
        }
        catch
        {
            // Silently ignore deletion failures - they'll be cleaned up by the OS
        }
    }

    private GexPlatformDbContext CreateSqliteContext()
    {
        var connectionString = $"Data Source={_tempDatabasePath};";
        var options = new DbContextOptionsBuilder<GexPlatformDbContext>()
            .UseSqlite(connectionString)
            .Options;

        return new GexPlatformDbContext(options);
    }

    [Fact]
    public async Task Migration_CreatesOptionChainsTable()
    {
        // Arrange
        using var context = CreateSqliteContext();

        // Act - Apply the migration
        await context.Database.MigrateAsync();

        // Assert - Table should exist and have the correct columns
        var tables = await context.Database.SqlQuery<string>(
            $@"SELECT name FROM sqlite_master WHERE type='table' AND name='OptionChains'"
        ).ToListAsync();

        Assert.Single(tables);
    }

    [Fact]
    public async Task Migration_CreatesOptionContractsTable()
    {
        // Arrange
        using var context = CreateSqliteContext();

        // Act
        await context.Database.MigrateAsync();

        // Assert
        var tables = await context.Database.SqlQuery<string>(
            $@"SELECT name FROM sqlite_master WHERE type='table' AND name='OptionContracts'"
        ).ToListAsync();

        Assert.Single(tables);
    }

    [Fact]
    public async Task Migration_CreatesMigrationHistoryTable()
    {
        // Arrange
        using var context = CreateSqliteContext();

        // Act
        await context.Database.MigrateAsync();

        // Assert
        var tables = await context.Database.SqlQuery<string>(
            $@"SELECT name FROM sqlite_master WHERE type='table' AND name='__EFMigrationsHistory'"
        ).ToListAsync();

        Assert.Single(tables);
    }

    [Fact]
    public async Task Migration_AllowsInsertIntoOptionChains()
    {
        // Arrange
        using var context = CreateSqliteContext();
        await context.Database.MigrateAsync();

        var chain = new OptionChain
        {
            Ticker = "AAPL",
            ExpirationDate = new DateTime(2026, 5, 15),
            UnderlyingPrice = 150.00m
        };

        // Act
        context.OptionChains.Add(chain);
        await context.SaveChangesAsync();

        // Assert
        var savedChain = await context.OptionChains.FirstOrDefaultAsync(c => c.Ticker == "AAPL");
        Assert.NotNull(savedChain);
        Assert.Equal("AAPL", savedChain.Ticker);
        Assert.True(savedChain.Id > 0);
    }

    [Fact]
    public async Task Migration_AllowsInsertIntoOptionContracts()
    {
        // Arrange
        using var context = CreateSqliteContext();
        await context.Database.MigrateAsync();

        var chain = new OptionChain
        {
            Ticker = "SPY",
            ExpirationDate = new DateTime(2026, 5, 15),
            UnderlyingPrice = 450.00m
        };

        var contract = new OptionContract
        {
            OptionChainId = 1,
            StrikePrice = 445.00m,
            OptionType = "Call",
            BidPrice = 5.00m,
            AskPrice = 6.00m,
            OpenInterest = 1000,
            Volume = 500,
            ImpliedVolatility = 0.25m
        };

        context.OptionChains.Add(chain);
        await context.SaveChangesAsync();

        contract.OptionChainId = chain.Id;
        context.OptionContracts.Add(contract);

        // Act
        await context.SaveChangesAsync();

        // Assert
        var savedContract = await context.OptionContracts.FirstOrDefaultAsync();
        Assert.NotNull(savedContract);
        Assert.Equal("Call", savedContract.OptionType);
        Assert.Equal(445.00m, savedContract.StrikePrice);
    }

    [Fact]
    public async Task Migration_EnforcesForeignKeyConstraint()
    {
        // Arrange
        using var context = CreateSqliteContext();
        await context.Database.MigrateAsync();

        var contract = new OptionContract
        {
            OptionChainId = 999, // Non-existent ID
            StrikePrice = 100.00m,
            OptionType = "Call",
            BidPrice = 5.00m,
            AskPrice = 6.00m,
            OpenInterest = 100,
            Volume = 50,
            ImpliedVolatility = 0.20m
        };

        context.OptionContracts.Add(contract);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            async () => await context.SaveChangesAsync()
        );
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task Migration_EnforcesUniqueConstraintOnTickerAndExpiration()
    {
        // Arrange
        using var context = CreateSqliteContext();
        await context.Database.MigrateAsync();

        var chain1 = new OptionChain
        {
            Ticker = "MSFT",
            ExpirationDate = new DateTime(2026, 5, 15),
            UnderlyingPrice = 380.00m
        };

        var chain2 = new OptionChain
        {
            Ticker = "MSFT",
            ExpirationDate = new DateTime(2026, 5, 15),
            UnderlyingPrice = 385.00m
        };

        context.OptionChains.Add(chain1);
        await context.SaveChangesAsync();
        context.OptionChains.Add(chain2);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            async () => await context.SaveChangesAsync()
        );
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task Migration_DefaultValuesForCreatedAtAndUpdatedAt()
    {
        // Arrange
        using var context = CreateSqliteContext();
        await context.Database.MigrateAsync();

        var beforeCreation = DateTime.UtcNow;
        var chain = new OptionChain
        {
            Ticker = "TSLA",
            ExpirationDate = new DateTime(2026, 5, 15),
            UnderlyingPrice = 200.00m
        };

        context.OptionChains.Add(chain);
        await context.SaveChangesAsync();
        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Act
        var savedChain = await context.OptionChains.FirstOrDefaultAsync(c => c.Ticker == "TSLA");

        // Assert - CreatedAt and UpdatedAt should be populated
        Assert.NotNull(savedChain);
        Assert.True(savedChain.CreatedAt >= beforeCreation && savedChain.CreatedAt <= afterCreation);
        Assert.True(savedChain.UpdatedAt >= beforeCreation && savedChain.UpdatedAt <= afterCreation);
    }

    [Fact]
    public async Task Migration_CascadeDeleteWorks()
    {
        // Arrange
        using var context = CreateSqliteContext();
        await context.Database.MigrateAsync();

        var chain = new OptionChain
        {
            Ticker = "GOOGL",
            ExpirationDate = new DateTime(2026, 5, 15),
            UnderlyingPrice = 2800.00m
        };

        var contract = new OptionContract
        {
            StrikePrice = 2800.00m,
            OptionType = "Call",
            BidPrice = 50.00m,
            AskPrice = 52.00m,
            OpenInterest = 500,
            Volume = 250,
            ImpliedVolatility = 0.28m
        };

        context.OptionChains.Add(chain);
        await context.SaveChangesAsync();

        contract.OptionChainId = chain.Id;
        context.OptionContracts.Add(contract);
        await context.SaveChangesAsync();

        // Act - Delete the chain
        context.OptionChains.Remove(chain);
        await context.SaveChangesAsync();

        // Assert - Related contracts should also be deleted
        var remainingContracts = await context.OptionContracts.CountAsync();
        Assert.Equal(0, remainingContracts);
    }

    [Fact]
    public async Task Migration_IndexesExistOnOptionChains()
    {
        // Arrange
        using var context = CreateSqliteContext();
        await context.Database.MigrateAsync();

        // Act
        var indexes = await context.Database.SqlQuery<string>(
            $@"SELECT name FROM sqlite_master WHERE type='index' AND tbl_name='OptionChains' AND name LIKE 'IX_%'"
        ).ToListAsync();

        // Assert - Should have at least one index (the unique one on Ticker, ExpirationDate)
        Assert.NotEmpty(indexes);
        Assert.Contains(indexes, i => i.Contains("Ticker_ExpirationDate"));
    }

    [Fact]
    public async Task Migration_IndexesExistOnOptionContracts()
    {
        // Arrange
        using var context = CreateSqliteContext();
        await context.Database.MigrateAsync();

        // Act
        var indexes = await context.Database.SqlQuery<string>(
            $@"SELECT name FROM sqlite_master WHERE type='index' AND tbl_name='OptionContracts' AND name LIKE 'IX_%'"
        ).ToListAsync();

        // Assert - Should have at least one index
        Assert.NotEmpty(indexes);
    }

    [Fact]
    public async Task Migration_CanUpdateEntity()
    {
        // Arrange
        using var context = CreateSqliteContext();
        await context.Database.MigrateAsync();

        var chain = new OptionChain
        {
            Ticker = "NVDA",
            ExpirationDate = new DateTime(2026, 5, 15),
            UnderlyingPrice = 1000.00m
        };

        context.OptionChains.Add(chain);
        await context.SaveChangesAsync();

        var originalUpdatedAt = chain.UpdatedAt;
        await Task.Delay(100);

        // Act - Update the entity
        chain.UnderlyingPrice = 1050.00m;
        context.OptionChains.Update(chain);
        await context.SaveChangesAsync();

        // Assert
        var updatedChain = await context.OptionChains.FirstOrDefaultAsync(c => c.Id == chain.Id);
        Assert.NotNull(updatedChain);
        Assert.Equal(1050.00m, updatedChain.UnderlyingPrice);
        Assert.True(updatedChain.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public async Task Migration_AllowsMultipleChainsForSameTicker()
    {
        // Arrange
        using var context = CreateSqliteContext();
        await context.Database.MigrateAsync();

        var chain1 = new OptionChain
        {
            Ticker = "QQQ",
            ExpirationDate = new DateTime(2026, 5, 15),
            UnderlyingPrice = 450.00m
        };

        var chain2 = new OptionChain
        {
            Ticker = "QQQ",
            ExpirationDate = new DateTime(2026, 6, 19),
            UnderlyingPrice = 450.00m
        };

        // Act
        context.OptionChains.AddRange(chain1, chain2);
        await context.SaveChangesAsync();

        // Assert
        var chains = await context.OptionChains.Where(c => c.Ticker == "QQQ").ToListAsync();
        Assert.Equal(2, chains.Count);
    }
}
