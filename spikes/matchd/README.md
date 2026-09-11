# matchd golden-set spike (Track B gate)

**Purpose:** Prove a scene-native matcher can beat the current `eros` parser on a fixed corpus before staffing a rewrite.

**Gate (from `docs/GREENFIELD_REBUILD.md`):**
1. Ingest ≥1k filenames (start with this seed; expand via Grok seat).
2. Precision/recall ≥ current parser on the same set.
3. Read-only import path for an `eros` library (later).

## Layout
- `corpus/seed.jsonl` — labeled release titles (one JSON object per line)
- `results/` — score outputs (gitignored except `.gitkeep`)
- `score_baseline.md` — how to measure current parser vs candidate

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

## Seats
| Seat | Job |
| --- | --- |
| Grok | Grow corpus to 200+ adversarial / dialect cases |
| Claude | Wire a `dotnet` harness that scores current `Parser.ParseMovieTitle` against `seed.jsonl` |
| Gemini | Review scoring methodology + false-positive risk |
| Cursor | Keep gate + PR hygiene; no rewrite until gate passes |
| Zed | Local iteration on harness once Claude lands it |

## Non-goals
- Replacing Whisparr runtime in this spike
- Shipping net10 on eros
- Mixing these files into PR #5 merge criteria
