# Whisparr Greenfield Rebuild — “if we started from nothing”

**Status:** Vision / provocation (not an implementation plan)  
**Date:** 2026-09-11  
**Companion:** incremental path stays [`RUNTIME_DECISION.md`](./RUNTIME_DECISION.md) + Notion upgrade board (net8 on `eros`).  
**This doc answers:** *If Whisparr did not inherit Radarr/Sonarr/NzbDrone, what would we build?*

---

## 1. The honest diagnosis

Whisparr today is a **movie PVR wearing a scene costume**:

| Inherited assumption | Reality for adult libraries |
| --- | --- |
| One title ↔ one year ↔ one TMDB id | One studio drops **many scenes per day**; identity is studio+date+performers+title |
| Filename grammar ≈ movie scene rules | Filenames are **site dialects** (`Studio.YYYY-MM-DD.Performer.Title`, category tags, JAV codes, …) |
| Metadata = one cloud proxy (SkyHook) | Truth lives in **StashDB / TPDB / local Stash / site APIs** — often all at once |
| UI trees fork Movie vs Scene on one `Movie` row | Users think in **Performer → Studio → Scene** graphs, not “movies with a flag” |
| Value = upstream sync velocity | Value = **matching quality**; every blind Radarr/Sonarr port risks the differentiator |

The net8 spike and sync dispositions are the right **survival** path. They are not the **product** path.

A rebuild is justified only if we optimize for the differentiator: **identity-aware scene matching**, not for “being a Servarr app.”

---

## 2. Product thesis (one sentence)

> **Whisparr is a local-first scene identity engine** that watches indexers, ranks releases against a performer/studio/scene graph, and files media with explainable matches — download clients are plugins, not the product.

Everything else (calendar, queue chrome, quality profiles) is table stakes copied from good PVR UX.

---

## 3. Domain model — scene-native, not movie-shaped

```text
                    ┌──────────────┐
                    │   Studio     │◄──── aliases, networks, parent labels
                    └──────┬───────┘
                           │ produces
              ┌────────────┼────────────┐
              ▼            ▼            ▼
        ┌──────────┐ ┌──────────┐ ┌──────────┐
        │  Scene   │ │  Scene   │ │  Movie   │  (optional long-form)
        └────┬─────┘ └────┬─────┘ └────┬─────┘
             │            │            │
             └──────┬─────┴─────┬──────┘
                    ▼           ▼
              ┌──────────┐ ┌──────────┐
              │Performer │ │  Tag /   │
              │ (+alias) │ │  Genre   │
              └──────────┘ └──────────┘

Release ──ranks──► CandidateMatch ──explains──► Scene (or reject)
```

**Hard rules for a greenfield schema:**

1. **`Scene` is the primary entity.** No `ItemType` enum on a movie table.
2. **Stable external IDs are first-class columns**, not a soup of nullable `TmdbId`/`StashId`/`ForeignId` fighting for meaning. Prefer: `ids JSONB` map `{ "stashdb": "...", "tpdb": "...", "whisparr": "..." }`.
3. **Aliases are entities** (performer aka, studio rebrands, title variants) with source + confidence — not string cleanup heuristics alone.
4. **Movies are an optional module** (feature flag / separate table), not the base class scenes inherit from.
5. **Library item ≠ metadata record.** Keep “what I want / have on disk” separate from “what StashDB knows.”

---

## 4. Matching engine — the product core

Replace “giant regex → hope” with a **pipeline**:

```text
Release title
    │
    ▼
[Normalize]  strip indexer category tags, codecs, group, website junk
    │
    ▼
[Parse candidates]  multiple grammars in parallel (studio-date-performer-title,
                    JAV code, bracket-site, stashid UUID, …)
    │
    ▼
[Retrieve]  query identity index by studio±date, performer overlap, code, fuzzy title
    │
    ▼
[Rank]  scored features with weights (date exact, studio alias, performer Jaccard,
        title similarity, duration/resolution hints, site grammar prior)
    │
    ▼
[Explain]  human-readable why #1 won / why rejected — stored on History
```

**Design choices that matter:**

- **Fixture-driven grammars** as versioned packages (`grammars/blacked@3`, `grammars/jav@1`) — sites evolve; ship grammar updates without full app releases.
- **Never gate “upstream PVR parser ports” into this engine.** Servarr regexes optimize for TV/movies; they are hostile to scene dialects (as today’s sync gates already admit).
- **Matching is ranked, not boolean.** Ambiguous same-studio-same-date scenes need explainable runners-up UI (interactive import becomes the happy path for edge cases, not a failure mode).
- Optional later: **embedding assist** for title/performer text — offline ONNX model, never a cloud dependency for core matching.

---

## 5. Metadata — pluggable providers, not one SkyHook funnel

| Provider | Role |
| --- | --- |
| StashDB | Default scene/performer/studio graph |
| ThePornDB / others | Alternate / fill gaps |
| Local Stash | Authoritative for *my* library tags & scrapes |
| Site adapters | Official APIs / scrapers where legal & ToS-ok |
| Manual / CSV / stash-id paste | Escape hatch always |

