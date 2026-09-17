# matchd golden-set spike (Track B gate)

**Purpose:** Prove a scene-native matcher can beat the current `eros` parser on a fixed corpus before staffing a rewrite.

**Gate (from PR #5 `docs/GREENFIELD_REBUILD.md` on eros):**
1. Ingest ≥1k filenames (seed is **200** labeled rows; next: 200 → 1k).
2. Precision/recall ≥ current parser on the same set.
3. Read-only import path for an `eros` library (later).

## Layout
- `corpus/seed.jsonl` — labeled release titles (one JSON object per line)
- `harness/` — net8 console scorer referencing eros `NzbDrone.Core` (`Whisparr.Core.csproj`)
- `run_baseline.sh` — builds harness + writes `results/baseline.json`
- `validate_corpus.py` — schema-only check (no parser)
- `results/baseline.json` — committed snapshot of current parser score
- `score_baseline.md` — rubric

## Labels
Each row:
```json
{
  "id": "gs-001",
  "releaseTitle": "...",
  "expect": {
    "kind": "scene|movie|reject",
    "studio": "...",
    "date": "YYYY-MM-DD",
    "primaryTitle": "...",
    "performers": ["..."]
  },
  "notes": "why this case matters",
  "source": "issue-1218|issue-1257|adversarial|production-sanitized"
}
```

## Run baseline (needs eros checkout)

```bash
git fetch origin cursor/upgrade-eros-a3ae
git worktree add /tmp/whisparr-eros origin/cursor/upgrade-eros-a3ae

python3 spikes/matchd/validate_corpus.py
WHISPARR_EROS_ROOT=/tmp/whisparr-eros ./spikes/matchd/run_baseline.sh
```

Uses `Parser.ParseMovieTitle` from the eros tree (not develop's TV-shaped parser).

## Seats
| Seat | Job |
| --- | --- |
| Grok | Grow corpus 200 → 1k (focus fail clusters in `baseline.json`) |
| Claude | Optional NUnit wrap / scoring tweaks (harness landed) |
| Gemini | Review scoring methodology + FP risk on 200-row `baseline.json` |
| Cursor | Covered Grok seat to 200; keep gate + PR hygiene |
| Zed | Local re-run of harness + #5 Docker gate notes |

## Non-goals
- Replacing Whisparr runtime in this spike
- Shipping net10 on eros
- Mixing these files into PR #5 merge criteria
