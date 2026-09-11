# Scoring the baseline parser

Until a `dotnet` harness lands (Claude seat), use this checklist.

## Manual / interim
For each `corpus/seed.jsonl` row, call current `Parser.ParseMovieTitle(releaseTitle)` on `eros` (or PR #5 branch) and score:

| Field | Scene rows | Movie rows | Reject rows |
| --- | --- | --- | --- |
| `kind` | `IsScene==true` | `IsScene==false` + title/year present | parse null / no studio+date invention |
| `studio` | exact or alias-equal | n/a | must be empty |
| `date` | exact `YYYY-MM-DD` | n/a | empty |
| `primaryTitle` | token overlap ≥ 0.6 vs expected | exact preferred | empty |

## Metrics to report
- Precision / recall for `kind=scene`
- Prefix-strip accuracy on `GAY|XXX|ADULT|LESBIAN|TRANS`
- False scene rate on `reject` rows

## Target for gate
Beat the recorded baseline in `results/baseline.json` (to be generated) on the same corpus hash.
