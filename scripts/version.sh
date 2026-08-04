#!/usr/bin/env bash

set -euo pipefail

version_root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
version_file="$version_root_dir/VERSION"

read_package_version() {
    local declared_version

    [[ -f "$version_file" ]] || {
        printf '[ERROR] VERSION is required\n' >&2
        return 1
    }

    declared_version="$(tr -d '[:space:]' < "$version_file")"
    if ! validate_package_version "$declared_version"; then
        printf '[ERROR] VERSION must be a stable or alpha, beta or rc SemVer such as 0.1.0 or 0.1.0-rc.1\n' >&2
        return 1
    fi

    printf '%s' "$declared_version"
}

validate_package_version() {
    local candidate="${1:-}"

    [[ "$candidate" =~ ^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)(-(alpha|beta|rc)\.([1-9][0-9]*))?$ ]]
}

resolve_package_version() {
    local declared_version requested_version

    declared_version="$(read_package_version)" || return 1
    requested_version="${PACKAGE_VERSION:-$declared_version}"

    if [[ "$requested_version" != "$declared_version" ]]; then
        printf '[ERROR] PACKAGE_VERSION %s does not match VERSION %s\n' \
            "$requested_version" \
            "$declared_version" >&2
        return 1
    fi

    printf '%s' "$declared_version"
}

if [[ "${BASH_SOURCE[0]}" == "$0" ]]; then
    resolve_package_version
    printf '\n'
fi
