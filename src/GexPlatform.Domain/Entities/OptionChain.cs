namespace GexPlatform.Domain.Entities;

/// <summary>
/// Represents an options chain for a specific underlying security and expiration date.
/// </summary>
public class OptionChain : BaseEntity
{
    /// <summary>
    /// Ticker symbol of the underlying security.
    /// </summary>
    public required string Ticker { get; set; }

    /// <summary>
    /// Expiration date of the options in this chain.
    /// </summary>
    public required DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Current price of the underlying security.
    /// </summary>
    public decimal UnderlyingPrice { get; set; }

    /// <summary>
    /// Collection of option contracts in this chain.
    /// </summary>
    public ICollection<OptionContract> Contracts { get; set; } = new List<OptionContract>();
}
