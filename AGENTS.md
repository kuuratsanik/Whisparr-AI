# Whisparr multi-agent coordination

This repo is worked by **several tools and models at once**. Read this before changing code.

## Source of truth

| Layer | Where |
| --- | --- |
| Product/tech plan | Notion: [Whisparr Technology Upgrade Plan](https://app.notion.com/p/3d69b4ffb7ec81f98ad9d91567b6958c) |
| Task board | Notion: [Whisparr Upgrade Tasks](https://app.notion.com/p/c728b8c93207460a80062a9918eb75ec) |
| Greenfield vision | `docs/GREENFIELD_REBUILD.md` (on `cursor/upgrade-eros-a3ae`) + Notion child page |
| Runtime decision | `docs/RUNTIME_DECISION.md` → **net8-now** for `eros` |
| Human chatter | Slack `#whisparr-tech-upgrade` |
| Code | GitHub `kuuratsanik/Whisparr-AI` |

## Two tracks (do not mix)

| Track | Goal | Branch home |
| --- | --- | --- |
| **A — Survive** | v3 primary, net8 spike, sync dispositions, parser fixtures | `eros` / `cursor/upgrade-eros-a3ae` |
| **B — Reimagine** | Scene-native identity + matching engine spike | Separate spike only after golden-set gate |

## Agent roster

| Seat | Tool / model | Owns | Must not |
| --- | --- | --- | --- |
| **Orchestrator** | Cursor Cloud (this agent) | PRs, Notion/Slack sync, merges of plan docs, subscriptions | Silent scope expansion into Track B rewrite |
| **Implementer (C#)** | Cursor / Claude | net8 TFMs, parser fixtures, backend bugs on `eros` | Blind upstream cherry-picks of gated parser commits |
| **Implementer (FE)** | Cursor / Claude | Node 20 align, Sonarr-line FE ports only | Vite rewrite in parallel with net8 |
| **Reviewer** | Gemini | Diff critique, risk notes, test-gap lists | Committing code without Orchestrator ack |
| **Adversary / red-team** | Grok | Break matching hypotheses, find false-positive grab cases | Merging or rewriting product docs |
| **Local IDE pair** | Zed (+ Claude/Gemini in Zed) | Fast local edits following `docs/coordination/HANDOFF.md` | Pushing to `develop`/`eros` without PR |
| **Docs synthesizer** | Claude (chat) | ADR/Notion prose from handoffs | Inventing status that isn’t in git/Notion |

Paste-ready prompts: `prompts/{cursor,claude,grok,gemini,zed}/`.

## Branch rules

- Prefix: `cursor/<desc>-a3ae` for Cursor Cloud; Zed locals use `zed/<desc>-a3ae`.
- Track A code → base **`eros`**. Track A docs that apply to v2 messaging → base **`develop`**.
- One concern per PR. No “net8 + greenfield rewrite” combo PRs.
- Update Notion task status when you start/finish a slice.

## Handoff protocol

1. Write a short note using `docs/coordination/HANDOFF.md` template into Slack thread **or** `docs/coordination/inbox/<date>-<agent>.md`.
2. Tag the next seat (`@cursor` / `@claude` / `@grok` / `@gemini` / `@zed`) with **Goal / Context / Constraints / Done-when**.
3. Orchestrator (Cursor) triages inbox → opens/updates PRs → marks Notion.

## Status snapshot (2026-09-11)

- Decision: **net8-now** for `eros`
- PR #4 — MVP-A docs + decision (`develop`)
- PR #5 — MVP-B–E + greenfield vision (`eros`, draft, hold merge for Docker)
- PR #3 — net10-direct docs **superseded for eros**
