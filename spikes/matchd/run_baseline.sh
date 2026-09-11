#!/usr/bin/env bash
# Score seed.jsonl against eros Parser.ParseMovieTitle.
set -euo pipefail

ROOT="$(cd "$(dirname "$0")" && pwd)"
EROS_ROOT="${WHISPARR_EROS_ROOT:-/tmp/whisparr-eros}"
CORPUS="${1:-$ROOT/corpus/seed.jsonl}"
OUT="${2:-$ROOT/results/baseline.json}"

if [[ ! -d "$EROS_ROOT/src/NzbDrone.Core" ]]; then
  echo "eros checkout not found at $EROS_ROOT" >&2
  echo "Set WHISPARR_EROS_ROOT or: git worktree add /tmp/whisparr-eros origin/cursor/upgrade-eros-a3ae" >&2
  exit 2
fi

dotnet run \
  --project "$ROOT/harness/MatchdBaselineScorer.csproj" \
  -c Release \
  -p:WhisparrErosRoot="$EROS_ROOT" \
  -- \
  "$CORPUS" \
  "$OUT"
