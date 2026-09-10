# Whisparr Upgrade & Remediation Plan

Status: Proposed · Owner: TBD · Target codebase: `kuuratsanik/Whisparr-AI` (`develop`, Whisparr v2, Sonarr-based)

This plan consolidates every upgrade and fix identified in the full repository audit: End-of-Life runtime, End-of-Life Node pin, known-vulnerable .NET and npm transitive packages, restoring CI quality gates, and modernizing major frameworks. It is organized into independently shippable phases ordered by risk and payoff, so each phase can merge on its own.

The audit that produced this plan found the project **builds clean, lint is clean, and ~99.8% of unit tests pass** (the only failures are environment-only tests that call the live `api.whisparr.com` metadata service). So the codebase is healthy; the work below is about currency and security, not defect-fixing.

---

## Baseline snapshot (as audited)

| Area | Current | Problem |
| --- | --- | --- |
| Runtime | .NET **6.0** (`net6.0`, 25 projects; `net6.0-windows`, 1) | .NET 6 is **out of support** (EOL 2024-11-12); no security patches |
| SDK pin (CI) | `dotnetVersion: '6.0.427'` in `azure-pipelines.yml`; no `global.json` | Unpinned locally; EOL SDK |
| Node pin | `volta.node = 16.17.0`, `volta.yarn = 1.22.19` | Node 16 is EOL (2023-09); builds actually run on Node 22 |
| .NET deps | 9 vulnerable transitive packages (6 High, 3 Moderate) | See Phase 1 table |
| npm deps | **23 advisories** (10 High / 12 Moderate / 1 Low) across 206 pkgs | See Phase 2 table |
| Frontend libs | React **17**, react-router 5, webpack 5, Babel 7.22 | Several majors behind |
| CI gates | Lint runs; analyzers Linux-only | Fine, but no dependency scanning in-repo |

Target runtime decision: **.NET 10 (LTS)**. As of this writing .NET 8 LTS support ends 2026-11-10 and .NET 9 (STS) is already EOL, so .NET 8 is not a durable target. The plan targets **net10.0** with an optional **net8.0 intermediate checkpoint** for teams that prefer a smaller first hop.

---

## Phase 0 — Guardrails & baseline (prerequisite, low risk)

Goal: make the upgrade measurable and reversible before changing anything.

1. **Pin the SDK** with a `global.json` at the repo root so local and CI use one SDK:
   ```json
   { "sdk": { "version": "6.0.428", "rollForward": "latestFeature" } }
   ```
   (Bumped to the target band in Phase 3.)
2. **Restore full test + lint as merge gates.** Ensure `dotnet test src/Whisparr.sln` (unit projects) and `yarn lint` / `yarn stylelint-linux` run on every PR. Exclude the network-bound `SkyHookProxy*Fixture` tests from PR runs via a category filter, or provide network egress to `api.whisparr.com` in CI.
3. **Add dependency scanning to the repo** (independent of Snyk/Semgrep PR checks, which only scan the diff):
   - .NET: `dotnet list package --vulnerable --include-transitive` as a scheduled CI job.
   - npm: `yarn npm audit` (or `yarn audit`) as a scheduled CI job.
   - Optionally enable Dependabot/Renovate for `nuget` and `npm` ecosystems.
4. **Capture a green baseline**: record current test counts (Common 467/467, Core 3356/3363 non-network) and bundle size so regressions are visible.

Exit criteria: `global.json` merged; CI runs tests + both linters + vulnerability scans; baseline metrics recorded.

---

## Phase 1 — .NET security remediation without a runtime change (low/medium risk)

Goal: clear the 9 vulnerable .NET transitive packages while staying on `net6.0`, so security is decoupled from the larger runtime migration.

| Package | Resolved | Severity | Advisory | Fix |
| --- | --- | --- | --- | --- |
| `MailKit` | 3.6.0 | Moderate | GHSA-9j88-vvj5-vhgr | Bump direct ref → `4.8.0`+ |
| `MimeKit` | 3.6.0 | High + Moderate | GHSA-gmc6-fwg3-75m5, GHSA-g7hc-96xr-gvvx | Pulled by MailKit; fixed by MailKit bump |
| `Microsoft.Data.SqlClient` | 2.1.2 | High | GHSA-98g6-xh36-x2p7 | Add explicit ref → `5.1.6`+ (transitive via FluentMigrator) |
| `Microsoft.IdentityModel.JsonWebTokens` | 6.8.0 | Moderate | GHSA-59j7-ghrg-fj52 | Add explicit ref → `6.35.0`+ |
| `System.IdentityModel.Tokens.Jwt` | 6.8.0 | Moderate | GHSA-59j7-ghrg-fj52 | Add explicit ref → `6.35.0`+ |
| `System.Formats.Asn1` | 6.0.0 | High | GHSA-447r-wph3-92pm | Add explicit ref → `6.0.1`+ |
| `System.Security.Cryptography.Pkcs` | 6.0.0 | High | GHSA-555c-2p6r-68mm | Fixed by MailKit bump; else explicit → `6.0.4`+ |
| `System.Net.Http` | 4.3.0 | High | GHSA-7jgj-8wvc-jh57 | Legacy transitive; remove/override — runtime-provided on net6 |
| `System.Text.RegularExpressions` | 4.3.0 | High | GHSA-cmhx-cq75-c4mj | Legacy transitive; remove/override — runtime-provided on net6 |

