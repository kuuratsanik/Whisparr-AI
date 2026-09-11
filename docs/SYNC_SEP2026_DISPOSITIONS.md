# Sep 2026 Upstream Sync Dispositions

Dispositions for conflicted and gated commits from the Sep 2026 upstream sync batch on Whisparr issues [#1238](https://github.com/Whisparr/Whisparr/issues/1238) (Radarr) and [#1240](https://github.com/Whisparr/Whisparr/issues/1240) (Sonarr).

Branch context: `cursor/upgrade-eros-a3ae` (eros-based .NET 8 spike). These entries record sync triage only; they do not imply the commits were applied on this branch.

## Radarr (`#1238`) — conflicted

| Commit | Subject | Disposition | Why |
| --- | --- | --- | --- |
| `04e27cb26` | Bump Sentry to 5.16.3 | `deferred-manual-port` | Did not apply cleanly on `eros-develop`. Sentry package/API surface has drifted vs eros; needs a deliberate version bump and compile/runtime check rather than a blind cherry-pick. |
| `48a61bf63` | Bump to 6.4.4 | `deferred-manual-port` | Version/chore bump conflicted with Whisparr eros versioning and related metadata; port manually when preparing the next eros release line. |
| `02834499f` | Remove unused package references | `deferred-manual-port` | Package graph on eros differs from Radarr develop; unused-ref removals must be re-validated against Whisparr csproj files after the net8 bump lands. |
| `36d849295` | Bump frontend dependencies | `deferred-manual-port` | Frontend lockfile / dependency tree diverged; yarn bump needs a dedicated frontend pass, not a conflicted cherry-pick. |

## Radarr (`#1238`) — gated

| Commit | Subject | Disposition | Gate |
| --- | --- | --- | --- |
| `255f38dce` | Fixed: Refactor showing grabbed/blocklisted releases in Interactive Search | `disposed-gate` | `parser` |
| `35d35321f` | Multiple Translations updated by Weblate | `disposed-gate` | `weblate` |
| `516ca5979` | Restore auto width behavior from previous FontAwesome versions | `disposed-gate` | `radarr-frontend` |

## Sonarr (`#1240`) — conflicted

| Commit | Subject | Disposition | Why |
| --- | --- | --- | --- |
| `3d87d4cb1` | Fixed: BroadcasTheNet searches for three digit episode numbers | `deferred-manual-port` | Indexer/search logic conflicted with Whisparr scene numbering; needs a Whisparr-specific port and tests. |
| `1b5aa5551` | Fixed: Improve logging for 'Allowed Hosts' and 'Trusted Networks' | `deferred-manual-port` | Security/host-list logging changes did not apply cleanly; re-port against current eros HTTP host validation code. |
| `5bebd3810` | Don't show Open Browser setting on unsupported setups | `deferred-manual-port` | Frontend setting visibility conflicted with Whisparr UI tree (Sonarr-derived but drifted); manual FE port required. |
| `9614f74c5` | Fixed: IPv6 Addresses for Download Clients | `deferred-manual-port` | Download-client address handling conflicted; needs manual merge with Whisparr client implementations. |
| `4ae579ef5` | Remove unused package references | `deferred-manual-port` | Same class of change as Radarr unused-ref cleanup; re-evaluate after net8 package alignment. |
| `2ae9bc094` | Clean up pending changes and fix OAuth on an existing import list | `deferred-manual-port` | Import-list OAuth/FE state conflicted with Whisparr list providers; manual port with UI verification. |
| `7c4c81a41` | Fixed: Series mapping by relative base folder name during Manual Import | `deferred-manual-port` | Manual-import path mapping is series-oriented upstream; Whisparr scene/movie folder semantics need a tailored port. |
| `5352e16d9` | Fixed: Pagination for Trakt import lists | `deferred-manual-port` | Trakt pagination changes conflicted with Whisparr import-list adapters; port with API smoke checks. |

## Sonarr (`#1240`) — gated

| Commit | Subject | Disposition | Gate |
| --- | --- | --- | --- |
| `2eca8c9c6` | Multiple Translations updated by Weblate | `disposed-gate` | `weblate` |
| `6421bb621` | New: Parse Web release with EAC3 Audio as WEB-DL | `disposed-gate` | `parser` |

## Notes

- `deferred-manual-port`: conflicted upstream commit; do not cherry-pick as-is.
- `disposed-gate`: held by a standing sync gate; not a blind pick. Revisit only under the gate’s process (`parser` → failing Whisparr parser test first; `weblate` → Weblate only; `radarr-frontend` → do not take Radarr FE while Sonarr remains the FE upstream).
