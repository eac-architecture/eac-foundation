#!/usr/bin/env bash

set -euo pipefail

export TESTINGPLATFORM_TELEMETRY_OPTOUT=1
export DOTNET_CLI_UI_LANGUAGE=en
export DOTNET_NOLOGO=1

root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
solution="$root_dir/EAC.Foundation.sln"
configuration="${BUILD_CONFIGURATION:-Release}"
test_output="${TEST_OUTPUT:-Table}"
test_color="${TEST_COLOR:-Auto}"
skip_build=0

if [[ "${1:-}" == "--no-build" ]]; then
    skip_build=1
    shift
fi
[[ "$#" -eq 0 ]] || {
    printf '[ERROR] Unknown test argument: %s\n' "$1" >&2
    exit 2
}

case "$test_output" in
    Table | Detailed | Normal)
        ;;
    *)
        printf '[ERROR] TEST_OUTPUT must be Table, Detailed or Normal\n' >&2
        exit 2
        ;;
esac

case "$test_color" in
    Auto | Always | Never)
        ;;
    *)
        printf '[ERROR] TEST_COLOR must be Auto, Always or Never\n' >&2
        exit 2
        ;;
esac

if [[ "$skip_build" -eq 0 ]]; then
    bash "$root_dir/scripts/build.sh"
fi

use_test_color=0
if [[ "$test_color" == "Always" ]] ||
    [[ "$test_color" == "Auto" && -t 1 && -z "${NO_COLOR:-}" ]]; then
    use_test_color=1
fi

if [[ "$test_output" == "Table" ]]; then
    shopt -s globstar nullglob
    test_sources=("$root_dir"/tests/**/*.cs)
    shopt -u globstar nullglob

    dotnet test \
        --solution "$solution" \
        --configuration "$configuration" \
        --no-build \
        --no-restore \
        --no-ansi \
        --no-progress \
        --output Detailed \
        --method-display classAndMethod \
        | awk \
            -v use_color="$use_test_color" \
            -f "$root_dir/scripts/format-test-output.awk" \
            "${test_sources[@]}" \
            -
else
    dotnet test \
        --solution "$solution" \
        --configuration "$configuration" \
        --no-build \
        --no-restore \
        --output "$test_output"
fi

printf '[OK] Automated tests completed\n'
