# Multi-agent roster & protocol

Whisparr work spans **Cursor, Zed, Claude, Grok, and Gemini**. None of them share memory. This folder is the shared brain.

## How coordination actually works

```text
                 Notion (plan + tasks)
                        ▲
                        │
   Slack #whisparr-tech-upgrade  ←── handoffs / status
                        ▲
                        │
              Cursor Cloud (orchestrator)
                 │         │
        ┌────────┼─────────┼────────┐
        ▼        ▼         ▼        ▼
     Claude    Gemini     Grok     Zed
   (implement) (review) (red-team) (local IDE)
```

Cursor owns the GitHub remote and Notion/Slack writes. Other seats produce **handoffs + patches/PRs** Cursor can land.

## Transport reality (read this)

Cloud Cursor **cannot** dispatch local agents. Treat the diagram above as a *human paste* protocol, not a live message bus.

| Channel | Who hears it | Cloud Cursor can… |
| --- | --- | --- |
| Slack `#whisparr-tech-upgrade` | Humans + Cursor Cloud subscription | Post / read standups |
| GitHub PRs / `docs/coordination/inbox/` | Anyone who opens the repo | Write handoffs; cannot force another seat to run |
| M93p `~/coordination/` (stdio MCP) | Zed Claude ACP **only when a human is driving a turn** | **Nothing** — LAN / cloudflared blocked from cloud |
| Google Drive `Agent-Bus` | Heartbeat publisher only | Write files if Drive MCP is auth’d; **must not** enable remote bash-with-sudo |

**Zed** is the local seat that is actually wired (Claude ACP + MCP to the coordination bus + scoped FS). It does **not** auto-poll Slack. Cursor Cloud is **not** on the M93p “active agents on the bus” list yet.

**Daily standups must not claim “Zed owes X”** unless a human is actively running Zed on that task. Local Docker / estate work is **human-or-Zed-at-keyboard**, not cloud-orchestrated.

**Agent-Bus command execution:** human **YES** recorded 2026-09-17 (`gdrive:Agent-Bus/AUTHORIZATION-COMMAND-EXEC-20260917.md`). Cloud staged `agent-bus-poll-WITH-EXEC.sh` + `enable-command-exec.sh`. **Not live until** someone on the M93p runs `enable-command-exec.sh`. Kill switch remains `STOP` in Agent-Bus root.

## Open PRs (coord map)

| PR | Track | Base | Notes |
| --- | --- | --- | --- |
| [#7](https://github.com/kuuratsanik/Whisparr-AI/pull/7) | B | `develop` | MatchD golden-set + baseline (draft) |
| [#6](https://github.com/kuuratsanik/Whisparr-AI/pull/6) | — | `develop` | Multi-agent coordination kit |
| [#4](https://github.com/kuuratsanik/Whisparr-AI/pull/4) | A | `develop` | MVP-A + net8-now decision |
| [#5](https://github.com/kuuratsanik/Whisparr-AI/pull/5) | A (+ vision) | `eros` | net8 spike, dispositions, fixtures; **Docker hold** |
| [#3](https://github.com/kuuratsanik/Whisparr-AI/pull/3) | — | `develop` | net10-direct docs; superseded **for eros** |
| [#2](https://github.com/kuuratsanik/Whisparr-AI/pull/2) | A | `develop` | superseded by #4 |

## Active Cursor agents

| Agent | URL | Role |
| --- | --- | --- |
| Current planned roadmap | https://cursor.com/agents/bc-444b7a12-b5c3-4388-8fe9-b4d0a8dda3ae | Orchestrator (this lane) |
| Eros net8 spike prep | https://cursor.com/agents/bc-a12c0ada-12df-5715-8457-e9e481b9084e | Implementer assist |
| Explore Whisparr domain core | https://cursor.com/agents/bc-1b29c94c-51b9-55b1-8ba0-b42be8322bd2 | Research |

## Seat contracts

See `prompts/*/SYSTEM.md` for paste-ready instructions per tool.

### Cursor
- Orchestrates Tracks A/B, keeps Notion + Slack truthful, opens/updates PRs.
- Subscribes to `#whisparr-tech-upgrade` handoff thread.

### Zed
- Local pair-programmer on the M93p (or any machine with the repo). Reads `AGENTS.md`. Branch prefix `zed/…-a3ae`.
- Listens to the **local** coordination bus / human-driven Zed turns — **not** Slack auto-dispatch from Cursor Cloud.
- Never force-pushes shared branches. Ends session with a Handoff (Slack or inbox) so Cursor can triage.
- Docker publisher smoke / estate scripts: run locally when at the keyboard; do not expect cloud to trigger them.

### Claude
- Deep implementer / ADR writer. Prefer focused diffs + fixtures.
- When used inside Cursor or Zed, still emit a Handoff for cross-tool visibility.

### Grok
- Adversarial matching review only unless Orchestrator assigns otherwise.
- Output: failure cases, filename corpus suggestions, risk list — not drive-by refactors.

### Gemini
- Structured review: correctness, missing tests, migration risk, API surface.
- Prefer tables and “ship / hold / rewrite” verdicts.

## Conflict resolution

1. Notion task status wins over chat memory.
2. `docs/RUNTIME_DECISION.md` wins over older upgrade docs for runtime target.
3. Track A merge holds beat Track B enthusiasm until golden-set gate passes.
4. Orchestrator (Cursor) is tie-breaker for branch/PR routing.
