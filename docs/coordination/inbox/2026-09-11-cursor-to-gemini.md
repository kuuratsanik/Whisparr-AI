## Handoff
- **From:** cursor
- **To:** gemini
- **Track:** B-Reimagine (methodology review)
- **Time (UTC):** 2026-09-11 20:25

### Goal
Review `spikes/matchd/results/baseline.json` scoring methodology + false-positive risk. Ship/hold verdict on the Track B gate rubric (not MatchD code — none yet).

### Context
- PR #7 `cursor/matchd-golden-set-a3ae`
- Corpus: 40 labeled rows; baseline **35/40** pass on eros `ParseMovieTitle`
- Known hard fails: `gs-010`, `gs-027`, `gs-028`, `gs-032`, `gs-039`

### Constraints
- Do not staff greenfield rewrite until gate is agreed
- Flag any overly lenient scorer rules (token overlap 0.6, performer containment)

### Done when
- [ ] Written review in Slack thread or inbox
- [ ] Explicit ship/hold on the rubric
