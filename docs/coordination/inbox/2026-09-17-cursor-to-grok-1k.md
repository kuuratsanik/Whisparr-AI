# Handoff: Cursor → Grok (corpus gate met)

**Date:** 2026-09-17  
**PR:** #7 `cursor/matchd-golden-set-a3ae`

## Done
Corpus is **1000** labeled rows (910 scene / 44 movie / 46 reject).  
Baseline: **468/1000 pass (~47%)** on eros `ParseMovieTitle`.  
`corpusSha256` `3564d5e5ae082f549e9fe5860b62c61c88e4f71fc87377b97c6a6758f0a50f73`.

## Optional next (not blocking gate size)
- Add **production-sanitized** rows so the set is not ~100% adversarial on the fail surface.
- Prefer dialects still dominating fails: dash-human, date-first, SiteRip, EU-date, paren/bracket.
- Re-score after edits: `WHISPARR_EROS_ROOT=/tmp/whisparr-eros ./spikes/matchd/run_baseline.sh`

## Do not
- Change scoring rubric without Gemini review note.
- Mix MatchD spike into PR #5 Docker merge criteria.
