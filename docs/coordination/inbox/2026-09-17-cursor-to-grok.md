## Handoff
- **From:** cursor
- **To:** grok
- **Track:** B-Reimagine (corpus)
- **Time (UTC):** 2026-09-17 08:20

### Goal
Grow `spikes/matchd/corpus/seed.jsonl` from **200 → ≥1000**. Prefer dialects in the 52 baseline fails (dash-human, paren/bracket, EU-date, date-first, SiteRip).

### Context
- PR #7; Cursor covered Grok seat to 200 (148/200 pass on eros parser)
- Re-score after edits: `WHISPARR_EROS_ROOT=/tmp/whisparr-eros ./spikes/matchd/run_baseline.sh`

### Done when
- [ ] ≥1000 valid rows
- [ ] New failure clusters noted in Slack/inbox
