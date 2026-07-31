#!/usr/bin/env bash

set -euo pipefail

root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
manifest="$root_dir/eng/capabilities.yml"
project="$root_dir/src/EAC.Foundation/EAC.Foundation.csproj"

required_files=(
    "$root_dir/Directory.Build.props"
    "$root_dir/Directory.Packages.props"
    "$root_dir/EAC.Foundation.sln"
    "$root_dir/NuGet.Config"
    "$root_dir/global.json"
    "$manifest"
    "$project"
)

for file in "${required_files[@]}"; do
    [[ -f "$file" ]] || {
        printf '[ERROR] Required repository file is missing: %s\n' "${file#"$root_dir"/}" >&2
        exit 1
    }
done

manifest_product="$(sed -nE 's/^product:[[:space:]]*(.+)$/\1/p' "$manifest")"
manifest_version="$(sed -nE 's/^version:[[:space:]]*(.+)$/\1/p' "$manifest")"
manifest_framework="$(sed -nE 's/^targetFramework:[[:space:]]*(.+)$/\1/p' "$manifest")"
project_package="$(sed -nE 's/.*<PackageId>([^<]+)<\/PackageId>.*/\1/p' "$project")"
project_version="$(sed -nE 's/.*<Version>([^<]+)<\/Version>.*/\1/p' "$project")"
project_framework="$(sed -nE 's/.*<TargetFramework>([^<]+)<\/TargetFramework>.*/\1/p' "$root_dir/Directory.Build.props")"

[[ "$manifest_product" == "$project_package" ]] || {
    printf '[ERROR] Manifest product and PackageId do not match\n' >&2
    exit 1
}
[[ "$manifest_version" == "$project_version" ]] || {
    printf '[ERROR] Manifest and project versions do not match\n' >&2
    exit 1
}
[[ "$manifest_framework" == "$project_framework" ]] || {
    printf '[ERROR] Manifest and project target frameworks do not match\n' >&2
    exit 1
}
grep -qE '^exceptions:[[:space:]]*\[\][[:space:]]*$' "$manifest" || {
    printf '[ERROR] Foundation 1.0 baseline cannot contain undeclared exceptions\n' >&2
    exit 1
}

"$root_dir/scripts/validate-docs.sh"
printf '[OK] Scope and governance validation completed\n'
