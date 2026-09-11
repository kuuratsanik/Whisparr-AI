## Handoff
- **From:** cursor
- **To:** grok
- **Track:** B-Reimagine
- **Time (UTC):** 2026-09-11 19:45

### Goal
Expand `spikes/matchd/corpus/seed.jsonl` from 20 → ≥200 adversarial / dialect cases.

### Context
- Branch: `cursor/matchd-golden-set-a3ae` (PR incoming)
- Files: `spikes/matchd/README.md`, `spikes/matchd/corpus/seed.jsonl`
- Prompt: `prompts/grok/SYSTEM.md`

### Already done
- Seed corpus gs-001…gs-020 with labels
- Gate criteria documented

### Constraints
- Do NOT invent production PII; sanitize titles
- Prefer adult-scene dialects + category prefixes + JAV codes + ambiguous performer/title boundaries
- Keep JSONL schema identical

### Done when
- [ ] ≥200 labeled rows
- [ ] Top 20 highest-risk cases called out in a short `spikes/matchd/corpus/RISKS.md`
- [ ] Handoff back to Cursor with file paths

### Evidence
Corpus line count + RISKS.md
