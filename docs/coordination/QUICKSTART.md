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
1. Cursor posts / reads handoffs in Slack (and may cover idle cloud-capable seats itself).
2. Assignees who are **actually staffed** work their seat (paste prompts; Claude/Grok/Gemini do not auto-subscribe).
3. **Zed only moves when a human opens a Zed turn** on the M93p (or local machine). Slack “@zed” is a reminder for you, not a network delivery.
4. Cursor updates Notion statuses + PRs.
5. Gemini reviews open PRs when someone pastes the reviewer prompt; Grok feeds adversarial fixtures when matching work is active.

## Asking another model for help
Use the Handoff template — never “hey fix whisparr” without Goal / Branch / Done-when.

## Do not
- Expect Cursor Cloud to reach `~/coordination/` or trigger Agent-Bus bash-with-sudo.
- Enable Agent-Bus command execution without an explicit human yes (unattended root risk).
- Blame “Zed silence” on Slack when no local turn was started.
