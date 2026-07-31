#!/usr/bin/env bash

set -euo pipefail

export DOTNET_CLI_UI_LANGUAGE=en
export DOTNET_NOLOGO=1

root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
solution="$root_dir/EAC.Foundation.sln"
configuration="${BUILD_CONFIGURATION:-Release}"

case "$configuration" in
    Release | Debug)
        ;;
    *)
        printf '[ERROR] BUILD_CONFIGURATION must be Release or Debug\n' >&2
        exit 2
        ;;
esac

dotnet restore "$solution" \
    --configfile "$root_dir/NuGet.Config" \
    --locked-mode
dotnet format "$solution" \
    --verify-no-changes \
    --no-restore \
    --verbosity minimal
dotnet build "$solution" \
    --configuration "$configuration" \
    --no-restore

printf '[OK] Build quality validation completed\n'
