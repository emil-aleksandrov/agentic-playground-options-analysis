# GitHub Issue Structure Guidelines

Based on analysis of existing issues in the agentic-playground-options-analysis project, this document outlines the recommended structure for creating well-organized, actionable GitHub Issues.

---

## Overview

Two patterns currently exist in the repository:

| Pattern      | Issues | Detail Level  | Use Case                                                                   |
| ------------ | ------ | ------------- | -------------------------------------------------------------------------- |
| **Detailed** | #27-30 | Comprehensive | Complex technical tasks requiring clear design and implementation guidance |
| **Simple**   | #21-26 | Basic         | Small tasks, quick wins, or well-understood work                           |

**Recommendation:** Prefer the **Detailed Pattern** for new issues to ensure clarity and reduce ambiguity.

---

## Label Taxonomy

Labels help organize and categorize issues. Use these standard labels consistently:

### Issue Type Labels

| Label           | Color  | Use Case                                      |
| --------------- | ------ | --------------------------------------------- |
| `task`          | Gray   | Technical implementation work                 |
| `feature`       | Green  | New user-facing capability or feature request |
| `bug`           | Red    | Bug fix or unexpected behavior                |
| `spike`         | Purple | Research, investigation, or proof-of-concept  |
| `documentation` | Blue   | Documentation updates or content creation     |

### Component Labels

| Label            | Color  | Use Case                                 |
| ---------------- | ------ | ---------------------------------------- |
| `backend`        | Orange | Backend/API work                         |
| `database`       | Orange | Database schema, migrations, queries     |
| `frontend`       | Orange | UI/UX work                               |
| `infrastructure` | Orange | DevOps, deployment, CI/CD                |
| `testing`        | Orange | Test implementation, test infrastructure |

### Priority Labels

| Label      | Color  | Use Case                                      |
| ---------- | ------ | --------------------------------------------- |
| `critical` | Red    | Blocking progress, production issue, security |
| `high`     | Orange | Important, should be done soon                |
| `medium`   | Yellow | Standard priority work                        |
| `low`      | Gray   | Nice to have, can be deferred                 |

### Status Labels (Use for tracking outside of project board)

| Label              | Color | Use Case                                        |
| ------------------ | ----- | ----------------------------------------------- |
| `in-progress`      | Blue  | Currently being worked on                       |
| `blocked`          | Red   | Waiting on external dependency or another issue |
| `help-wanted`      | Green | Looking for contributors                        |
| `good-first-issue` | Green | Good for new team members                       |

### Topic Labels (Optional, for cross-cutting concerns)

| Label             | Color      | Use Case                                 |
| ----------------- | ---------- | ---------------------------------------- |
| `gex-calculation` | Light Blue | Related to GEX calculation engine        |
| `data-ingestion`  | Light Blue | Related to data fetching and parsing     |
| `performance`     | Light Blue | Performance optimization or benchmarking |
| `security`        | Red        | Security-related concerns                |
| `refactoring`     | Gray       | Code quality improvements, cleanup       |

### Label Usage Guidelines

**Recommended label combinations:**

- Tasks: `task` + `[component]` + `[priority]`
  - Example: `task`, `backend`, `high`
- Bugs: `bug` + `[component]` + `[priority]`
  - Example: `bug`, `database`, `critical`
- Features: `feature` + `[component]` + `[priority]`
  - Example: `feature`, `frontend`, `medium`
- Spikes: `spike` + `[priority]`
  - Example: `spike`, `high`

**Avoid:**

