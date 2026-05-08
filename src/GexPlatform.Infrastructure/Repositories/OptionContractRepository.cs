using GexPlatform.Application.Interfaces;
using GexPlatform.Domain.Entities;
using GexPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GexPlatform.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for OptionContract entities.
/// </summary>
public class OptionContractRepository : Repository<OptionContract>, IOptionContractRepository
{
    private readonly GexPlatformDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionContractRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public OptionContractRepository(GexPlatformDbContext dbContext)
        : base(dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OptionContract>> GetByOptionChainIdAsync(int optionChainId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.OptionContracts
            .AsNoTracking()
            .Where(c => c.OptionChainId == optionChainId)
            .OrderBy(c => c.StrikePrice)
            .ThenBy(c => c.OptionType)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OptionContract>> GetByChainStrikeAndTypeAsync(int optionChainId, decimal strikePrice, string optionType, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(optionType);

        return await _dbContext.OptionContracts
            .AsNoTracking()
            .Where(c => c.OptionChainId == optionChainId
                     && c.StrikePrice == strikePrice
                     && c.OptionType == optionType)
            .ToListAsync(cancellationToken);
    }
}
