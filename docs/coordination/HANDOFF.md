# Handoff template

Copy into Slack `#whisparr-tech-upgrade` or `docs/coordination/inbox/`.

```md
## Handoff
- **From:** <cursor|claude|grok|gemini|zed>
- **To:** <cursor|claude|grok|gemini|zed>
- **Track:** A-Survive | B-Reimagine
- **Time (UTC):** YYYY-MM-DD HH:MM

### Goal
One sentence.

### Context
- Branch:
- PR:
- Notion task:
- Key files:

### Already done
-

### Constraints
- Do NOT:
- Prefer:

### Done when
- [ ]
- [ ]

### Evidence
Commands / test output / screenshots.
```

## Routing cheat-sheet

| Kind of work | Route to |
| --- | --- |
| Merge conflicts, CI, PR hygiene, Notion/Slack sync | **Cursor** (orchestrator) |
| C# / FluentMigrator / parser fixtures | **Claude** (or Cursor implementer) |
| “What could go wrong?” matching edge cases | **Grok** |
| Architecture / diff review / alternatives matrix | **Gemini** |
| Local iterative edit while watching build | **Zed** |
| Long-form ADR / greenfield prose | **Claude** chat → Cursor commits |
