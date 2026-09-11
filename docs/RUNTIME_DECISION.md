# Runtime decision: net8-now vs net10-later (eros / v3)

**Decision date:** 2026-09-11  
**Status:** Decided  
**Choice:** **net8-now** for Whisparr v3 (`eros`)

## Context

- Whisparr **v3 (`eros`)** still targets **.NET 6** (EOL) while Radarr `develop` already ships **.NET 8**.
- Whisparr **v2 (`develop`)** already absorbed Sonarr’s **.NET 10** bump via upstream sync (Batch 39 / May 2026). That does **not** change the v3 path: v3 backend ownership follows **Radarr**, not Sonarr.
- Notion upgrade plan default (2026-09-09): absorb Radarr’s net8 now; jump to net10 only when Sonarr frontend sync forces it.
- Alternate draft ([PR #3](https://github.com/kuuratsanik/Whisparr-AI/pull/3) `docs/UPGRADE_PLAN.md`) proposed targeting **.NET 10 LTS** directly on the current line. That proposal is **superseded for `eros`** by this decision. Keep PR #3’s security-remediation phasing notes as optional hygiene on v2 only.

## Decision

1. **MVP-C / WS2 on `eros`:** bump TFMs and CI from `net6.0` → **`net8.0`**, mirroring Radarr’s net8 change set (drop `linux-x86`, refresh package refs / Azure `dotnetVersion`).
2. **Do not** jump `eros` straight to `net10.0` in this spike.
3. Revisit net10 on `eros` only when Sonarr-owned frontend sync makes dual runtimes untenable.

## Rationale

- Smaller OS / packaging break surface than a net6→net10 jump.
- Matches the Radarr backend lineage Whisparr v3 already follows.
- Unblocks EOL runtime risk without waiting on a Sonarr FE-forced mega-bump.
- v2 already being on net10 proves Sonarr-lineage bumps are viable later; it is not a reason to skip the Radarr-aligned intermediate on v3.

## Follow-ups

- Hold merge of the net8 spike until **MVP-B** (Sep 2026 conflicted sync ports) high-water is sane.
- Coordinate Docker image rebuilds (hotio / linuxserver) before shipping net8 packages.
- Update Notion task **Decision — net8-now vs net10-later** to Done with a link to this doc.
