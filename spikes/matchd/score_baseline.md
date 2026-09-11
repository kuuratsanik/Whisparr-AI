# Scoring the baseline parser

Harness: `spikes/matchd/harness` via `./run_baseline.sh` (eros `Parser.ParseMovieTitle`).

## Field rubric
| Field | Scene rows | Movie rows | Reject rows |
| --- | --- | --- | --- |
| `kind` | `IsScene==true` | not scene + title/year present | `null` or `!IsScene` |
| `studio` | exact (case-insensitive) | n/a | must not invent scene studio |
| `date` | exact `YYYY-MM-DD` | n/a | empty / non-scene |
| `primaryTitle` | token overlap ≥ 0.6 vs haystack (`ReleaseTokens` + titles) | `PrimaryMovieTitle` exact or overlap ≥ 0.6 | n/a |
| `performers` | all performer tokens appear in haystack / `FirstPerformer` | n/a | n/a |

## Metrics in `results/baseline.json`
- Scene kind precision / recall
- Studio / date / title accuracy on applicable rows
- Prefix-strip accuracy on category-prefixed rows (`GAY|XXX|PORN|ADULT|…`)
- Reject ok-rate
- `failedIds` for red-team follow-up

## Latest seed snapshot (40 rows)
Recorded in `results/baseline.json`: **35/40 pass**. Hard fails worth expanding around:
- `gs-010` dash-separated human form
- `gs-027` parenthetical studio
- `gs-028` EU spaced date (mis-parsed)
- `gs-032` date-first ordering
- `gs-039` SiteRip mash

## Target for gate
Beat the recorded baseline on the same `corpusSha256`. Do not compare across different corpus hashes.
