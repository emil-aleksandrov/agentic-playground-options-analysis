# Agents

This directory contains specialized AI agents for different project tasks. Each agent is a self-contained guide with specific expertise, competencies, and output formats.

## Available Agents

| Agent                                                             | Purpose                                                                                     | When to Use                                                              |
| ----------------------------------------------------------------- | ------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------ |
| [.NET Backend Coding Agent](dotnet-backend-coding-agent.agent.md) | Design and implement .NET backend services with clean, maintainable OOP code                | Feature development, bug fixes, code reviews, architectural guidance     |
| [Integration Testing Agent](integration-testing-agent.agent.md)   | Write integration tests that verify component interactions and external system integrations | Testing multi-component workflows, database operations, API integrations |
| [Unit Testing Agent](unit-testing-agent.agent.md)                 | Write comprehensive, maintainable unit tests with proper mocking and test patterns          | Testing individual methods/classes, edge cases, error handling           |
| [Research Agent](research-agent.agent.md)                         | Investigate topics, analyze requirements, and evaluate solutions                            | Technology decisions, requirements breakdown, architecture research      |
| [Task Writing Agent](task-writing-agent.agent.md)                 | Create structured GitHub Issues, epics, and project work items                              | Convert research/requirements to actionable tasks                        |

## Recommended Workflow

The agents work together in a natural project workflow:

```
1. RESEARCH
   ↓
   Use Research Agent to investigate requirements, technology options,
   and architectural approaches. Produces research documentation.
   ↓
2. PLAN
   ↓
   Use Task Writing Agent to convert research findings and requirements
   into structured GitHub Issues with acceptance criteria.
   ↓
3. IMPLEMENT
   ↓
   Use .NET Backend Coding Agent to implement features based on tasks.
   ↓
4. TEST
   ↓
   Use Integration Testing Agent for component/workflow testing
   Use Unit Testing Agent for individual method/class testing
```

## Using Agents

### Basic Prompting Pattern

When invoking an agent, provide:

1. **What**: Clear description of what you need
2. **Context**: Relevant background information
3. **Constraints**: Any specific requirements or limitations
4. **References**: Links to related tasks, research, or code

**Example:**

> "Research Agent: Investigate database options for the GEX platform. We need to support real-time options data storage with high write throughput. Consider SQLite for development, PostgreSQL for production."

### Agent Specifications

Each agent file contains:

- **Name & Description**: What the agent does
- **Argument Hint**: Examples of what to ask the agent
- **Instructions**: Detailed competencies, processes, and output formats
- **Clarification Questions**: Questions to ask if the request is ambiguous

### Referencing Skills

Agents may reference shared skills from the `.github/skills/` directory. These skills provide reusable knowledge that multiple agents can leverage.

## Agent Output Expectations

Each agent produces specific outputs based on its specialty:

### .NET Backend Coding Agent

- Production-ready code following SOLID principles
- Clean architecture patterns
- Comprehensive error handling
- Async/await patterns where applicable
- Code organized in appropriate layers (Domain, Application, Infrastructure, Presentation)

### Integration Testing Agent

- Test classes with proper setup/teardown (IAsyncLifetime)
- Tests for real component interactions
- Database or HTTP mocking patterns
- Complete test scenarios covering workflows

### Unit Testing Agent

- Individual test methods with Arrange-Act-Assert pattern
- Mock implementations for dependencies
- Edge cases and error scenarios
- Fast, deterministic tests

### Research Agent

- Clear research summaries with evidence
- Comparative analysis of options
- Specific recommendations with justification
- Actionable next steps

### Task Writing Agent

- Well-structured GitHub Issues
- Clear acceptance criteria
- Proper dependencies and sequencing
- Estimation and labeling

## Guidelines

Consult [GUIDELINES.md](GUIDELINES.md) for:

- Detailed tips for effective agent prompting
- Common patterns and anti-patterns
- How to structure complex requests
- Best practices for agent collaboration

## Contributing

### Adding New Agents

1. Create a new `[agent-name].agent.md` file
2. Follow the standard agent format with metadata and instructions
3. Include specific examples and output formats
4. Add clarification questions for ambiguous scenarios
5. Reference relevant skills
6. Update this README with the new agent

### Updating Agents

- Update agent instructions based on lessons learned
- Keep metadata accurate and current
- Test agent prompts before documenting them
- Update GUIDELINES.md with new patterns discovered

## Maintenance

- Review agent instructions quarterly for relevance
- Gather feedback from team members on agent effectiveness
- Update agents based on project needs evolution
- Archive agents that are no longer used
