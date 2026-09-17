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

## Latest seed snapshot (200 rows)
Recorded in `results/baseline.json`: **148/200 pass** (scene kind P/R = 1.0; rejectOk = 1.0).
Dominant fail clusters (MatchD opportunity surface):
- Dash-separated human form (`gs-010` family)
- Paren / bracket studio (`gs-027` / `gs-009` family)
- EU spaced date mash (`gs-028` family)
- Date-first ordering (`gs-032` family)
- SiteRip mash (`gs-039` family)

## Target for gate
Beat the recorded baseline on the same `corpusSha256`. Do not compare across different corpus hashes.
