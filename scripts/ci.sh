#!/usr/bin/env bash

set -euo pipefail

export TESTINGPLATFORM_TELEMETRY_OPTOUT=1
export DOTNET_CLI_UI_LANGUAGE=en
export CI=true

root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

bash "$root_dir/scripts/validate.sh"
bash "$root_dir/scripts/build.sh"
bash "$root_dir/scripts/test.sh" --no-build

printf '[OK] Continuous integration checks completed\n'
