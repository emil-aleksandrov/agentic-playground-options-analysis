using GexPlatform.Application.Dtos;
using GexPlatform.Application.Interfaces;
using GexPlatform.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GexPlatform.Infrastructure.Services;

/// <summary>
/// Implementation of IOptionsDataProvider using Yahoo Finance data source.
/// </summary>
public class YahooFinanceClient : IOptionsDataProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<YahooFinanceClient> _logger;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
    private readonly IAsyncPolicy<HttpResponseMessage> _circuitBreakerPolicy;

    /// <summary>
    /// Base URL for Yahoo Finance API endpoints.
    /// </summary>
    private const string YahooFinanceBaseUrl = "https://query1.finance.yahoo.com/v7";

    public YahooFinanceClient(HttpClient httpClient, ILogger<YahooFinanceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        // Initialize Polly retry policy for transient errors
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .OrResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500)
            .WaitAndRetryAsync<HttpResponseMessage>(
                retryCount: 3,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "Retry {RetryCount} after {DelaySeconds}s for Yahoo Finance API",
                        retryCount, timespan.TotalSeconds);
                });

        // Initialize Polly circuit breaker policy
        _circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync<HttpResponseMessage>(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, timespan) =>
                {
                    _logger.LogError(
                        "Circuit breaker opened for {DurationSeconds}s due to repeated failures",
                        timespan.TotalSeconds);
                },
                onReset: () =>
                {
                    _logger.LogInformation("Circuit breaker reset");
                });
    }

    /// <summary>
    /// Fetches options chain data for a given ticker and expiration date.
    /// </summary>
    public async Task<OptionChainDto> GetOptionsChainAsync(
        string ticker,
        DateTime expirationDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching options chain for ticker {Ticker} expiration {ExpirationDate}", ticker, expirationDate);

            var timestamp = (long)(expirationDate.Date - new DateTime(1970, 1, 1)).TotalSeconds;
            var url = $"{YahooFinanceBaseUrl}/finance/options/{ticker}?date={timestamp}";

            using var response = await ExecuteWithPoliciesAsync(url, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var responseData = await JsonSerializer.DeserializeAsync<YahooFinanceResponse>(responseStream, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (responseData?.OptionChains?.Count == 0)
            {
                _logger.LogWarning("No options chain data found for ticker {Ticker}", ticker);
                throw new OptionsDataException($"No options chain data found for ticker {ticker}");
            }

            var optionChain = responseData!.OptionChains![0];
            var quote = optionChain.Quote;

            var result = new OptionChainDto
            {
                Ticker = ticker,
                ExpirationDate = expirationDate,
                UnderlyingPrice = quote?.RegularMarketPrice ?? 0m,
                Contracts = MapContractsFromResponse(optionChain)
            };

            _logger.LogInformation("Successfully fetched {ContractCount} contracts for {Ticker}", result.Contracts.Count, ticker);

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while fetching options chain for ticker {Ticker}", ticker);
            throw new OptionsDataException($"Failed to fetch options data from Yahoo Finance for ticker {ticker}", ex);
        }
        catch (OptionsDataException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching options chain for ticker {Ticker}", ticker);
            throw new OptionsDataException($"Unexpected error fetching options data for ticker {ticker}", ex);
        }
    }

    /// <summary>
    /// Fetches available expiration dates for a given ticker.
    /// </summary>
    public async Task<IEnumerable<DateTime>> GetExpirationDatesAsync(
        string ticker,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching expiration dates for ticker {Ticker}", ticker);

            var url = $"{YahooFinanceBaseUrl}/finance/options/{ticker}";

            using var response = await ExecuteWithPoliciesAsync(url, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var responseData = await JsonSerializer.DeserializeAsync<YahooFinanceResponse>(responseStream, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (responseData?.OptionChains?.Count == 0)
            {
                _logger.LogWarning("No expiration dates found for ticker {Ticker}", ticker);
                return Enumerable.Empty<DateTime>();
            }

            var expirationDates = responseData!.OptionChains!
                .Where(oc => oc.ExpirationDate.HasValue)
                .Select(oc => UnixTimeStampToDateTime(oc.ExpirationDate!.Value))
                .OrderBy(d => d)
                .ToList();

            _logger.LogInformation("Found {ExpirationDateCount} expiration dates for {Ticker}", expirationDates.Count, ticker);

            return expirationDates;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while fetching expiration dates for ticker {Ticker}", ticker);
            throw new OptionsDataException($"Failed to fetch expiration dates from Yahoo Finance for ticker {ticker}", ex);
        }
        catch (OptionsDataException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching expiration dates for ticker {Ticker}", ticker);
            throw new OptionsDataException($"Unexpected error fetching expiration dates for ticker {ticker}", ex);
        }
    }

    /// <summary>
    /// Maps Yahoo Finance API response contracts to OptionContractDto objects.
    /// </summary>
    private static List<OptionContractDto> MapContractsFromResponse(YahooOptionChain optionChain)
    {
        var contracts = new List<OptionContractDto>();

        // Map call options
        if (optionChain.Options?.Any() == true)
        {
            foreach (var option in optionChain.Options)
            {
                contracts.Add(new OptionContractDto
                {
                    StrikePrice = option.Strike ?? 0m,
                    OptionType = "Call",
                    BidPrice = option.Bid ?? 0m,
                    AskPrice = option.Ask ?? 0m,
                    LastPrice = option.LastPrice,
                    OpenInterest = option.OpenInterest ?? 0,
                    Volume = option.Volume ?? 0,
                    ImpliedVolatility = option.ImpliedVolatility ?? 0m,
                    Delta = option.Delta,
                    Gamma = option.Gamma,
                    Theta = option.Theta,
                    Vega = option.Vega,
                    Rho = option.Rho
                });

                // Also add put option if available
                if (option.ContractSymbol?.Contains("P") == true)
                {
                    contracts.Add(new OptionContractDto
                    {
                        StrikePrice = option.Strike ?? 0m,
                        OptionType = "Put",
                        BidPrice = option.Bid ?? 0m,
                        AskPrice = option.Ask ?? 0m,
                        LastPrice = option.LastPrice,
                        OpenInterest = option.OpenInterest ?? 0,
                        Volume = option.Volume ?? 0,
                        ImpliedVolatility = option.ImpliedVolatility ?? 0m,
                        Delta = option.Delta,
                        Gamma = option.Gamma,
                        Theta = option.Theta,
                        Vega = option.Vega,
                        Rho = option.Rho
                    });
                }
            }
        }

        return contracts;
    }

    /// <summary>
    /// Converts Unix timestamp to DateTime.
    /// </summary>
    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }

    /// <summary>
    /// Executes an HTTP GET request with Polly retry and circuit breaker policies.
    /// </summary>
    private async Task<HttpResponseMessage> ExecuteWithPoliciesAsync(
        string url,
        CancellationToken cancellationToken)
    {
        // Combine retry and circuit breaker policies
        var combinedPolicy = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);

        return await combinedPolicy.ExecuteAsync(
            ct => _httpClient.GetAsync(url, ct),
            cancellationToken).ConfigureAwait(false);
    }

    #region Yahoo Finance Response Models

    private class YahooFinanceResponse
    {
        [JsonPropertyName("optionChains")]
        public List<YahooOptionChain>? OptionChains { get; set; }
    }

    private class YahooOptionChain
    {
        [JsonPropertyName("expirationDate")]
        public long? ExpirationDate { get; set; }

        [JsonPropertyName("quote")]
        public YahooQuote? Quote { get; set; }

        [JsonPropertyName("options")]
        public List<YahooOption>? Options { get; set; }
    }

    private class YahooQuote
    {
        [JsonPropertyName("regularMarketPrice")]
        public decimal? RegularMarketPrice { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
    }

    private class YahooOption
    {
        [JsonPropertyName("contractSymbol")]
        public string? ContractSymbol { get; set; }

        [JsonPropertyName("strike")]
        public decimal? Strike { get; set; }

        [JsonPropertyName("bid")]
        public decimal? Bid { get; set; }

        [JsonPropertyName("ask")]
        public decimal? Ask { get; set; }

        [JsonPropertyName("lastPrice")]
        public decimal? LastPrice { get; set; }

        [JsonPropertyName("openInterest")]
        public long? OpenInterest { get; set; }

        [JsonPropertyName("volume")]
        public long? Volume { get; set; }

        [JsonPropertyName("impliedVolatility")]
        public decimal? ImpliedVolatility { get; set; }

        [JsonPropertyName("delta")]
        public decimal? Delta { get; set; }

        [JsonPropertyName("gamma")]
        public decimal? Gamma { get; set; }

        [JsonPropertyName("theta")]
        public decimal? Theta { get; set; }

        [JsonPropertyName("vega")]
        public decimal? Vega { get; set; }

        [JsonPropertyName("rho")]
        public decimal? Rho { get; set; }
    }

    #endregion
}
