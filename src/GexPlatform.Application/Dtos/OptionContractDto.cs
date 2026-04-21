namespace GexPlatform.Application.Dtos;

/// <summary>
/// Data transfer object for a single options contract.
/// </summary>
public class OptionContractDto
{
    /// <summary>
    /// Strike price of the option.
    /// </summary>
    public decimal StrikePrice { get; set; }

    /// <summary>
    /// Type of option: "Call" or "Put".
    /// </summary>
    public required string OptionType { get; set; }

    /// <summary>
    /// Bid price (offer to buy).
    /// </summary>
    public decimal BidPrice { get; set; }

    /// <summary>
    /// Ask price (offer to sell).
    /// </summary>
    public decimal AskPrice { get; set; }

    /// <summary>
    /// Last traded price.
    /// </summary>
    public decimal? LastPrice { get; set; }

    /// <summary>
    /// Open interest (number of open contracts).
    /// </summary>
    public long OpenInterest { get; set; }

    /// <summary>
    /// Trading volume for the contract.
    /// </summary>
    public long Volume { get; set; }

    /// <summary>
    /// Implied volatility as a decimal (e.g., 0.25 for 25%).
    /// </summary>
    public decimal ImpliedVolatility { get; set; }

    /// <summary>
    /// Delta - sensitivity to underlying price changes.
    /// </summary>
    public decimal? Delta { get; set; }

    /// <summary>
    /// Gamma - rate of change of delta.
    /// </summary>
    public decimal? Gamma { get; set; }

    /// <summary>
    /// Theta - time decay effect.
    /// </summary>
    public decimal? Theta { get; set; }

    /// <summary>
    /// Vega - sensitivity to volatility changes.
    /// </summary>
    public decimal? Vega { get; set; }

    /// <summary>
    /// Rho - sensitivity to interest rate changes.
    /// </summary>
    public decimal? Rho { get; set; }
}
