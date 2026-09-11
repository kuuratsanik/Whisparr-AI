# Docker merge gate (PR #5 / eros net8)

**Status:** Hold merge of `cursor/upgrade-eros-a3ae` into `eros` until the items below are owned.

## Why
The net8 TFM/SDK bump changes published runtime images. Merging Core green without a coordinated Docker publisher update can ship broken tags to users.

## Checklist before merge
- [ ] Confirm image base / SDK tag for net8 (not leftover net6)
- [ ] Publisher (or release owner) signs off on rebuild + smoke of `linux-x64` image
- [ ] Document any dropped RID (e.g. `linux-x86`) in release notes
- [ ] Smoke: container starts, UI loads, parse of one Studio.Date scene title succeeds
- [ ] Link sign-off comment on PR #5

## Out of scope for this note
- Track B matchd golden-set (PR #7)
- net10 on eros

## Contact
Post in `#whisparr-tech-upgrade` with Docker publisher named before flipping #5 to ready.
