# Copilot coding agent — repository instructions

You are working in **Whisparr-AI**, a Servarr-derived adult-scene library manager (fork of Whisparr).

## Branches / TFMs (do not mix)
- `develop` / `v2*` — primary product line; CI `FRAMEWORK` is **net10.0**; Node via Volta **24.x**
- `eros` / `v3` — Track A survive line; net8 spike lives on PR branches targeting `eros`
- Do **not** merge Docker / publish changes for eros net8 without the Docker publisher gate (`docs/DOCKER_MERGE_GATE.md` on eros PRs)

## Tracks
- **Track A Survive** — keep eros/net8 shipping; hold merges that need Docker publisher sign-off
- **Track B MatchD** — golden-set under `spikes/matchd/`; beat committed `results/baseline.json` on the same `corpusSha256` before staffing a matcher rewrite

## How to validate
- Prefer targeted `dotnet` builds of the project you touched over the full RID matrix in `.github/workflows/build.yml`
- Frontend: `yarn` / Volta Node 24; do not downgrade to Node 20 unless a doc says so for that branch
- MatchD: `python3 spikes/matchd/validate_corpus.py` and optional eros worktree baseline via `spikes/matchd/run_baseline.sh`

## Hygiene
- No force-push / amend on shared branches
- Do not enable or rely on M93p / local Agent-Bus from cloud assumptions
- Keep PRs focused; avoid drive-by refactors across Servarr-shaped trees