Implementation notes:
- Prefer **central pinning** in `src/Directory.Build.props` (a shared `ItemGroup` of explicit `PackageReference` overrides) so the fix applies to every project uniformly. This is the least invasive way to force safe transitive versions.
- The two `4.3.0` `System.*` packages are legacy references dragged in by old transitive graphs; on `net6.0` they are provided by the shared framework. Overriding them to a patched version (or eliminating the offending parent) silences the advisory.
- After changes: `dotnet restore` then re-run `dotnet list package --vulnerable --include-transitive` and confirm an empty report.

Validation: full solution build + unit tests green; vulnerable-package report clean.

Exit criteria: `dotnet list package --vulnerable` reports **0** packages.

---

## Phase 2 — Node & npm security remediation (low/medium risk, frontend-isolated)

Goal: update the Node toolchain pin and clear npm advisories, mostly via `resolutions` since the majority are transitive dev-dependencies of the webpack toolchain.

1. **Update the Node/Yarn pin** in `package.json`:
   ```json
   "volta": { "node": "20.18.0", "yarn": "1.22.22" }
   ```
   Node 20 is LTS and matches what the environment already runs. (Node 22 is also acceptable.)
2. **Force-patch transitive advisories** with a `resolutions` block. Target fixed versions:

   | Package | Advisory class | Target |
   | --- | --- | --- |
   | `ws` | High DoS (many headers / tiny fragments) | `^8.18.0` |
   | `immutable` | High Prototype Pollution / DoS | `^4.3.7` |
   | `path-to-regexp` | High ReDoS backtracking | `^1.9.0` (or `0.1.12` where pinned) |
   | `qs` | Moderate/Low DoS | `^6.14.0` |
   | `tough-cookie` | Moderate Prototype Pollution | `^4.1.4` |
   | `node-fetch` | High header forwarding | `^2.7.0` |
   | `@babel/*` (helpers/traverse) | Moderate ReDoS | latest `7.26.x` |
   | `@sentry/browser`, `@sentry/integrations` | Moderate Prototype Pollution | `7.120.x` (or plan 8.x) |

3. **`lodash` `_.template` code-injection (High, GHSA):** lodash 4.17.21 is already the latest 4.x and this advisory has **no patched 4.x release**. Remediate by auditing for `_.template` usage (none expected in app code) and, longer term, trimming lodash usage or moving to `lodash-es`/native. Track as accepted-with-mitigation if `_.template` is unused.
4. Re-run `yarn install` + `yarn audit`; confirm High count drops to zero (or documented accepted items only). Rebuild the UI (`yarn run build --env production`) and re-run `yarn lint`.

Validation: production webpack bundle builds; ESLint + Stylelint clean; `yarn audit` shows no unaccepted High advisories.

Exit criteria: no unaccepted High npm advisories; Node pin current.

---

## Phase 3 — Runtime upgrade: .NET 6 → .NET 10 LTS (high risk, the core migration)

Goal: move the whole solution to a supported LTS runtime. Most `System.*` runtime advisories from Phase 1 also disappear permanently here because the shared framework provides patched assemblies.

Recommended sequencing: optionally land an intermediate **net8.0** checkpoint first (smaller behavioral delta, easy to validate), then move to **net10.0**. Either way the mechanical steps are the same.

1. **Bump the SDK**: update `global.json` to the target band (e.g. `10.0.x`) and `azure-pipelines.yml` `dotnetVersion` / all `UseDotNet@2` tasks (8 occurrences) to the matching SDK.
2. **Retarget the TFM**: change `net6.0` → `net10.0` and `net6.0-windows` → `net10.0-windows` across the 26 projects. Centralize the TFM in `src/Directory.Build.props` if not already, to avoid per-project drift.
3. **Update literal `net6.0` references** in build/runtime glue:
   - `src/Directory.Build.props`
   - `src/NzbDrone.Test.Common/NzbDroneRunner.cs`
   - `src/NzbDrone.Common/EnvironmentInfo/RuntimeInfo.cs`
   - `build.sh` (all `PackageTests`/`Package` calls hard-code `"net6.0"`) and every `net6.0` path in `azure-pipelines.yml` publish steps.
