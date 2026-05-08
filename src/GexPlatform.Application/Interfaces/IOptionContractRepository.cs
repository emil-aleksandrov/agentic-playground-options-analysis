using GexPlatform.Domain.Entities;

namespace GexPlatform.Application.Interfaces;

/// <summary>
/// Repository interface for OptionContract entities.
/// </summary>
public interface IOptionContractRepository : IRepository<OptionContract>
{
    /// <summary>
    /// Gets all contracts for a specific options chain.
    /// </summary>
    /// <param name="optionChainId">The options chain ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation that returns the contracts.</returns>
    Task<IEnumerable<OptionContract>> GetByOptionChainIdAsync(int optionChainId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets contracts for a specific strike and type within an options chain.
    /// </summary>
    /// <param name="optionChainId">The options chain ID.</param>
    /// <param name="strikePrice">The strike price.</param>
    /// <param name="optionType">The option type (Call or Put).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation that returns the matching contracts.</returns>
    Task<IEnumerable<OptionContract>> GetByChainStrikeAndTypeAsync(int optionChainId, decimal strikePrice, string optionType, CancellationToken cancellationToken = default);
}
