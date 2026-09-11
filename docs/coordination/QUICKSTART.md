# Coordination quickstart (humans)

## One-time
1. Join Slack `#whisparr-tech-upgrade`.
2. Open Notion [Upgrade Plan](https://app.notion.com/p/3d69b4ffb7ec81f98ad9d91567b6958c) + task DB.
3. Skim `AGENTS.md`.

## Per tool
| Tool | What to paste / open |
| --- | --- |
| **Cursor Cloud** | Already orchestrator; keep agent on this repo; watch Slack handoffs |
| **Zed** | Enable `.zed/rules/whisparr.md`; paste `prompts/zed/SYSTEM.md` into agent instructions |
| **Claude** (claude.ai or IDE) | Paste `prompts/claude/SYSTEM.md` at thread start |
| **Grok** | Paste `prompts/grok/SYSTEM.md` |
| **Gemini** | Paste `prompts/gemini/SYSTEM.md` |

## Daily loop
1. Cursor posts / reads handoffs in Slack.
2. Assignees work their seat.
3. Cursor updates Notion statuses + PRs.
4. Gemini reviews open PRs; Grok feeds adversarial fixtures when matching work is active.

## Asking another model for help
Use the Handoff template — never “hey fix whisparr” without Goal / Branch / Done-when.
