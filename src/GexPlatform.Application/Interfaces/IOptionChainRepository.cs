using GexPlatform.Domain.Entities;

namespace GexPlatform.Application.Interfaces;

/// <summary>
/// Repository interface for OptionChain entities.
/// </summary>
public interface IOptionChainRepository : IRepository<OptionChain>
{
    /// <summary>
    /// Gets an options chain by ticker and expiration date.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <param name="expirationDate">The expiration date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation that returns the options chain or null if not found.</returns>
    Task<OptionChain?> GetByTickerAndExpirationAsync(string ticker, DateTime expirationDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all options chains for a specific ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation that returns the options chains.</returns>
    Task<IEnumerable<OptionChain>> GetByTickerAsync(string ticker, CancellationToken cancellationToken = default);
}
