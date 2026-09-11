# Toolchain alignment (MVP-E)

**Branch:** `eros` / `cursor/upgrade-eros-a3ae`  
**Status:** Aligned

| Pin | Value | Source |
| --- | --- | --- |
| Volta Node | `20.11.1` | `package.json` → `volta.node` |
| Volta Yarn | `1.22.19` | `package.json` → `volta.yarn` |
| Azure Pipelines Node | `20.X` | `azure-pipelines.yml` → `nodeVersion` |
| .NET (after MVP-C spike) | `net8.0` / SDK `8.0.405` | csproj TFMs + `dotnetVersion` |

Volta Node 20.x matches CI Node 20.X. Do not pin Volta to Node 16 or Node 24 on this line — keep local and pipeline Node majors identical.

Frontend dependency bumps continue to arrive via Sonarr-owned sync (not Radarr FE).