4. **Framework-provided packages**: remove now-redundant `Microsoft.Extensions.*` / `System.*` explicit versions that the target shared framework supplies; keep only genuinely external ones.
5. **Runtime identifiers / packaging**: verify the RID list in `Directory.Build.props` and the `build.sh` packaging matrix still map to supported runtime packs on the new SDK (win/linux/osx/musl/arm variants).
6. **Code fixes**: address new analyzer diagnostics (`TreatWarningsAsErrors=true` is on, so warnings block the build), any obsoleted API usages, and nullable/analysis-level changes (`AnalysisLevel` is `6.0-all` → bump to match).
7. **Update the Cursor environment** (`.cursor/install.sh` from PR #1) to install the new SDK channel instead of `6.0`, so Cloud Agents match CI.

Validation: `dotnet msbuild -restore src/Whisparr.sln -t:Build` clean on the new SDK; full unit suite green; app boots and serves `/ping` 200; smoke-test the packaged Linux artifact.

Exit criteria: solution targets the LTS TFM, CI builds/publishes all RIDs, tests green, app runs.

---

## Phase 4 — Major library upgrades (medium risk, incremental, optional per library)

Goal: reduce the backlog of major-version-behind libraries. Do these one PR at a time behind the green test gate; none are required for security once Phases 1–3 land.

Backend candidates (current → suggested):
- `FluentValidation` 9.5.4 → 11.x (API changes in validators)
- `DryIoc.dll` 5.4.1 / `DryIoc.Microsoft.DependencyInjection` 6.2.0 → latest
- `Swashbuckle.AspNetCore.SwaggerGen` 6.4.0 → latest (aligns with new ASP.NET)
- `RestSharp` 106.15.0 → 11x (major API change; scope carefully) — or confirm still needed
- `Dapper` 2.0.123, `Newtonsoft.Json` 13.0.2, `NLog` 4.7.14 → latest minors
- `Sentry` 3.23.1 → latest (coordinate with frontend Sentry bump)

Frontend candidates:
- **React 17 → 18** (concurrent features; `react-dom` root API change) — the largest frontend item; validate with the full UI walkthrough.
- `react-router`/`react-router-dom` 5 → 6 (or keep 5 if 18 migration is deferred)
- `webpack` 5 minor refresh + loaders/plugins; `typescript` 4.9 → 5.x
- Remove/replace deprecated `react-*` addons (`react-addons-shallow-compare`, etc.)

Each library PR: bump → build → unit tests → for UI libs, run the manual UI walkthrough (dashboard, System → Status, Settings) and compare against the Phase 0 baseline.

---

## Phase 5 — Validation, rollout, and rollback

1. **Full regression**: `dotnet test src/Whisparr.sln` (all unit projects) + integration tests with network access to `api.whisparr.com`; `yarn lint`, `yarn stylelint-linux`, production bundle build.
2. **End-to-end smoke**: boot the packaged app, run DB migrations on a fresh data dir, exercise a real action (add a site / trigger a search) beyond just loading a page.
3. **Cross-RID packaging**: build the full `build.sh --packages` matrix and smoke-test at least linux-x64, linux-musl-x64, and win-x64.
4. **Cloud Agent parity**: rebuild the Cursor environment (PR #1 scripts + new SDK) and verify a fresh agent boots and serves.
5. **Rollback**: each phase is an isolated, revertable PR; keep `develop` releasable at every step. The EOL runtime and current deps remain functional, so there is no forced cutover — phases can pause between merges.

---

## Fix checklist (quick reference)

- [ ] Phase 0: add `global.json`; restore test/lint gates; add nuget + npm vuln scans to CI
- [ ] Phase 1: central-pin .NET overrides (MailKit 4.x, SqlClient 5.x, JWT 6.35+, Asn1 6.0.1, Pkcs, drop legacy `System.Net.Http`/`System.Text.RegularExpressions` 4.3.0) → 0 vulnerable packages
- [ ] Phase 2: bump Node/Yarn pin; add `resolutions` (ws, immutable, path-to-regexp, qs, tough-cookie, node-fetch, babel, sentry); document lodash `_.template` mitigation
- [ ] Phase 3: retarget `net6.0` → `net10.0` (SDK, TFMs, `Directory.Build.props`, `build.sh`, `azure-pipelines.yml`, RIDs, code); update `.cursor/install.sh`
- [ ] Phase 4: major libs one PR each (React 17→18, FluentValidation, Swashbuckle, RestSharp, etc.)
- [ ] Phase 5: full regression, cross-RID packaging, e2e smoke, Cloud Agent parity, releasable `develop`

---

## Relationship to open PRs

- **PR #1** (`cursor/setup-dev-environment-ab13`): reproducible Cloud Agent dev environment — a prerequisite that makes this plan testable. Phase 3 updates its `install.sh` SDK channel.
- **PR #2** (`cursor/upgrade-automation-fe83`, draft): documents a v3 (`eros`) / v2-maintenance split. If the project pivots primary development to v3, apply Phases 1–2 (security) to v2 as maintenance and target Phases 3–4 (runtime + libraries) at the v3 line.
