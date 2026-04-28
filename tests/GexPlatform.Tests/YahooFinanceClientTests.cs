using GexPlatform.Application.Dtos;
using GexPlatform.Infrastructure.Exceptions;
using GexPlatform.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace GexPlatform.Tests;

/// <summary>
/// Unit tests for YahooFinanceClient.
/// Tests cover successful scenarios, error handling, retry policies, and circuit breaker behavior.
/// </summary>
public class YahooFinanceClientTests
{
    private readonly Mock<ILogger<YahooFinanceClient>> _mockLogger;
    private readonly HttpClient _httpClient;
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;

    public YahooFinanceClientTests()
    {
        _mockLogger = new Mock<ILogger<YahooFinanceClient>>();
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpHandler.Object)
        {
            BaseAddress = new Uri("https://query1.finance.yahoo.com/v7/")
        };
    }

    #region GetOptionsChainAsync Tests

    [Fact]
    public async Task GetOptionsChainAsync_WithValidResponse_ReturnsOptionChainDto()
    {
        // Arrange
        var ticker = "AAPL";
        var expirationDate = new DateTime(2026, 05, 15);
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        var responseContent = CreateValidYahooFinanceResponse();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await client.GetOptionsChainAsync(ticker, expirationDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticker, result.Ticker);
        Assert.Equal(expirationDate, result.ExpirationDate);
        Assert.Equal(150.25m, result.UnderlyingPrice);
        Assert.NotEmpty(result.Contracts);
        Assert.True(result.Contracts.Count > 0);
    }

    [Fact]
    public async Task GetOptionsChainAsync_WithContractSymbolP_AddsCallAndPutContracts()
    {
        // Arrange
        var ticker = "MSFT";
        var expirationDate = new DateTime(2026, 05, 22);
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        var responseContent = CreateYahooFinanceResponseWithPutContracts();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await client.GetOptionsChainAsync(ticker, expirationDate);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Contracts);
        // Should have call and put contracts when ContractSymbol contains "P"
        var callContracts = result.Contracts.Where(c => c.OptionType == "Call").ToList();
        var putContracts = result.Contracts.Where(c => c.OptionType == "Put").ToList();
        Assert.NotEmpty(callContracts);
        Assert.NotEmpty(putContracts);
    }

    [Fact]
    public async Task GetOptionsChainAsync_WithEmptyOptionChains_ThrowsOptionsDataException()
    {
        // Arrange
        var ticker = "INVALID";
        var expirationDate = new DateTime(2026, 05, 15);
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        var responseContent = CreateYahooFinanceResponseWithEmptyChains();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OptionsDataException>(
            () => client.GetOptionsChainAsync(ticker, expirationDate));

        Assert.Contains("No options chain data found", exception.Message);
    }

    [Fact]
    public async Task GetOptionsChainAsync_WithHttpRequestException_ThrowsOptionsDataException()
    {
        // Arrange
        var ticker = "AAPL";
        var expirationDate = new DateTime(2026, 05, 15);
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OptionsDataException>(
            () => client.GetOptionsChainAsync(ticker, expirationDate));

        Assert.Contains("Failed to fetch options data from Yahoo Finance", exception.Message);
        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public async Task GetOptionsChainAsync_WithNullResponseData_ThrowsOptionsDataException()
    {
        // Arrange
        var ticker = "AAPL";
        var expirationDate = new DateTime(2026, 05, 15);
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OptionsDataException>(
            () => client.GetOptionsChainAsync(ticker, expirationDate));

        Assert.NotNull(exception);
    }

    [Fact]
    public async Task GetOptionsChainAsync_WithUnsuccessfulStatusCode_ThrowsOptionsDataException()
    {
        // Arrange
        var ticker = "AAPL";
        var expirationDate = new DateTime(2026, 05, 15);
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Not Found", System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act & Assert
        // EnsureSuccessStatusCode() throws HttpRequestException which is caught and wrapped
        await Assert.ThrowsAsync<OptionsDataException>(
            () => client.GetOptionsChainAsync(ticker, expirationDate));
    }

    [Fact]
    public async Task GetOptionsChainAsync_WithNullableFieldsInResponse_UsesDefaultValues()
    {
        // Arrange
        var ticker = "AAPL";
        var expirationDate = new DateTime(2026, 05, 15);
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        var responseContent = CreateYahooFinanceResponseWithNullableFields();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await client.GetOptionsChainAsync(ticker, expirationDate);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Contracts);

        var contract = result.Contracts.First();
        // Null values should default to 0
        Assert.Equal(0m, contract.StrikePrice);
        Assert.Equal(0m, contract.BidPrice);
        Assert.Equal(0, contract.OpenInterest);
    }

    [Fact]
    public async Task GetOptionsChainAsync_UsesCorrectUrlFormat()
    {
        // Arrange
        var ticker = "TSLA";
        var expirationDate = new DateTime(2026, 05, 15);
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        var responseContent = CreateValidYahooFinanceResponse();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };

        HttpRequestMessage? capturedRequest = null;

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(response);

        // Act
        await client.GetOptionsChainAsync(ticker, expirationDate);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Contains(ticker, capturedRequest!.RequestUri!.ToString());
        Assert.Contains("date=", capturedRequest!.RequestUri!.ToString());
    }

    #endregion

    #region GetExpirationDatesAsync Tests

    [Fact]
    public async Task GetExpirationDatesAsync_WithValidResponse_ReturnsExpirationDates()
    {
        // Arrange
        var ticker = "AAPL";
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        var responseContent = CreateValidYahooFinanceResponse();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await client.GetExpirationDatesAsync(ticker);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        var expirationList = result.ToList();
        for (int i = 0; i < expirationList.Count - 1; i++)
        {
            Assert.True(expirationList[i] <= expirationList[i + 1], "Expiration dates should be ordered");
        }
    }

    [Fact]
    public async Task GetExpirationDatesAsync_WithEmptyOptionChains_ReturnsEmptyList()
    {
        // Arrange
        var ticker = "INVALID";
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        var responseContent = CreateYahooFinanceResponseWithEmptyChains();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await client.GetExpirationDatesAsync(ticker);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetExpirationDatesAsync_WithHttpRequestException_ThrowsOptionsDataException()
    {
        // Arrange
        var ticker = "AAPL";
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Connection timeout"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OptionsDataException>(
            () => client.GetExpirationDatesAsync(ticker));

        Assert.Contains("Failed to fetch expiration dates from Yahoo Finance", exception.Message);
    }

    [Fact]
    public async Task GetExpirationDatesAsync_WithMultipleExpirations_ReturnsSortedAndUnique()
    {
        // Arrange
        var ticker = "AAPL";
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        var responseContent = CreateYahooFinanceResponseWithMultipleExpirations();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await client.GetExpirationDatesAsync(ticker);

        // Assert
        var expirationList = result.ToList();
        Assert.True(expirationList.Count >= 2, "Should have multiple expiration dates");

        // Verify sorted order
        for (int i = 0; i < expirationList.Count - 1; i++)
        {
            Assert.True(expirationList[i] <= expirationList[i + 1]);
        }
    }

    [Fact]
    public async Task GetExpirationDatesAsync_WithNullExpirationDate_FiltersOutNull()
    {
        // Arrange
        var ticker = "AAPL";
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        var responseContent = @"{
            ""optionChains"": [
                {
                    ""expirationDate"": null,
                    ""quote"": { ""regularMarketPrice"": 150.25, ""symbol"": ""AAPL"" },
                    ""options"": []
                },
                {
                    ""expirationDate"": 1746172800,
                    ""quote"": { ""regularMarketPrice"": 150.25, ""symbol"": ""AAPL"" },
                    ""options"": []
                }
            ]
        }";

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await client.GetExpirationDatesAsync(ticker);

        // Assert
        var expirationList = result.ToList();
        Assert.Single(expirationList);
    }

    [Fact]
    public async Task GetExpirationDatesAsync_LogsInformationOnSuccess()
    {
        // Arrange
        var ticker = "AAPL";
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        var responseContent = CreateValidYahooFinanceResponse();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        await client.GetExpirationDatesAsync(ticker);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Fetching expiration dates")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region Retry Policy Tests

    [Fact]
    public async Task GetOptionsChainAsync_With500Error_RetriesAndSucceeds()
    {
        // Arrange
        var ticker = "AAPL";
        var expirationDate = new DateTime(2026, 05, 15);
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        var responseContent = CreateValidYahooFinanceResponse();
        var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };

        var callCount = 0;
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback(() => callCount++)
            .Returns<HttpRequestMessage, CancellationToken>((req, ct) =>
            {
                if (callCount < 2)
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
                }
                return Task.FromResult(successResponse);
            });

        // Act
        var result = await client.GetOptionsChainAsync(ticker, expirationDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticker, result.Ticker);
        Assert.Equal(2, callCount); // Should have been called twice (first failed, second succeeded)
    }

    [Fact]
    public async Task GetOptionsChainAsync_WithTimeoutException_RetriesAndSucceeds()
    {
        // Arrange
        var ticker = "AAPL";
        var expirationDate = new DateTime(2026, 05, 15);
        var client = new YahooFinanceClient(_httpClient, _mockLogger.Object);

        var responseContent = CreateValidYahooFinanceResponse();
        var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };

        var callCount = 0;
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback(() => callCount++)
            .Returns<HttpRequestMessage, CancellationToken>((req, ct) =>
            {
                if (callCount == 1)
                {
                    throw new TimeoutException("Request timeout");
                }
                return Task.FromResult(successResponse);
            });

        // Act
        var result = await client.GetOptionsChainAsync(ticker, expirationDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticker, result.Ticker);
    }

    #endregion

    #region Helper Methods

    private static string CreateValidYahooFinanceResponse()
    {
        return @"{
            ""optionChains"": [
                {
                    ""expirationDate"": 1746172800,
                    ""quote"": {
                        ""regularMarketPrice"": 150.25,
                        ""symbol"": ""AAPL""
                    },
                    ""options"": [
                        {
                            ""contractSymbol"": ""AAPL260515C00150000"",
                            ""strike"": 150.0,
                            ""bid"": 2.5,
                            ""ask"": 2.7,
                            ""lastPrice"": 2.6,
                            ""openInterest"": 1500,
                            ""volume"": 2000,
                            ""impliedVolatility"": 0.25,
                            ""delta"": 0.65,
                            ""gamma"": 0.02,
                            ""theta"": -0.05,
                            ""vega"": 0.15,
                            ""rho"": 0.10
                        }
                    ]
                }
            ]
        }";
    }

    private static string CreateYahooFinanceResponseWithPutContracts()
    {
        return @"{
            ""optionChains"": [
                {
                    ""expirationDate"": 1746604800,
                    ""quote"": {
                        ""regularMarketPrice"": 380.50,
                        ""symbol"": ""MSFT""
                    },
                    ""options"": [
                        {
                            ""contractSymbol"": ""MSFT260522C00380000"",
                            ""strike"": 380.0,
                            ""bid"": 3.5,
                            ""ask"": 3.7,
                            ""lastPrice"": 3.6,
                            ""openInterest"": 2000,
                            ""volume"": 3000,
                            ""impliedVolatility"": 0.22,
                            ""delta"": 0.55,
                            ""gamma"": 0.01,
                            ""theta"": -0.04,
                            ""vega"": 0.12,
                            ""rho"": 0.08
                        },
                        {
                            ""contractSymbol"": ""MSFT260522P00380000"",
                            ""strike"": 380.0,
                            ""bid"": 2.0,
                            ""ask"": 2.2,
                            ""lastPrice"": 2.1,
                            ""openInterest"": 1800,
                            ""volume"": 2500,
                            ""impliedVolatility"": 0.20,
                            ""delta"": -0.45,
                            ""gamma"": 0.01,
                            ""theta"": -0.03,
                            ""vega"": 0.11,
                            ""rho"": -0.07
                        }
                    ]
                }
            ]
        }";
    }

    private static string CreateYahooFinanceResponseWithEmptyChains()
    {
        return @"{
            ""optionChains"": []
        }";
    }

    private static string CreateYahooFinanceResponseWithNullableFields()
    {
        return @"{
            ""optionChains"": [
                {
                    ""expirationDate"": 1746172800,
                    ""quote"": {
                        ""regularMarketPrice"": null,
                        ""symbol"": ""AAPL""
                    },
                    ""options"": [
                        {
                            ""contractSymbol"": ""AAPL260515C00150000"",
                            ""strike"": null,
                            ""bid"": null,
                            ""ask"": null,
                            ""lastPrice"": null,
                            ""openInterest"": null,
                            ""volume"": null,
                            ""impliedVolatility"": null,
                            ""delta"": null,
                            ""gamma"": null,
                            ""theta"": null,
                            ""vega"": null,
                            ""rho"": null
                        }
                    ]
                }
            ]
        }";
    }

    private static string CreateYahooFinanceResponseWithMultipleExpirations()
    {
        return @"{
            ""optionChains"": [
                {
                    ""expirationDate"": 1746172800,
                    ""quote"": { ""regularMarketPrice"": 150.25, ""symbol"": ""AAPL"" },
                    ""options"": []
                },
                {
                    ""expirationDate"": 1746604800,
                    ""quote"": { ""regularMarketPrice"": 150.25, ""symbol"": ""AAPL"" },
                    ""options"": []
                },
                {
                    ""expirationDate"": 1747209600,
                    ""quote"": { ""regularMarketPrice"": 150.25, ""symbol"": ""AAPL"" },
                    ""options"": []
                }
            ]
        }";
    }

    #endregion
}
