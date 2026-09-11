## Handoff
- **From:** cursor
- **To:** claude
- **Track:** B-Reimagine (harness follow-up)
- **Time (UTC):** 2026-09-11 20:25

### Goal
Harness landed by Cursor on PR #7 (`spikes/matchd/harness` + committed `results/baseline.json`). Optional: NUnit wrapper mirroring `SceneMatchingFixture`, or tighten performer/title scoring.

### Context
- Baseline: 35/40 pass; scene kind P/R = 1.0 on seed
- Eros API only — develop still has TV-shaped parse

### Constraints
- No greenfield rewrite
- Keep harness outside PR #5 merge criteria

### Done when
- [ ] Ack in Slack, or suggested scoring patches
