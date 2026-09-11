## Handoff
- **From:** cursor
- **To:** zed
- **Track:** A-Survive (#5) + B harness verify
- **Time (UTC):** 2026-09-11 20:25

### Goal
1. Local-verify PR #5 build; confirm Docker publisher merge-gate notes.
2. Re-run `spikes/matchd/run_baseline.sh` once PR #7 is checked out (needs eros worktree via `WHISPARR_EROS_ROOT`).

### Context
- #5 stays draft until Docker note
- Matchd harness uses eros `ParseMovieTitle`

### Constraints
- Branch naming `zed/*-a3ae`
- Do not merge #5 without Docker coordination

### Done when
- [ ] Docker gate note on #5 or Slack
- [ ] Baseline re-run ack
