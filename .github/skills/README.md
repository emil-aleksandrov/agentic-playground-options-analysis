# Skills

This directory contains reusable knowledge bases that agents can reference. Skills provide consolidated expertise on specific topics that multiple agents may need.

## Available Skills

### Testing

- [.NET Testing Fundamentals](testing/dotnet-testing-fundamentals.skill.md) - Shared testing knowledge for both unit and integration testing in .NET

## Skill Format

Each skill file contains:

- **Name & Description**: What knowledge the skill provides
- **Organization**: Logical sections for easy reference
- **Examples**: Code samples and patterns
- **Best Practices**: Proven approaches and anti-patterns to avoid
- **Metadata**: Version, last updated, contributed by

## How Agents Use Skills

Agents reference skills in their instructions to:

1. Avoid repeating common knowledge
2. Ensure consistency across related work
3. Provide specific links to detailed guidance
4. Enable collaborative knowledge building

**Example:** Both Unit Testing Agent and Integration Testing Agent reference `dotnet-testing-fundamentals.skill.md` for shared testing principles, while each focuses on their specialty.

## Organizing Skills

Skills are organized by category:

```
skills/
├── README.md (this file)
├── testing/
│   ├── dotnet-testing-fundamentals.skill.md
│   ├── xunit-patterns.skill.md (future)
│   └── moq-patterns.skill.md (future)
├── architecture/
│   ├── clean-architecture.skill.md (future)
│   └── design-patterns.skill.md (future)
├── backend/
│   └── aspnet-core-best-practices.skill.md (future)
└── database/
    └── ef-core-patterns.skill.md (future)
```

## Creating New Skills

### When to Create a Skill

Create a new skill when:

- Multiple agents need the same knowledge
- Knowledge is detailed enough to deserve its own document
- Knowledge is stable and likely to be referenced often
- Knowledge is cross-cutting (applies to many scenarios)

### When NOT to Create a Skill

- One-off knowledge specific to a single agent
- Highly specialized knowledge used by only one agent
- Knowledge that changes frequently or is experimental

### Skill Template

```yaml
---
name: [Skill Name]
description: [One-line description of what this skill provides]
version: 1.0.0
last-updated: YYYY-MM-DD
contributed-by: [Team member or GitHub username]
category: [testing|architecture|backend|database|other]
related-agents: [List of agents that reference this]
changelog:
  - version: 1.0.0
    date: YYYY-MM-DD
    changes: Initial creation
---

# [Skill Name]

[Brief introduction]

## [Section 1]

[Content]

## [Section 2]

[Content]
```

## Referencing Skills in Agents

In agent instructions, reference skills like this:

```markdown
### Shared Knowledge

This agent leverages the following shared skills:

- [.NET Testing Fundamentals](../skills/testing/dotnet-testing-fundamentals.skill.md)
  for testing frameworks, patterns, and best practices
- [Clean Architecture](../skills/architecture/clean-architecture.skill.md)
  for architectural guidance

See those skills for detailed information on [topic].
```

## Maintaining Skills

### Review Process

- Review skills quarterly for accuracy and relevance
- Update skills when new patterns emerge
- Archive obsolete skills
- Version skills to track evolution

### Version Format

Use semantic versioning:

- **1.0.0** → Initial version
- **1.1.0** → Minor updates (added sections, clarifications)
- **2.0.0** → Major changes (restructured, contradicts previous version)

### Cross-References

Keep skills cross-referenced with agents and tasks:

- Link from agent instructions to skills
- Link from tasks to relevant skills when applicable
- Update this README when skills are added/removed

## Examples of Effective Skills

### Good Skill (Reusable)

- ".NET Testing Fundamentals" - Referenced by Unit Testing Agent, Integration Testing Agent, and Coding Agent
- Covers principles, patterns, and examples applicable to all testing scenarios
- Stable knowledge that doesn't change frequently

### Better Candidates for New Skills

- "ASP.NET Core Best Practices" - Useful for Coding Agent, Testing Agents, and Architectural guidance
- "Entity Framework Core Patterns" - Useful for Coding Agent, Integration Testing Agent, database architecture
- "Design Patterns for .NET" - Useful for Coding Agent and Research Agent

## Contributing

### Adding a New Skill

1. Discuss need for the skill with the team
2. Create the skill file in appropriate category folder
3. Include metadata with version and contributors
4. Add to this README
5. Update relevant agent files to reference it

### Improving Existing Skills

1. Make improvements to the skill file
2. Update version number
3. Update the changelog
4. Update last-updated date
5. Notify users (agents, team) of changes

## Discoverability

To find skills relevant to your task:

1. Check agent instructions for skill references
2. Browse this README for available skills by category
3. Search skill content for specific topics
4. Ask agents which skills are relevant to your question
