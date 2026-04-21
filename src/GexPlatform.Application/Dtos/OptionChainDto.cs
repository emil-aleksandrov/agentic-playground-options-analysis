namespace GexPlatform.Application.Dtos;

/// <summary>
/// Data transfer object for options chain data.
/// </summary>
public class OptionChainDto
{
    /// <summary>
    /// Ticker symbol of the underlying security.
    /// </summary>
    public required string Ticker { get; set; }

    /// <summary>
    /// Expiration date of the options.
    /// </summary>
    public required DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Current price of the underlying security.
    /// </summary>
    public decimal UnderlyingPrice { get; set; }

    /// <summary>
    /// Collection of option contracts.
    /// </summary>
    public List<OptionContractDto> Contracts { get; set; } = new();
}
