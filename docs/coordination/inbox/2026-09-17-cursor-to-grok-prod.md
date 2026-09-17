# Handoff: Cursor → Grok (production-sanitized started)

**Date:** 2026-09-17  
**PR:** #7

## Done
- Gate size met earlier at 1000; now **1155** rows with **155** `production-sanitized` (all pass on eros baseline).
- Baseline **623/1155 (~54%)**; fail count still **532** (adversarial only).

## Optional next
Grow `production-sanitized` / real issue-derived rows toward ≥30% of corpus. Prefer dialects that still fail (dash-human, EU-date, date-first, SiteRip, paren) but with indexer-realistic sanitized names — not more copies of already-passing dotted forms only.

Re-score: `WHISPARR_EROS_ROOT=/tmp/whisparr-eros ./spikes/matchd/run_baseline.sh`
