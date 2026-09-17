# Whisparr (Zed project rules)

Follow the repo root `AGENTS.md` and `docs/coordination/ROSTER.md` (especially **Transport reality**).

- Track A code branches off `eros`; docs for v2 messaging off `develop`.
- Branch prefix for this IDE: `zed/<desc>-a3ae`.
- You only run when a human opens a Zed turn — Slack/Cursor Cloud cannot auto-dispatch you.
- After a session, leave a handoff in Slack `#whisparr-tech-upgrade` or `docs/coordination/inbox/`.
- Runtime decision for v3: **net8-now** (see PR #4 / `docs/RUNTIME_DECISION.md` on upgrade branches).
- Do not mix greenfield rewrite work into the net8 spike PR.
- Do not enable Agent-Bus remote bash-with-sudo unless the human explicitly asks.
