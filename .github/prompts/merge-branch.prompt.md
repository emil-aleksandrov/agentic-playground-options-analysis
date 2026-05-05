---
description: "Merge a branch into main and resolve conflicts with honest feedback on risks"
name: "Merge Branch"
argument-hint: "source-branch [target-branch] [strategy]"
agent: "agent"
---

# Merge Branch Task

Merge a source branch into the target branch (default: main) with careful conflict resolution and risk assessment.

## Your Task

1. **Check status**: Verify the current branch state and confirm uncommitted changes status
2. **Perform merge**: Execute the merge operation (specify strategy if preferred: `recursive`, `resolve`, `ours`, `theirs`)
3. **Resolve conflicts**: If conflicts arise:
   - List all conflicted files
   - For each conflict, explain what changed in source vs target
   - Suggest the correct resolution based on context
   - Flag any risky resolutions (data loss, logic changes, breaking changes)
4. **Validate**: Run any available tests to ensure the merge didn't break functionality
5. **Report**: Provide complete, honest feedback:
   - What merged successfully
   - What conflicts were found and how they were resolved
   - Any warnings about the merge (e.g., breaking API changes, database impact)
   - Next steps (e.g., if CI needs to run, if other branches depend on this)

## Key Principles

- **Honest feedback**: Don't hide issues—if the merge causes problems, flag them clearly
- **Proactive warnings**: Point out ripple effects before they become blockers
- **Context matters**: Explain _why_ each resolution choice was made, not just _what_ was changed

## Parameters

**Source branch**: The branch being merged (provide exact name)
**Target branch**: Where to merge into (default: `main`)
**Merge strategy**: Optional merge strategy if you have a preference
