# Agent Usage Guidelines

This guide provides best practices for working effectively with AI agents, along with common patterns, anti-patterns, and tips for structuring requests.

## Fundamental Principles

### 1. Clarity and Context

**✅ Good:**

> "Research Agent: We need to evaluate database options for storing real-time options market data. Requirements: 1) High write throughput (10K+ writes/sec), 2) Complex queries for analysis, 3) Can afford 15-30 min data latency. Current consideration: SQLite vs PostgreSQL vs TimescaleDB."

**❌ Bad:**

> "Research database options"

**Why it matters:** Agents need context to provide relevant recommendations. Include:

- Why you need the research (business drivers)
- Constraints and requirements
- Trade-offs you care about
- What you've already considered

### 2. Concrete Acceptance Criteria

**✅ Good:**

> "Coding Agent: Implement the GetOptionsChainAsync method. Acceptance criteria: 1) Return DTO with strike price, implied volatility, and Greeks, 2) Handle timeouts gracefully with retry logic, 3) Include unit tests with mocked HTTP responses, 4) Follow existing repository patterns in YahooFinanceClient."

**❌ Bad:**

> "Implement the GetOptionsChainAsync method"

**Why it matters:** Without acceptance criteria, agents may miss important details or generate different output than expected.

### 3. Reference Existing Work

**✅ Good:**

> "Task Writing Agent: Convert research from `.research/technology/2026-04-15-gamma-exposure-gex-platform-research.md` into a GitHub Epic with subtasks for implementation."

**❌ Bad:**

> "Create tasks for the GEX research"

**Why it matters:** Linking to existing research and documentation helps agents:

- Maintain consistency with previous decisions
- Avoid duplicating work
- Provide specific context

## Workflow Patterns

### Pattern 1: Research → Plan → Implement → Test

This is the recommended workflow:

```
1. Research Agent: Investigate approach
   Output: Research document with recommendations

2. Task Writing Agent: Convert research to tasks
   References: Research document from step 1
   Output: GitHub Issues with acceptance criteria

3. Coding Agent: Implement feature
   References: GitHub Issue from step 2
   Output: Production-ready code

4. Testing Agents: Write tests
   References: Code from step 3, Issue from step 2
   Output: Comprehensive test coverage
```

### Pattern 2: Code Review Collaboration

**Scenario:** You've written code and want agent review

```
Coding Agent: "Review the YahooFinanceClient implementation at
src/GexPlatform.Infrastructure/Services/YahooFinanceClient.cs
against these criteria:

1) Error handling - are all failure scenarios covered?
2) Async patterns - is async/await used correctly?
3) Testability - are dependencies properly injected?
4) Performance - any obvious optimization opportunities?

Provide specific recommendations and code examples for improvements."
```

### Pattern 3: Feature Decomposition

**Scenario:** Large feature that needs breaking down

```
Task Writing Agent: "Break down this feature into GitHub Issues:

Feature: User can analyze gamma exposure for TSLA options

Requirements:
- Fetch options chain from Yahoo Finance
- Calculate gamma exposure per strike
- Display results in interactive chart
- Cache results for 30 minutes
- Handle API failures gracefully

Create an Epic with Story cards and Technical Tasks,
ordered by dependencies. Estimate story points."
```

## Advanced Techniques

### Cross-Agent Collaboration

Agents can reference each other's outputs:

```
Sequence:
1. Research Agent produces research document
2. Task Writing Agent references research → creates issues
3. Coding Agent references issues → implements features
4. Testing Agent references code → writes tests
5. All reference the original research for validation

This ensures consistency across the entire workflow.
```

### Leveraging Skills

When requesting work, mention relevant skills:

```
Coding Agent: "Implement GetOptionsChainAsync following the patterns
in .github/skills/backend/aspnet-core-best-practices.skill.md for
error handling and .github/skills/architecture/clean-architecture.skill.md
for service layer organization."
```

### Asking for Multiple Outputs

```
Unit Testing Agent: "Write unit tests for OptionChainService.

Deliverables:
1) Happy path tests (valid symbols, valid expiration dates)
2) Error handling tests (API failures, timeouts, malformed responses)
3) Edge case tests (null inputs, empty results)
4) Performance tests (< 100ms per test)

Use xUnit with Moq for mocking. Reference
.github/skills/testing/dotnet-testing-fundamentals.skill.md for patterns."
```

## Anti-Patterns to Avoid

### ❌ Anti-Pattern 1: Vague Scope

**Bad:** "Build the options analysis feature"

**Why it fails:** Too broad - agent doesn't know what to prioritize, how much detail to include, or where to start.

**Better:** "Implement the GetExpirationDatesAsync method for YahooFinanceClient. Should parse response to extract unique expiration dates, handle parsing errors, and return sorted list. See existing GetOptionsChainAsync for patterns."

### ❌ Anti-Pattern 2: Contradictory Requirements

**Bad:** "Write minimal tests but ensure 100% code coverage"

**Why it fails:** Agent must choose between conflicting goals.

**Better:** "Write focused tests for critical paths (business logic, error handling). Aim for 80% coverage on service layer, 100% on domain models. Skip trivial getters/setters."

### ❌ Anti-Pattern 3: Assuming Agent Knowledge

**Bad:** "Use the standard GEX calculation approach"

**Why it fails:** Agent may not know your specific "standard" approach.

**Better:** "Use the Black-Scholes gamma formula implemented in MathNet.Numerics. For market maker positions, use the aggregate of all options Greeks by strike. See formula in Architecture Decision Record ADR-002."

