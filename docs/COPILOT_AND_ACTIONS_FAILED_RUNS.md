# Preventing failed Copilot / Actions runs (Whisparr-AI)

**Date:** 2026-09-17  
**Repo:** `kuuratsanik/Whisparr-AI`  
**Default branch:** `develop`

## What is actually failing today

There are **no GitHub Copilot coding-agent sessions, PRs, or branches** in this repository’s history (no Copilot-authored PRs; no `copilot-setup-steps.yml` on `develop` / `eros` / upstream Whisparr either).

The only recurring red GitHub Actions runs on `develop` are:

| Workflow | Cadence | Root cause |
| --- | --- | --- |
| **Upstream Pull** | daily `0 6 * * *` | `actions/create-github-app-token` throws `Input required and not supplied: app-id` because secrets `SYNC_APP_ID` / `SYNC_APP_PRIVATE_KEY` are unset |

Latest example: run `35215459244` (2026-09-17) — fails at “Mint an app token”, then skips the rest. Seven consecutive scheduled failures in the last week.

PR checks on #4–#7 (Snyk / Semgrep) are green; they are unrelated.

## If you meant GitHub Copilot coding agent

Copilot cloud agent runs in an ephemeral Actions environment. Failed agent sessions are usually caused by:

1. **Missing / brittle setup** — no `.github/workflows/copilot-setup-steps.yml` on the **default branch** (`develop`), so Copilot trial-and-errors dependency install and often stalls on this large .NET + Node monorepo.
2. **Hard-failing setup steps** — if a setup step exits non-zero, remaining setup is skipped and the agent starts in a half-prepared env (or looks “failed” in session logs). Best practice: mark build/restore steps `continue-on-error: true` so `@copilot fix …` can still enter the workspace after a broken commit.
3. **Wrong OS / runner** — agent is Ubuntu x64 (or Windows) only; macOS runners are unsupported.
4. **Firewall / private deps** — private NuGet/npm or blocked egress without Agent secrets/variables.
5. **No repo instructions** — without `.github/copilot-instructions.md`, the agent may target the wrong TFM (`net10` on `develop` vs `net8` on `eros`), merge Docker-gated PRs, or fight Azure Pipelines vs GHA.

### Prevention checklist (Copilot)

- [x] Add resilient `.github/workflows/copilot-setup-steps.yml` (this PR) — must land on `develop`
- [x] Add `.github/copilot-instructions.md` with branch/TFM/Docker-hold constraints
- [ ] Repo/org: enable Copilot coding agent only if you intend to use it; configure Agent secrets if private feeds are needed
- [ ] After merge: Actions → “Copilot Setup Steps” → Run workflow once to validate
- [ ] Prefer assigning small, scoped issues; this repo’s full matrix build is too heavy for agent loops

Official refs:
- https://docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/customize-the-agent-environment
- https://devblogs.microsoft.com/dotnet/maui-team-copilot-tips/

## If you meant the red Actions runs (most likely)

### Prevention (implemented here)

`upstream_pull.yml` now **succeeds as a no-op** when `SYNC_APP_ID` / `SYNC_APP_PRIVATE_KEY` are missing, with an Actions notice explaining how to enable sync. That stops daily red noise without disabling the workflow forever.

### To actually enable Upstream Pull later

1. Create a GitHub App with **Contents**, **Issues**, **Pull requests** read/write on this repo (see comments in `upstream_pull.yml`).
2. Set repository Actions secrets:
   - `SYNC_APP_ID`
   - `SYNC_APP_PRIVATE_KEY`
3. Re-run **Upstream Pull** via `workflow_dispatch` (optionally `dry_run=true` first).

### Alternate preventions (not chosen)

- Disable the schedule (`on: workflow_dispatch` only) until secrets exist
- Delete/rename the workflow — loses the Sonarr track design documented in-file

## Out of scope

- Fixing Sonarr cherry-pick conflicts once sync is enabled
- Enabling Copilot org-wide
- Changing PR #5 Docker hold / MatchD Track B
