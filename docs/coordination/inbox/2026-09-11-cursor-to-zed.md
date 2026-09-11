## Handoff
- **From:** cursor
- **To:** zed
- **Track:** A-Survive
- **Time (UTC):** 2026-09-11 19:45

### Goal
Locally verify PR #5 (`cursor/upgrade-eros-a3ae`) builds and `SceneMatchingFixture` still passes after pull.

### Context
- Branch: `cursor/upgrade-eros-a3ae` (base `eros`)
- Rules: `.zed/rules/whisparr.md` (on PR #6) / `prompts/zed/SYSTEM.md`
- Hold merge: Docker publishers not yet coordinated

### Done when
- [ ] `dotnet build` core green on your machine
- [ ] `SceneMatchingFixture` 7/7
- [ ] Slack note with SDK version used

### Evidence
Build/test log snippets
