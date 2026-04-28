---
name: .NET Testing Fundamentals
description: Shared testing knowledge applicable to both unit and integration testing in .NET, including frameworks, patterns, best practices, and examples.
version: 1.0.0
last-updated: 2026-04-23
category: testing
contributed-by: Platform Team
related-agents:
  - Unit Testing Agent
  - Integration Testing Agent
changelog:
  - version: 1.0.0
    date: 2026-04-23
    changes: Initial creation from extracted unit testing fundamentals
---

# .NET Testing Fundamentals

This skill document contains shared testing knowledge applicable to both unit and integration testing in .NET.

## Core Testing Frameworks & Libraries

### Test Frameworks

- **xUnit.net**: Primary testing framework for .NET (xUnit, xUnit.net)
- **NUnit**: Alternative testing framework with rich assertions
- **MSTest**: Microsoft's testing framework (v2/v3)

### Mocking & Isolation Libraries

- **Moq**: Primary mocking library for .NET
- **NSubstitute**: Alternative mocking library with fluent API

### Assertion & Data Generation

- **FluentAssertions**: Readable assertion library
- **AutoFixture**: Test data generation and object creation
- **Bogus**: Fake data generation for realistic test scenarios

## Testing Principles & Best Practices

### Test Structure (Arrange-Act-Assert)

- **Arrange**: Set up test data, mocks, and system under test
- **Act**: Execute the method/component being tested
- **Assert**: Verify the expected behavior and outcomes
- Keep each test focused on a single behavior or scenario

### Test Naming Conventions

- **Method**: `MethodName_Scenario_ExpectedResult`
- **Class**: `ClassUnderTestTests` or `FeatureNameIntegrationTests`
- **Examples**:
  - `CalculateTotal_ValidItems_ReturnsCorrectSum`
  - `ProcessOrder_InvalidPayment_ThrowsValidationException`
  - `GetUserById_UserExists_ReturnsUser`
  - `CreateOrder_WithValidPayment_SavesToDatabaseSuccessfully`

### Test Categories & Organization

- **Unit Tests**: Test individual methods/classes in isolation
- **Integration Tests**: Test component interactions and external systems
- **Acceptance Tests**: Test end-to-end user scenarios
- **Smoke Tests**: Quick validation of critical paths
- Organize tests in parallel folder structure to production code

### Test Data Management

- Use realistic but minimal test data
- Avoid hard-coded values; use constants or builders
- Create test data builders for complex objects
- Use AutoFixture or Bogus for varied test scenarios
- Consider using database seeders for integration tests

### Assertions & Verification

- Use descriptive assertion messages
- Test both positive and negative scenarios
- Verify exact behavior, not just "no exceptions"
- Use FluentAssertions for readable assertions

### Edge Cases & Error Handling

- Test null/empty inputs
- Test boundary conditions
- Test exception scenarios
- Test timeout and cancellation scenarios
- Test concurrent access where applicable
- Test resource cleanup and disposal

## Advanced Testing Patterns

### Parameterized Tests

- Use `[Theory]` with `[InlineData]` for multiple input scenarios
- Use `[MemberData]` for complex parameter sets
- Test equivalence classes and boundary values

### Test Doubles

- **Dummy**: Objects passed but never used
- **Stub**: Returns predetermined responses
- **Mock**: Verifies interactions and expectations
- **Fake**: Working implementations for testing (in-memory DB)
- **Spy**: Records interactions for later verification

### Async Testing

- Use `async Task` for async test methods
- Test both successful and failed async operations
- Test cancellation scenarios with `CancellationToken`
- Verify async operations complete within expected timeframes

### Exception Testing

- Test specific exception types, not just `Exception`
- Verify exception messages contain expected information
- Test custom exception properties
- Use `Assert.Throws<T>()` or `ShouldThrow<T>()`

## Code Coverage & Quality Metrics

### Coverage Goals

- Aim for 70-80% line coverage on business logic
- Focus on critical paths and complex algorithms
- Don't chase 100% coverage at the expense of test quality
- Cover error handling and edge cases

### Test Quality Indicators

- Tests should be deterministic and repeatable
- Tests should be maintainable and readable
- Tests should provide clear failure messages
- Tests should have minimal external dependencies where appropriate

## Testing Framework Examples

### xUnit.net Basic Pattern

```csharp
[Fact]
public void Method_Scenario_ExpectedResult()
{
    // Arrange
    var sut = new SystemUnderTest();

    // Act
    var result = sut.Method();

    // Assert
    Assert.Equal(expected, result);
}

[Theory]
[InlineData(1, 2, 3)]
[InlineData(0, 0, 0)]
public void Add_ValidNumbers_ReturnsSum(int a, int b, int expected)
{
    // Arrange & Act & Assert
    Assert.Equal(expected, Calculator.Add(a, b));
}
```

### Moq Usage Pattern

```csharp
[Fact]
public void ProcessOrder_ValidOrder_CallsRepository()
{
    // Arrange
    var mockRepo = new Mock<IOrderRepository>();
    var order = new Order { Id = 1, Total = 100 };
    var sut = new OrderService(mockRepo.Object);

    // Act
    sut.ProcessOrder(order);

    // Assert
    mockRepo.Verify(r => r.Save(order), Times.Once);
}
```

### FluentAssertions Pattern

```csharp
[Fact]
public void GetUsers_ReturnsValidUsers()
{
    // Act
    var users = userService.GetUsers();

    // Assert
    users.Should().NotBeNull()
         .And.HaveCountGreaterThan(0)
         .And.OnlyContain(u => u.Id > 0);
}
```

## Test Maintenance & Refactoring

### When Tests Break

- First verify if the production code change is correct
- Update tests to reflect new behavior, not old assumptions
- If tests are too brittle, consider testing behavior over implementation

### Test Smells to Avoid

- Tests that are hard to understand or maintain
- Tests that test multiple things at once
- Tests with complex setup or teardown
- Tests that depend on external state or timing
- Flaky tests that fail intermittently

### Continuous Integration

- Run tests on every commit
- Fail builds on test failures
- Monitor test execution time and coverage trends
- Use parallel test execution for faster feedback

## Tools & Extensions

### Test Runners

- Visual Studio Test Explorer
- dotnet test CLI
- ReSharper/Rider test runners
- Coverlet for coverage reporting

### Additional Tools

- Stryker.NET for mutation testing
- TestStack.White for UI testing
- SpecFlow for BDD-style tests
- ApprovalTests for snapshot testing
