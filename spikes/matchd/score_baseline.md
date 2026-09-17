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

## Latest seed snapshot (1000 rows)
Recorded in `results/baseline.json` (`corpusSha256` `3564d5e5…`): **468/1000 pass** (~47%).
Composition: 910 scene / 44 movie / 46 reject.
Scene kind P=1.0 / R≈0.997; rejectOk=1.0; prefixStrip=1.0; studio≈0.415; date≈0.878; title≈0.617.
Corpus is intentionally fail-cluster-heavy so MatchD has a clear opportunity surface (all 532 fails are adversarial scene rows):
- Dash-human (~143 fails)
- Date-first (~111)
- SiteRip mash (~111)
- EU spaced date (~111)
- Paren/bracket studio (~56)

Prior 500-row snapshot was ~258/500 (~52%) — pass rate dropped as fail clusters were padded to gate size.

## Target for gate
Beat the recorded baseline on the same `corpusSha256`. Do not compare across different corpus hashes.
