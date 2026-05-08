using GexPlatform.Application.Interfaces;
using GexPlatform.Domain.Entities;
using GexPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GexPlatform.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for OptionChain entities.
/// </summary>
public class OptionChainRepository : Repository<OptionChain>, IOptionChainRepository
{
    private readonly GexPlatformDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionChainRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public OptionChainRepository(GexPlatformDbContext dbContext)
        : base(dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <inheritdoc />
    public async Task<OptionChain?> GetByTickerAndExpirationAsync(string ticker, DateTime expirationDate, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticker);

        return await _dbContext.OptionChains
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.Ticker == ticker && c.ExpirationDate == expirationDate,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OptionChain>> GetByTickerAsync(string ticker, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticker);

        return await _dbContext.OptionChains
            .AsNoTracking()
            .Where(c => c.Ticker == ticker)
            .OrderByDescending(c => c.ExpirationDate)
            .ToListAsync(cancellationToken);
    }
}
