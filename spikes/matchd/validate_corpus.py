#!/usr/bin/env python3
"""Validate matchd corpus JSONL schema (no parser dependency)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent
CORPUS = ROOT / "corpus" / "seed.jsonl"
REQUIRED = {"id", "releaseTitle", "expect", "notes", "source"}
KINDS = {"scene", "movie", "reject"}


def main() -> int:
    rows = []
    with CORPUS.open() as fh:
        for i, line in enumerate(fh, 1):
            line = line.strip()
            if not line:
                continue
            try:
                obj = json.loads(line)
            except json.JSONDecodeError as exc:
                print(f"line {i}: invalid JSON: {exc}", file=sys.stderr)
                return 1
            missing = REQUIRED - set(obj)
            if missing:
                print(f"line {i}: missing keys {missing}", file=sys.stderr)
                return 1
            kind = obj["expect"].get("kind")
            if kind not in KINDS:
                print(f"line {i}: bad kind {kind!r}", file=sys.stderr)
                return 1
            rows.append(obj)

    ids = [r["id"] for r in rows]
    if len(ids) != len(set(ids)):
        print("duplicate ids", file=sys.stderr)
        return 1

    by_kind = {}
    for r in rows:
        by_kind[r["expect"]["kind"]] = by_kind.get(r["expect"]["kind"], 0) + 1

    print(f"ok: {len(rows)} rows {by_kind}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
