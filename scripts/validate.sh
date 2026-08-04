#!/usr/bin/env bash

set -euo pipefail

root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
manifest="$root_dir/eng/capabilities.yml"
project="$root_dir/src/EAC.Foundation/EAC.Foundation.csproj"

required_files=(
    "$root_dir/.config/dotnet-tools.json"
    "$root_dir/Directory.Build.props"
    "$root_dir/Directory.Packages.props"
    "$root_dir/EAC.Foundation.sln"
    "$root_dir/NuGet.Config"
    "$root_dir/VERSION"
    "$root_dir/global.json"
    "$manifest"
    "$project"
    "$root_dir/scripts/release-candidate.sh"
    "$root_dir/scripts/version.sh"
)

for file in "${required_files[@]}"; do
    [[ -f "$file" ]] || {
        printf '[ERROR] Required repository file is missing: %s\n' "${file#"$root_dir"/}" >&2
        exit 1
    }
done

for script in "$root_dir"/scripts/*.sh; do
    bash -n "$script"
done

manifest_product="$(sed -nE 's/^product:[[:space:]]*(.+)$/\1/p' "$manifest")"
manifest_version_source="$(sed -nE 's/^versionSource:[[:space:]]*(.+)$/\1/p' "$manifest")"
manifest_framework="$(sed -nE 's/^targetFramework:[[:space:]]*(.+)$/\1/p' "$manifest")"
project_package="$(sed -nE 's/.*<PackageId>([^<]+)<\/PackageId>.*/\1/p' "$project")"
project_version="$(sed -nE 's/.*<Version>([^<]+)<\/Version>.*/\1/p' "$project")"
project_framework="$(sed -nE 's/.*<TargetFramework>([^<]+)<\/TargetFramework>.*/\1/p' "$root_dir/Directory.Build.props")"
source "$root_dir/scripts/version.sh"
validate_package_version "1.2.3" || {
    printf '[ERROR] Stable SemVer versions must be accepted\n' >&2
    exit 1
}
validate_package_version "1.2.3-rc.1" || {
    printf '[ERROR] Governed prerelease SemVer versions must be accepted\n' >&2
    exit 1
}
if validate_package_version "1.2.3-preview.1"; then
    printf '[ERROR] Unsupported prerelease channels must be rejected\n' >&2
    exit 1
fi
package_version="$(resolve_package_version)"

[[ "$manifest_product" == "$project_package" ]] || {
    printf '[ERROR] Manifest product and PackageId do not match\n' >&2
    exit 1
}
[[ "$manifest_version_source" == "VERSION" ]] || {
    printf '[ERROR] Manifest versionSource must be VERSION\n' >&2
    exit 1
}
[[ "$project_version" == "0.0.0-local" ]] || {
    printf '[ERROR] The project fallback version must remain 0.0.0-local\n' >&2
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

printf '[OK] Package version resolved from VERSION: %s\n' "$package_version"

bash "$root_dir/scripts/validate-docs.sh"
printf '[OK] Scope and governance validation completed\n'
