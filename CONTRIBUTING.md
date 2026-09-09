# How to Contribute

We're always looking for people to help make Whisparr even better, there are a number of ways to contribute.

This file has been moved to the wiki for the latest details please see the [contributing wiki page](https://wiki.servarr.com/whisparr/contributing).

## Documentation

Setup guides, [FAQ](https://wiki.servarr.com/whisparr/faq), the more information we have on the [wiki](https://wiki.servarr.com/whisparr) the better.

## Development

See the [Wiki Page](https://wiki.servarr.com/whisparr/contributing)

### Branch map (platform declaration)

| Intent | Branch to use | Notes |
| --- | --- | --- |
| **New features / upgrades / upstream sync** | `eros` (v3) | **Primary** product line. Runtime bumps (.NET), FE sync from Sonarr, and parser product work belong here. |
| **Security / critical fixes only** | `develop` (v2) | **Maintenance**. Upstream GitHub may name this `v2-develop`; local/fork clones often still use `develop`. |
| Do **not** dual-land major features on both lines | — | Prefer port or close as won’t-fix-on-v2 when a fix is v3-only. |

If you are unsure which line a change belongs on, assume **v3 (`eros`)**.
