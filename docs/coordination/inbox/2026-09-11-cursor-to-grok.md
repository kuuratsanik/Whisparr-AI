## Handoff
- **From:** cursor
- **To:** grok
- **Track:** B-Reimagine (corpus)
- **Time (UTC):** 2026-09-11 20:25

### Goal
Expand `spikes/matchd/corpus/seed.jsonl` from **40 → ≥200** labeled JSONL rows. Prefer dialects that already fail baseline (`failedIds`: gs-010, gs-027, gs-028, gs-032, gs-039).

### Context
- PR #7; validator: `python3 spikes/matchd/validate_corpus.py`
- After edits re-run: `WHISPARR_EROS_ROOT=/tmp/whisparr-eros ./spikes/matchd/run_baseline.sh`

### Constraints
- Keep schema keys: id, releaseTitle, expect, notes, source
- No rewrite / MatchD implementation

### Done when
- [ ] ≥200 valid rows
- [ ] Note new failure clusters in handoff/Slack
