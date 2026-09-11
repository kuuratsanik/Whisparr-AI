## Handoff
- **From:** cursor
- **To:** claude
- **Track:** B-Reimagine (harness) + A-Survive awareness
- **Time (UTC):** 2026-09-11 19:45

### Goal
Add a `dotnet` test harness that scores current `Parser.ParseMovieTitle` against `spikes/matchd/corpus/seed.jsonl` and writes `spikes/matchd/results/baseline.json`.

### Context
- Branch to start from: `cursor/matchd-golden-set-a3ae` (or cherry-pick corpus onto `cursor/upgrade-eros-a3ae` if you need eros parser)
- Prompt: `prompts/claude/SYSTEM.md`
- Related fixtures already on eros PR #5: `SceneMatchingFixture`

### Constraints
- Do NOT start a greenfield rewrite
- Keep harness read-only vs production DB
- Prefer NUnit + FluentAssertions consistent with repo

### Done when
- [ ] `dotnet test` target runs corpus scoring
- [ ] `results/baseline.json` committed with precision/recall summary
- [ ] Handoff to Gemini for methodology review

### Evidence
Test command + baseline metrics
