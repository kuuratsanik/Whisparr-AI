#!/usr/bin/env bash
# Launches the Whisparr backend (which also serves the React UI) on port 6969.
# Run as a Cursor Cloud Agent terminal so logs are visible and it can be
# restarted easily.
set -euo pipefail

cd "$(dirname "$0")/.."

export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$HOME/.dotnet:$PATH"
export DOTNET_CLI_TELEMETRY_OPTOUT=1

DATA_DIR="$HOME/whisparr-data"
mkdir -p "$DATA_DIR"

cd _output/net6.0
exec ./Whisparr -nobrowser -data="$DATA_DIR"