Interface sketch:

```csharp
public interface IMetadataProvider
{
    Task<SceneGraphDiff> LookupSceneAsync(SceneQuery q, CancellationToken ct);
    IAsyncEnumerable<SearchHit> SearchAsync(string text, CancellationToken ct);
    ProviderCapabilities Caps { get; }
}
```

Users enable N providers with priority + merge policy (“StashDB wins IDs, local Stash wins tags”).

---

## 6. Application architecture (greenfield stack pick)

Opinionated default — optimize for one small team and long-lived NAS installs:

| Layer | Choice | Why |
| --- | --- | --- |
| Core language | **.NET 10** (minimal APIs, workers) | Team already knows it; great SQLite story; one runtime with v2 already there |
| Sync/match worker | Same process, **channel-based workers** (or Orleans-free simple queues) | Avoid distributed-systems cosplay for a home server |
| DB | **SQLite default**, Postgres optional | Keep Servarr ops familiarity; schema is new |
| API | **Versioned HTTP + OpenAPI**, first-class | UI, CLI, Home Assistant, stash plugins all equal citizens |
| Realtime | **SSE or WebSocket** narrow events | Don’t rebuild SignalR surface area on day one |
| Frontend | **React 19 + TanStack Query + Zustand** (or Solid if we want less React inertia) | Scene graph UX needs snappy cache; kill jQuery/RR5/redux-actions |
| Packaging | Official **OCI images** + optional native; **no Mono ghost paths** | Container-native from commit one |
| Extensibility | **Wasm or out-of-proc provider plugins** for indexers/download clients | ThingiProvider-in-process couples failure domains |

**Explicit non-goals for v1 greenfield:** mobile apps, multi-tenant SaaS, AI chat in the UI, rewriting download-client protocols we can wrap.

---

## 7. UX — one graph, not four cloned CRUDs

Today: parallel `Movie/`, `Scene/`, `Studio/`, `Performer/` trees with duplicated list/detail patterns.

Greenfield UI information architecture:

1. **Home / Wanted** — what to grab next (scene-native calendar by studio or performer).
2. **Identity browser** — search once across scenes/performers/studios; pivot freely.
3. **Activity** — queue + explainable match decisions.
4. **Import studio** — interactive import centered on *ranking visualization*, not a spreadsheet of rejected guesses.
5. **System** — providers, indexers, clients, grammars.

Brand/UX note: treat **Whisparr** as the hero of first paint; the product is adult-library craftsmanship, not a Radarr skin.

---

## 8. Migration story (because greenfield without a bridge is a hobby)

A rewrite that cannot import an `eros` library is dead on arrival.

1. **Read-only importer** for current Whisparr SQLite/Postgres (map `Movie+ItemType=Scene` → `Scene`, preserve stash IDs & file paths).
2. **Side-by-side run** — greenfield watches library folder; old instance remains grabber until parity.
3. **Cutover checklist** — indexers/clients reconfigured once; grammars proven on user’s last 90 days of history.

No attempt to auto-merge v2 studios DB + v3 scenes DB into one magical migrator on day one.

---

## 9. What we deliberately leave behind

- `NzbDrone.*` naming and movie-centric APIs for scenes  
- Dual-maintaining v2 feature work  
- Blind upstream cherry-picks into the parser  
- Single SkyHook as the only metadata truth  
- React 17 + jQuery + webpack folklore as a long-term FE bet  
- “Be Radarr but purple” as a product strategy  

---

## 10. Two-track strategy (how this coexists with the current roadmap)

| Track | Purpose | Horizon |
| --- | --- | --- |
| **A — Survive** | net8 on `eros`, sync dispositions, parser fixtures, Node 20 (PRs #4/#5) | Now |
| **B — Reimagine** | Greenfield identity+matching engine; prove on a spike repo | Spike → decision gate |

**Decision gate for Track B:** after a spike that (1) ingests 1k real scene filenames from fixtures, (2) beats current parser precision/recall on that set, (3) imports an `eros` library read-only — only then staff a real rewrite. Until then, Track A funds time and trust.

---

## 11. Spike backlog (if we greenlight Track B)

1. Spec the scene-native schema + ID map.  
2. Extract today’s fixtures (`SceneMatchingFixture` + failing production samples) into a **golden set**.  
3. Build `matchd` prototype (library + CLI) with normalize→parse→rank→explain.  
4. Pluggable StashDB provider only.  
5. Library importer from Whisparr v3 DB.  
6. Thin web UI: search graph + match explanation view.  
7. Go/no-go review vs continuing Track A only.

---

## 12. Bottom line

Rebuilding Whisparr “from nothing” is not “rewrite Servarr in a new framework.”  
It is **promoting matching + identity to the center of the product**, demoting upstream PVR parity to a compatibility layer for indexers and download clients.

Track A keeps the lights on.  
Track B is how Whisparr stops being a fork and becomes a category.
