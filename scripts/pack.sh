#!/usr/bin/env bash

set -euo pipefail

export DOTNET_CLI_UI_LANGUAGE=en
export DOTNET_NOLOGO=1

root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
project="$root_dir/src/EAC.Foundation/EAC.Foundation.csproj"
configuration="${BUILD_CONFIGURATION:-Release}"
skip_build=0

if [[ "${1:-}" == "--no-build" ]]; then
    skip_build=1
    shift
fi
[[ "$#" -eq 0 ]] || {
    printf '[ERROR] Unknown pack argument: %s\n' "$1" >&2
    exit 2
}

if [[ "$skip_build" -eq 0 ]]; then
    bash "$root_dir/scripts/build.sh"
fi

dotnet pack "$project" \
    --configuration "$configuration" \
    --no-build \
    --no-restore

printf '[OK] NuGet package created locally; no artifact was published\n'
