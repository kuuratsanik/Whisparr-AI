# Gemini-seat review: MatchD baseline rubric + FP risk (1155 rows)

**Seat:** Gemini (covered by Cursor, 2026-09-17)  
**Artifacts:** `corpus/seed.jsonl`, `results/baseline.json`, `score_baseline.md`, harness `Program.cs`  
**Current baseline:** **623/1155 pass (~54%)**, `corpusSha256` `592d2126605d73e89ea67cae79dc2be6e4a150270e3d998457baf3065301c746`  
**Verdict:** **Ship the gate design** with caveats below. Do **not** start a MatchD rewrite until a challenger beats the recorded `corpusSha256` under this rubric.

## What the harness measures
- API under test: eros `Parser.ParseMovieTitle` only (not develop TV parser; not Stash/TPDB).
- Scene pass = kind + studio (exact CI) + date (exact) + title overlap ≥ 0.6 + all performer tokens present.
- Movie pass = non-scene + title exact/overlap ≥ 0.6.
- Reject pass = null parse **or** `!IsScene` (must not invent scene studio/date).
- Prefix strip tracked separately on `GAY|XXX|…` prefixed titles.

## Methodology — hold risks (fix, not blockers)
1. **Adversarial overweight:** 822/1155 rows are `adversarial`, and **all 532 fails** remain adversarial scene dialects. Pass rate (~54% after adding 155 all-pass `production-sanitized` rows) is still an *opportunity surface*, not a claim about production libraries. Keep growing `production-sanitized` / issue sources toward ≥30%.
2. **Title overlap ≥ 0.6 is lenient:** token overlap on haystack (`ReleaseTokens` + titles) can mark a pass when studio/date already failed? No — all scene fields must pass. Still, overlap can forgive wrong title boundaries if tokens leak from performers. Acceptable for go/no-go; tighten to exact/normalized title if MatchD claims “beats parser on titles.”
3. **Studio exact-match is brittle:** `EvilAngel` vs `Evil Angel` would fail even if human-ok. Fine for baseline fairness (same rule for challenger).
4. **Reject definition is soft:** a wrong movie-shaped parse of junk still “rejects” as non-scene. That under-measures false identity invention for non-scene files that parse as movies. Optional follow-up metric: `rejectHardOk` = parse null **or** empty title.
5. **No metadata linkage:** scoring does not hit StashDB/TPDB. Gate proves *filename dialect parsing*, not graph identity. Aligns with Track B spike scope; do not sell it as end-to-end MatchD.

## False-positive risk
- **Scene FP (kind):** Scene kind precision was **1.0** at 1k — parser rarely invents scenes on this set. Low FP concern for kind.
- **Field FP on adversarial:** many “fails” are correct parser limitations (dash-human, EU date, SiteRip mash). Treating them as MatchD wins is fine; treating current passes as production-proof is not.
- **Rubric FP (scoring marks pass wrongly):** highest risk is performer/title token bleed via haystack overlap. Spot-check: prefer cases where `primaryTitle` tokens are disjoint from performer names (already mostly true). Add adversarial title==performer collisions later if MatchD games overlap.
- **Reject FP:** junk that parses as movie still counts rejectOk — see soft reject note.

## Ship / hold
| Decision | Call |
| --- | --- |
| Keep rubric + committed baseline as Track B gate | **Ship** |
| Staff MatchD implementation now | **Hold** until challenger > baseline on same SHA |
| Compare scores across corpus hashes | **Hold** (forbidden) |
| Use 1k pass% as production quality metric | **Hold** until production-sanitized / issue rows dominate |

## Recommended next measurements
1. Keep `production-sanitized` ≥20% of corpus; re-record baseline SHA.
2. Add a `rejectHardOk` side metric (null-or-empty-title) without changing pass/fail.
3. Freeze a **challenge slice** (the 532 fail IDs at SHA `3564d5e5…`) so MatchD can report lift on hard dialects separately from overall %.
