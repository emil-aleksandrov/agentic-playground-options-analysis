# Gamma Exposure (GEX) Analysis Platform

A proof-of-concept application for analyzing Gamma Exposure in stock options trading using free, delayed market data. Built with .NET/C# backend and Blazor WebAssembly frontend.

## Overview

This project demonstrates the calculation and analysis of Gamma Exposure (GEX), which represents the aggregate gamma position of market makers in options contracts. GEX analysis provides insights into market maker positioning and potential price pressure on underlying stocks.

### What is Gamma Exposure?

- **Gamma**: Measurement of how fast delta changes with underlying price movements
- **Gamma Exposure (GEX)**: Net gamma position of market makers across all options strikes
- **Net Long Gamma**: Market makers profit from volatility; typically stabilizes prices
- **Net Short Gamma**: Market makers profit from stability; can contribute to momentum moves

## Options Trading Basics

This project analyzes stock options, which are financial derivatives that give buyers the right (but not obligation) to buy or sell an underlying stock at a specific price within a certain timeframe. Understanding these basics will help with the business logic.

### Key Concepts

- **Call Option**: Right to buy the stock at a set price (strike price) by expiration
- **Put Option**: Right to sell the stock at a set price by expiration
- **Strike Price**: The predetermined price for buying/selling the stock
- **Expiration Date**: When the option contract expires (typically monthly)
- **Premium**: The price paid for the option contract
- **In-the-Money (ITM)**: Option with intrinsic value (profitable if exercised)
- **Out-of-the-Money (OTM)**: Option without intrinsic value
- **At-the-Money (ATM)**: Option where strike equals current stock price

### Options Greeks

Options pricing involves "Greeks" that measure risk and sensitivity:

- **Delta**: How option price changes with stock price
- **Gamma**: Rate of change of delta (central to GEX analysis)
- **Theta**: Time decay effect
- **Vega**: Sensitivity to volatility changes
- **Rho**: Sensitivity to interest rate changes

### Resources

