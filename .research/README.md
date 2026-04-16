# Research Documentation

This directory contains research findings, analysis, and decision documentation for the project. All research is stored as version-controlled Markdown files.

## Directory Structure

- **`technology/`** - Technology stack decisions, framework evaluations, and tool selections
- **`architecture/`** - Architecture patterns, design decisions, and system design research
- **`requirements/`** - Requirements analysis, stakeholder analysis, and feature specifications
- **`security/`** - Security research, compliance requirements, and vulnerability assessments
- **`performance/`** - Performance optimization research, benchmarking, and scalability analysis
- **`processes/`** - Process improvements, workflow optimization, and team productivity research

## File Naming Convention

- Use kebab-case: `research-topic-name.md`
- Include date prefix: `2024-01-15-database-selection.md`
- Be descriptive and specific

## Document Format

Each research document should include:

```markdown
---
research-date: YYYY-MM-DD
researcher: [Researcher Name]
topic: [Brief topic description]
decision: [Made/Pending/Deferred]
related-tasks: [JIRA-123, JIRA-456]
tags: [comma, separated, tags]
---

# Research: [Topic Title]

## Summary

[Executive summary]

## Questions Investigated

- Question 1: [Answer]
- Question 2: [Answer]

## Options Evaluated

### Option 1: [Name]

- Pros: [List]
- Cons: [List]
- Effort: [High/Medium/Low]

## Recommendation

[Clear recommendation with justification]

## References

- [Links and sources]

## Next Steps

[What to do next]
```

## Usage

1. **Reference in Tasks**: Link research documents in JIRA task descriptions
2. **Update as Needed**: Keep research current as new information becomes available
3. **Version Control**: Commit research alongside related code changes
4. **Cross-Reference**: Link related research documents together

## Agents

- **Research Agent**: Creates these documents through investigation
- **Task Writing Agent**: References these documents when creating tasks
- **Coding Agents**: May reference research for implementation decisions

## Maintenance

- Review research documents periodically for outdated information
- Update decision status as implementations progress
- Archive completed research that is no longer relevant
