using GexPlatform.Application.Dtos;

namespace GexPlatform.Application.Interfaces;

/// <summary>
/// Interface for fetching options chain data from external data providers.
/// </summary>
public interface IOptionsDataProvider
{
    /// <summary>
    /// Fetches options chain data for a given ticker and expiration date.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol (e.g., "AAPL").</param>
    /// <param name="expirationDate">The expiration date of the options chain.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>Options chain data including all contracts for the expiration date.</returns>
    /// <exception cref="HttpRequestException">Thrown when API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when data parsing fails.</exception>
    Task<OptionChainDto> GetOptionsChainAsync(
        string ticker,
        DateTime expirationDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches available expiration dates for a given ticker.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>List of available expiration dates for the ticker.</returns>
    /// <exception cref="HttpRequestException">Thrown when API request fails.</exception>
    Task<IEnumerable<DateTime>> GetExpirationDatesAsync(
        string ticker,
        CancellationToken cancellationToken = default);
}
