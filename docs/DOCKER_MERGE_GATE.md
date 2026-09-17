# Docker merge gate (PR #5 / eros net8)

**Status:** Hold merge of `cursor/upgrade-eros-a3ae` into `eros` until publisher sign-off.

## Why
net8 TFM/SDK bump changes what third-party images must package. This repo does **not** ship an official Dockerfile — consumers pull **hotio / linuxserver** (and forks). Merging Core green without their rebuild ships broken tags.

## Facts from this branch (cloud-verified)

| Item | Value on `cursor/upgrade-eros-a3ae` |
| --- | --- |
| Azure `dotnetVersion` | `8.0.405` (`azure-pipelines.yml`) |
| TFMs | `net8.0` |
| `RuntimeIdentifiers` | `win-x64;win-x86;osx-x64;osx-arm64;linux-x64;linux-musl-x64;linux-arm;linux-musl-arm;linux-arm64;linux-musl-arm64` |
| Dropped vs historical net6 line | **`linux-x86` removed** (still have `win-x86`) |
| In-repo OCI Dockerfile | **None** — images are external publishers |
| CI Docker usage | Test containers only (`ghcr.io/servarr/testimages:alpine`, postgres sidecars) — not product images |

## Checklist before merge

### Cloud / docs (done or doable here)
- [x] Confirm spike targets net8 SDK `8.0.405` (not leftover net6 in pipeline vars)
- [x] Document dropped RID: `linux-x86` no longer in `Directory.Build.props` RIDs
- [x] Name external publishers to ping: **hotio**, **linuxserver** (see `RUNTIME_DECISION.md`)

### Human / local only (Cursor Cloud cannot do these)
- [ ] Publisher rebuilds `linux-x64` (and musl/arm tags they support) against net8 artifacts
- [ ] Smoke: container starts, UI loads, parse `Studio.2025-08-10.Performer.Name.Scene.Title.mp4`
- [ ] Release notes mention dropped `linux-x86` runtime packs
- [ ] Named sign-off comment on [PR #5](https://github.com/kuuratsanik/Whisparr-AI/pull/5)

## Suggested ping text
> Whisparr `eros` net8 spike is green in-repo (PR #5). Please rebuild images on SDK 8.0.x / `net8.0` publish outputs. `linux-x86` RID is gone. Hold merging to `eros` until your tag smokes.

## Out of scope
- Track B matchd golden-set (PR #7)
- net10 on eros
- Enabling M93p Agent-Bus remote exec from cloud

## Contact
Post publisher name + smoke result in `#whisparr-tech-upgrade` before flipping #5 to ready.