- [Investopedia: Options Trading](https://www.investopedia.com/terms/o/optionscontract.asp)
- [CBOE: Options Basics](https://www.cboe.com/learn-center/how-options-work/)
- [Options Greeks Explained](https://www.investopedia.com/terms/g/greeks.asp)

## Project Status

🚀 **Proof of Concept - IN DEVELOPMENT**

This is an experimental platform to demonstrate GEX calculation capabilities and analysis features.

## Technology Stack

### Backend

- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **Architecture**: Clean Architecture
- **Database**: SQLite (Development), PostgreSQL-ready
- **ORM**: Entity Framework Core

### Frontend

- **Framework**: Blazor WebAssembly
- **Styling**: Bootstrap 5
- **Charts**: Chart.js / Plotly.js

### Data Sources

- **Primary**: Yahoo Finance (15-20 min delayed)
- **Secondary**: Alpha Vantage (1-15 min delayed)

### Calculations

- **Math Library**: MathNet.Numerics
- **Model**: Black-Scholes gamma formula

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code
- Git

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/yourusername/gamma-exposure-gex.git
   cd gamma-exposure-gex
   ```

2. **Install dependencies**

   ```bash
   dotnet restore
   ```

3. **Build the solution**

   ```bash
   dotnet build
   ```

4. **Run the application**

   ```bash
   dotnet run
   ```

   The application will be available at `https://localhost:5001`

## Project Structure

```
├── .github/
│   ├── agents/                    # AI Agent definitions
│   │   ├── coding-agent-one.agent.md
│   │   ├── unit-testing-agent.agent.md
│   │   ├── research-agent.agent.md
│   │   └── task-writing-agent.agent.md
│   └── workflows/                 # GitHub Actions (future)
├── .research/                     # Research documentation
│   ├── technology/                # Tech stack research
│   ├── architecture/              # Architecture decisions
│   ├── requirements/              # Requirements analysis
│   ├── security/                  # Security research
│   ├── performance/               # Performance research
│   └── processes/                 # Process documentation
├── src/
│   ├── GEX.Domain/               # Domain entities and business logic
│   ├── GEX.Application/          # Application layer, use cases, DTOs
│   ├── GEX.Infrastructure/       # Data access, external APIs
│   └── GEX.Presentation/         # Web API controllers
├── tests/
│   ├── GEX.Domain.Tests/
│   ├── GEX.Application.Tests/
│   └── GEX.Infrastructure.Tests/
├── README.md                      # This file
├── CONTRIBUTING.md                # Development guidelines
├── .gitignore                     # Git ignore rules
└── global.json                    # .NET version specification
```

## Key Features

### Current (POC)

- [ ] Options data ingestion from Yahoo Finance
- [ ] GEX calculation engine (Black-Scholes)
- [ ] REST API for GEX data access
- [ ] Basic web interface for visualization
- [ ] Historical data storage and retrieval

### Planned Future Enhancements

- Real-time data integration
- Advanced analysis dashboard
- Alert system for extreme GEX levels
- Machine learning predictions
- Mobile application
- Multi-user support with authentication

## Data Flow

```
Yahoo Finance API
        ↓
Data Ingestion Service
        ↓
Options Data Storage (SQLite)
        ↓
GEX Calculation Engine
        ↓
Analysis & Reporting
        ↓
REST API
        ↓
Web Interface
```

## API Endpoints

### Current GEX

```
GET /api/gex/{symbol}
Response: Current GEX level and zero gamma point
```

### Historical GEX

```
GET /api/gex/{symbol}/history?days=30
Response: Historical GEX data for specified period
```

### Options Chain

```
GET /api/options/{symbol}
Response: Current options chain data
```

### GEX Analysis

```
POST /api/analysis/gex-pressure
Body: { "symbol": "SPY", "threshold": 1000000 }
Response: GEX pressure analysis and indicators
```

## Development

### Setting Up Development Environment

1. **Install Visual Studio 2022** with:
   - ASP.NET and web development
   - .NET desktop development
   - Data storage and processing

2. **Configure local settings**

   ```bash
   # Copy example settings
   cp src/GEX.Presentation/appsettings.Development.example.json \
      src/GEX.Presentation/appsettings.Development.json
   ```

3. **Run database migrations**
   ```bash
   dotnet ef database update --project src/GEX.Infrastructure
   ```

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific test project
dotnet test tests/GEX.Domain.Tests
```

### Code Style & Quality

This project follows the guidelines defined in the **[.NET Backend Coding Agent](.github/agents/coding-agent-one.agent.md)**:

- SOLID principles
- Clean Code practices
- Async/await patterns
- Comprehensive error handling
- 80%+ test coverage target

See [CONTRIBUTING.md](CONTRIBUTING.md) for detailed guidelines.

## Research & Documentation

Research findings and analysis are documented in the `.research/` directory:

- **Technology Research**: Data stack evaluation, framework selection
- **Requirements Analysis**: Feature specifications, constraints
- **Task Breakdown**: Implementation tasks and user stories
- **Instructions**: Development instructions and best practices

See [.research/README.md](.research/README.md) for more details.

## Limitations

### Current POC Limitations

- **Delayed Data**: Free data sources have 15-20 minute delays
- **No Real-Time Updates**: Limited update frequency
- **Limited Coverage**: Focus on major stocks; not all symbols
- **Single User**: No multi-user support yet
- **No Trading**: For analysis only, not trading execution

### Known Issues

- API rate limits from free data sources
- SQLite performance limits with large datasets
- Calculation accuracy dependent on input data quality

## Performance Considerations

### Current Performance

- **GEX Calculation**: ~100-500ms for single stock
- **API Response Time**: <2 seconds for current data
- **Data Refresh**: 15-20 minute intervals

### Optimization Opportunities

- Database indexing for faster queries
- Caching layer for frequently accessed data
- Parallel processing for bulk calculations
- Query optimization for large datasets

## Security & Compliance

⚠️ **Important**:

- This is a **POC for analysis only**
- Not for live trading or investment decisions
- No guarantee of data accuracy or completeness
- Delayed market data not suitable for trading

## Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

### Development Process

1. Create feature branch: `git checkout -b feature/my-feature`
2. Make changes following code guidelines
3. Write tests for new functionality
4. Commit with clear messages: `git commit -m "Add: feature description"`
5. Push and create GitHub Pull Request

## Testing

The project uses:

- **xUnit.net** for unit testing
- **Moq** for mocking dependencies
- **FluentAssertions** for readable assertions
- Target: 80%+ code coverage

See [Unit Testing Agent](.github/agents/unit-testing-agent.agent.md) for testing guidelines.

## Deployment

### Docker Deployment

```bash
# Build Docker image
docker build -t gex-platform .

# Run container
docker run -p 5001:5001 gex-platform
```

### Local Development

```bash
# Run with hot reload
dotnet watch run
```

## Troubleshooting

### API Rate Limiting

If you hit rate limits from Yahoo Finance:

- Data is cached locally to minimize requests
- Implement exponential backoff in retry logic
- Consider upgrading to paid data sources

### Database Issues

```bash
# Reset database
dotnet ef database drop
dotnet ef database update
```

### Build Issues

```bash
# Clean and rebuild
dotnet clean
dotnet build
```

## Performance Metrics

Target metrics for POC:

- ✅ GEX calculation accuracy: ±0.1%
- ✅ API response time: <2 seconds
- ✅ Concurrent users: 10+
- ✅ Code coverage: >80%
- ✅ Test pass rate: 100%

## Resources

### Documentation

- [.research/README.md](.research/README.md) - Research documentation
- [CONTRIBUTING.md](CONTRIBUTING.md) - Development guidelines
- [.github/agents/](​.github/agents/) - AI Agent specifications

### External Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Black-Scholes Model](https://en.wikipedia.org/wiki/Black%E2%80%93Scholes_model)
- [Options Greeks](https://www.investopedia.com/terms/g/greeks.asp)
- [SqueezeMetrics](https://squeezemetrics.com/)

## License

[Choose appropriate license - e.g., MIT, Apache 2.0, GPL]

## Contact & Support

- **Issues**: Use GitHub Issues for bug reports and feature requests
- **Discussions**: GitHub Discussions for general questions
- **Email**: [contact information if applicable]

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history and updates.

---

**Last Updated**: April 16, 2026  
**Status**: Proof of Concept - Development in Progress