### ❌ Anti-Pattern 4: No Success Criteria

**Bad:** "Implement an options data caching layer"

**Why it fails:** Agent doesn't know when it's done or what "good" looks like.

**Better:** "Implement a caching layer that: 1) Caches options chain by symbol, 2) TTL of 30 minutes, 3) Returns cached data if fresh, 4) Invalidates on manual refresh, 5) Handles cache misses transparently. Success when all integration tests pass."

## Prompting Structure Template

Use this structure for complex requests:

```
[Agent Name]: [Primary request]

## Context
[Why this is needed]
[Related tasks/research]
[Important constraints]

## Scope
- Include: [Specific deliverables]
- Exclude: [Out of scope]

## Acceptance Criteria / Success Metrics
- [ ] Criterion 1
- [ ] Criterion 2
- [ ] Criterion 3

## References
- Related issue: [Link]
- Related code: [Path]
- Related research: [Link]
- Related skill: [skill.md]

## Additional Notes
[Anything else that helps the agent deliver quality work]
```

## Common Scenarios

### Scenario 1: Code Review Request

```
.NET Backend Coding Agent:

Please review the OrderService implementation at
src/GexPlatform.Application/Services/OrderService.cs

Focus on:
1) Does it follow the clean architecture pattern?
2) Are dependencies properly injected?
3) Are all async operations properly handled?
4) Is error handling comprehensive?

Provide specific recommendations with code examples for any issues.
Reference .github/skills/backend/aspnet-core-best-practices.skill.md for standards.
```

### Scenario 2: Test Coverage Gaps

```
Unit Testing Agent:

Our YahooFinanceClient has low test coverage for error scenarios.
Current tests at src/GexPlatform.Tests/YahooFinanceClientTests.cs

Add tests for:
1) HTTP timeout scenarios (30 second timeout)
2) Malformed JSON responses
3) Circuit breaker state transitions
4) Retry policy with exponential backoff

Use the existing test patterns and Moq library.
Reference .github/skills/testing/dotnet-testing-fundamentals.skill.md.
```

### Scenario 3: Architecture Decision

```
Research Agent:

We need to evaluate data storage for high-frequency options data.
Requirements:
- 10K+ writes/sec
- Support complex analytical queries
- Real-time data freshness (< 1 minute)
- Cost-conscious (startup budget)

Evaluate: SQLite, PostgreSQL, TimescaleDB, and DuckDB
Consider: Performance, cost, operational complexity, learning curve

Produce a research document with recommendation and implementation path.
```

## Feedback Loop

### Refining Agent Output

If agent output isn't quite right:

**Step 1: Clarify your request**

```
Coding Agent: "The implementation is close, but needs adjustment:

Current: [What it did]
Expected: [What you need]
Specific issue: [The gap]

Please adjust to: [Specific requirement]"
```

**Step 2: Provide examples**

```
Coding Agent: "I need the error handling to match this pattern:

[Example code from existing codebase]

The current implementation differs in: [Specific difference]
Please update to follow this pattern."
```

**Step 3: Reference standards**

```
Coding Agent: "This doesn't match the project standards.

Review .github/skills/backend/aspnet-core-best-practices.skill.md
section 'Error Handling' and adjust accordingly."
```

## Performance Tips

### Tip 1: Batch Related Work

**✅ Efficient:**

```
Task Writing Agent: "Create GitHub Issues for the entire 'Options Data Ingestion' epic.

Feature breakdown:
1. Design research (research-agent-output.md)
2. API client implementation (3 stories)
3. Data persistence layer (2 stories)
4. Cache layer (1 story)

Create Epic with Stories and Tasks, ordered by dependencies."
```

**❌ Inefficient:**

```
Multiple separate requests:
"Task Writing Agent: Create issue for API client"
"Task Writing Agent: Create issue for data persistence"
"Task Writing Agent: Create issue for cache"
```

### Tip 2: Reuse Agent Outputs

```
Workflow:
Research Agent → [Creates research doc]
Task Writing Agent → [References research doc] → [Creates issues]
Coding Agent → [References issues] → [Implements features]

This is more efficient than:
Coding Agent → [Re-research from scratch]
```

### Tip 3: Be Specific About Constraints

```
✅ Good: "Performance requirement: each test < 100ms"
❌ Vague: "Make it fast"

✅ Good: "Code coverage target: 80% on service layer, 100% on domain models"
❌ Vague: "Good test coverage"
```

## Monitoring Agent Effectiveness

### Questions to Ask

- Is the output usable without rework?
- Did the agent ask clarifying questions?
- Is the code style consistent with the project?
- Are acceptance criteria met?

### Improving Over Time

1. Track which request patterns produce best results
2. Document effective prompting approaches
3. Update this guide with patterns that work well
4. Create agent-specific examples in related issues

## When to Ask Multiple Agents

```
Effective multi-agent request:

1. Research Agent: Investigate options
2. [Then] Task Writing Agent: Plan the work
3. [Then] Coding Agent: Implement
4. [Then] Testing Agent: Add tests

Sequential, with outputs feeding into next step.
```

```
❌ Inefficient multi-agent request:

Ask all agents the same question in parallel.
They duplicate work and may contradict each other.
```

## Resources

- See [README.md](README.md) for agent descriptions
- See [../skills/README.md](../skills/README.md) for available skills
- Check `.research/` directory for previous research documents
- Review `.github/issues` for task examples in your project