- Too many labels (max 3-4 per issue)
- Overlapping labels (don't use both `high` and `critical`)
- Status labels for issues in GitHub Projects (use project status instead)

---

## Detailed Pattern (Recommended for Technical Tasks)

Use this structure for most development work:

```markdown
## Task: [Task Title]

### Description

[Clear, concise description of what needs to be done and why]

### Scope

**Included:**

- [Feature/component 1]
- [Feature/component 2]
- [Feature/component 3]

**Not Included:**

- [What's explicitly out of scope]
- [Related but separate work]

### Acceptance Criteria

- [ ] Criterion 1: [Specific, testable behavior]
- [ ] Criterion 2: [Specific, testable behavior]
- [ ] Criterion 3: [Specific, testable behavior]
- [ ] Criterion 4: [Specific, testable behavior]

### Technical Details

**Key Information:**

- Platform/Framework: [Relevant tech stack]
- Architecture: [Design patterns, layers]
- Dependencies: [External systems, libraries]

**Requirements:**

- [Specific requirement 1]
- [Specific requirement 2]

### Implementation Details

1. [Implementation step 1]
2. [Implementation step 2]
3. [Implementation step 3]
4. [Implementation step 4]
5. [Implementation step 5]

### Testing Strategy

- Unit Tests: [What to test]
- Integration Tests: [System boundaries to validate]
- Edge Cases: [Boundary conditions]
- Error Scenarios: [Failure modes]
- Mocking: [What should be mocked]

### Dependencies

- **Blocked by:** [Issue #X - Task Name]
- **Blocks:** [Issue #Y - Task Name], [Issue #Z - Task Name]

### Resources

- [Resource Name 1]: [Link or path]
- [Resource Name 2]: [Link or path]
- Documentation: [Link]

### Estimated Story Points

**[Number] points** — [Justification: complexity, scope, uncertainty, dependencies]
```

---

## Simple Pattern (Use for Small/Clear Tasks)

Use for straightforward, well-understood work:

```markdown
## Task: [Task Title]

### Description

[One or two sentence description]

### Acceptance Criteria

- [ ] Criterion 1
- [ ] Criterion 2
- [ ] Criterion 3

### Dependencies

- [Related issues or prerequisites]

### Story Points

[Number]
```

---

## Section Guidance

### Title (Issue Title Field)

**Format:** `[Pattern]: [Specific Action/Component]`

**Examples:**

- ✅ `Task: Set Up SQLite Database Schema for Options Data`
- ✅ `Feature: Implement Black-Scholes Gamma Calculation`
- ✅ `Bug: Fix null reference in UserService.GetById()`
- ❌ `Fix stuff`
- ❌ `Work on backend`

**Guidelines:**

- Use action verbs (Implement, Build, Set Up, Add, Fix, Refactor)
- Be specific about components or features
- Keep to 50-70 characters when possible
- Avoid vague language ("handle", "support")

### Description

**Content:**

- Why is this work needed?
- What problem does it solve?
- Business or technical context
- Who benefits from this?

**Style:**

- Write for someone unfamiliar with the project
- 2-3 sentences maximum for simple issues
- More detail for complex tasks

**Example:**

```
Implement the database schema required to store options chain data and support GEX calculations.
This task establishes the persistent data layer for the platform using SQLite with Entity Framework Core.
```

### Scope

**Included Section:**

- Explicitly list what IS part of this task
- Use bullet points for clarity
- Be specific about components or layers

**Not Included Section:**

- List related work that is explicitly OUT of scope
- Prevents scope creep and clarifies boundaries
- Example: "Data ingestion logic (separate task)"

### Acceptance Criteria

**Requirements:**

- 3-5 criteria typically (fewer = unclear, more = should split)
- Use checkbox format: `- [ ] Criterion text`
- Make each criterion independently testable
- Each criterion should be verifiable without seeing other criteria

**Style Guidelines:**

- ✅ "User receives 401 Unauthorized when providing invalid credentials"
- ❌ "Authentication works"

- ✅ "API returns paginated results with 25 items per page"
- ❌ "Implement pagination"

- ✅ "Null input validation throws ArgumentNullException with message 'Email cannot be null'"
- ❌ "Handle null inputs"

### Technical Details

**When to include:** For tasks requiring specific technology choices, architecture decisions, or constraints.

**Sections:**

- **Platform/Framework:** What tech is involved?
- **Architecture:** Design patterns, layer structure
- **Constraints:** Performance targets, data limits
- **Key Concepts:** Domain knowledge needed

**Example:**

```
**Database Platform:** SQLite
**ORM:** Entity Framework Core 8.x
**Architecture:** Repository pattern for data access layer
```

### Implementation Details

**Content:**

- Step-by-step breakdown of how to implement
- Numbered list for clarity
- File/folder references where relevant
- Where to put code (GexPlatform.Domain, GexPlatform.Infrastructure, etc.)

**Example:**

```
1. Define domain entities in GexPlatform.Domain/Entities
2. Create DbContext in GexPlatform.Infrastructure/Data
3. Configure relationships, indexes, and constraints
4. Create EF Core migration
5. Add data access abstractions (IRepository pattern)
```

### Testing Strategy

**Sections:**

- **Unit Tests:** What logic needs unit test coverage?
- **Integration Tests:** System boundaries and interactions?
- **Edge Cases:** Boundary conditions, extreme values?
- **Error Scenarios:** What can go wrong? API failures, invalid data?
- **Mocking:** What external dependencies to mock?

**Example:**

```
- Unit Tests: Parsing logic, validation rules, error handling
- Integration Tests: Full ingestion pipeline (fetch → parse → validate)
- Edge Cases: Empty chains, single strike, extreme IV values
- Error Scenarios: API failures, invalid JSON, missing required fields
- Mocking: Mock Yahoo Finance responses with realistic test data
```

### Dependencies

**Blocked By:**

- List issues that must be completed first
- Format: `#[Issue Number] - [Task Name]`

**Blocks:**

- List issues waiting on this task
- Format: `#[Issue Number] - [Task Name]`

**Example:**

```
- Blocked by: #2 (Set Up .NET Project Infrastructure)
- Blocks: #29 (Implement Options Data Ingestion from Yahoo Finance)
```

### Resources

**Types of Resources:**

- Research documents (`.research/` folder files)
- External documentation links
- Architecture or design documents
- Reference materials or specifications

**Format:**

```
- Resource Name: [Link or path]
- Research Doc: .research/technology/filename.md
- External: https://example.com/docs
```

### Estimated Story Points

**Scale (Fibonacci):**

- **1:** Tiny, obvious (~30 min - 1 hour)
- **2:** Small, well-defined (~1-2 hours)
- **3:** Small-medium, straightforward (~3-4 hours)
- **5:** Medium task, some complexity (~half day)
- **8:** Larger task, multiple components (~full day)
- **13:** Large task, should probably split (~2+ days)

**Format:**

```
**[Number] points** — [Brief justification]

Example:
**5 points** — Multiple integration points (API, parsing, validation, storage);
error handling complexity; external dependency management
```

**Estimation Factors:**

- Complexity of the problem
- Uncertainty or research needed
- External dependencies
- Testing requirements
- Risk or potential issues

---

## Issue Consistency Observations

### Current State

**Newer Issues (#27-30):**

- ✅ Comprehensive, detailed structure
- ✅ Clear scope boundaries
- ✅ Detailed implementation guidance
- ✅ Explicit testing strategy
- ✅ Story points with justification

**Older Issues (#21-26):**

- ⚠️ Minimal structure
- ⚠️ Less detailed acceptance criteria
- ⚠️ No implementation guidance
- ⚠️ No explicit testing strategy
- ⚠️ Story points without context

### Recommendations

1. **Going Forward:** Use the Detailed Pattern for all new technical tasks
2. **Updates:** Consider updating older issues to the Detailed Pattern as work approaches
3. **Consistency:** Choose a standard pattern per issue type (Task, Bug, Feature)

---

## Quick Reference: When to Use Each Pattern

### Use Detailed Pattern For:

- Backend feature implementation
- Database schema changes
- API endpoint development
- Complex calculations or algorithms
- Cross-component features
- Work requiring design decisions
- Tasks blocking multiple other issues

### Use Simple Pattern For:

- Bug fixes (well-understood root cause)
- Documentation updates
- Minor UI tweaks
- Dependency updates
- Refactoring well-scoped code
- Tasks under 1-2 hours
- Work with no dependencies

---

## Template Markdown

Copy this template for new technical tasks:

```markdown
## Task: [Task Title]

### Description

[What needs to be done and why]

### Scope

**Included:**

- [Item 1]
- [Item 2]
- [Item 3]

**Not Included:**

- [Item 1]

### Acceptance Criteria

- [ ] Criterion 1
- [ ] Criterion 2
- [ ] Criterion 3
- [ ] Criterion 4

### Technical Details

[Technical context, architecture, constraints]

### Implementation Details

1. Step 1
2. Step 2
3. Step 3
4. Step 4

### Testing Strategy

- Unit Tests: [What]
- Integration Tests: [What]
- Edge Cases: [What]
- Error Scenarios: [What]
- Mocking: [What]

### Dependencies

- Blocked by: [Issue #X]
- Blocks: [Issue #Y]

### Resources

- [Name]: [Link/Path]

### Estimated Story Points

**[Number] points** — [Justification]
```

---

## Related Documentation

- **Project Setup:** See `.github/` folder
- **Research Documents:** See `.research/` folder
- **Development Standards:** See `.instructions.md`
- **Code Structure:** See project `README.md`

---

**Last Updated:** May 7, 2026  
**Created by:** Task Writing Agent  
**Status:** Active - Use for all new GitHub issues
