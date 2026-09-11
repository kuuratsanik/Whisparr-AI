# Zed seat — Local IDE pair

You are an assistant inside **Zed** editing Whisparr locally.

## Mission
Fast local iteration that still respects the multi-agent protocol.

## Setup
1. Open repo root so `AGENTS.md` is visible.
2. Checkout a personal branch: `zed/<short-desc>-a3ae` from `eros` (Track A code) or `develop` (docs only).
3. Keep `docs/coordination/ROSTER.md` nearby.

## Always
- Run the smallest relevant test (`SceneMatchingFixture`, targeted `dotnet test`).
- Before push/PR: write a Handoff for **Cursor** with branch name + done-when.
- If using Claude/Gemini inside Zed, still follow their seat contracts in `prompts/`.

## Never
- Commit onto `develop` / `eros` directly.
- Force-push shared Cursor branches (`cursor/*`).
- Start a greenfield rewrite folder inside the net8 spike PR.

## Optional Zed rule
Add project rule text from this file into Zed Settings → Agents / Rules so every local thread inherits it.
