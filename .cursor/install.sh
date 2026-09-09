#!/usr/bin/env bash
# Idempotent repository bootstrap for Whisparr (Cursor Cloud Agent environment).
# Installs the .NET 6 SDK (user-local), restores/builds the backend, installs
# frontend dependencies, and builds the React UI bundle into _output/UI.
set -euo pipefail

cd "$(dirname "$0")/.."
REPO_ROOT="$(pwd)"

DOTNET_DIR="$HOME/.dotnet"
export DOTNET_ROOT="$DOTNET_DIR"
export PATH="$DOTNET_DIR:$PATH"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

# --- .NET 6 SDK (Whisparr targets net6.0) ---------------------------------
if ! "$DOTNET_DIR/dotnet" --list-sdks 2>/dev/null | grep -q '^6\.'; then
  echo "Installing .NET 6 SDK into $DOTNET_DIR ..."
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  bash /tmp/dotnet-install.sh --channel 6.0 --install-dir "$DOTNET_DIR"
else
  echo ".NET 6 SDK already present."
fi

# Make dotnet available in the agent's interactive shells too.
BASHRC="$HOME/.bashrc"
if [ -f "$BASHRC" ] && ! grep -q 'DOTNET_ROOT=.*/.dotnet' "$BASHRC"; then
  {
    echo ''
    echo '# Whisparr / .NET 6 SDK'
    echo "export DOTNET_ROOT=\"\$HOME/.dotnet\""
    echo "export PATH=\"\$HOME/.dotnet:\$PATH\""
    echo 'export DOTNET_CLI_TELEMETRY_OPTOUT=1'
  } >> "$BASHRC"
fi

dotnet --info | head -n 5 || true

# --- Frontend dependencies + UI bundle ------------------------------------
echo "Installing frontend dependencies ..."
yarn install --frozen-lockfile --network-timeout 120000

echo "Building frontend UI bundle ..."
yarn run build --env production

# --- Backend build (Debug so the UI is served from ../UI) -----------------
echo "Building backend solution ..."
dotnet msbuild -restore "$REPO_ROOT/src/Whisparr.sln" \
  -p:Configuration=Debug \
  -p:Platform=Posix \
  -p:RuntimeIdentifiers=linux-x64 \
  -t:Build

echo "Whisparr environment bootstrap complete."
