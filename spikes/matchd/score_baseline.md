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

## Latest seed snapshot (1155 rows)
Recorded in `results/baseline.json` (`corpusSha256` `592d2126…`): **623/1155 pass** (~54%).
Composition: 1045 scene / 54 movie / 56 reject.
Sources: adversarial 822 · production-sanitized 155 · issue-1218 134 · issue-1257 44.
Scene kind P=1.0 / R≈0.997; rejectOk=1.0; prefixStrip=1.0; studio≈0.491; date≈0.894; title≈0.668.
All **155** `production-sanitized` rows pass. Fail surface unchanged (**532** adversarial scene rows):
- Dash-human (~143 fails)
- Date-first (~111)
- SiteRip mash (~111)
- EU spaced date (~111)
- Paren/bracket studio (~56)

Prior snapshots: 1000-row `3564d5e5…` 468/1000 (~47%); 500-row ~258/500 (~52%).

Methodology review: `GEMINI_RUBRIC_REVIEW.md` — **ship gate design**; do not staff MatchD rewrite until a challenger beats this SHA.

## Target for gate
Beat the recorded baseline on the same `corpusSha256`. Do not compare across different corpus hashes.
