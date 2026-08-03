#!/usr/bin/env bash

set -euo pipefail

export DOTNET_CLI_UI_LANGUAGE=en
export DOTNET_NOLOGO=1
export TESTINGPLATFORM_TELEMETRY_OPTOUT=1

root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
project="$root_dir/src/EAC.Foundation/EAC.Foundation.csproj"
package_id="$(sed -nE 's/.*<PackageId>([^<]+)<\/PackageId>.*/\1/p' "$project")"
source "$root_dir/scripts/version.sh"
version="$(resolve_package_version)"
commit="${RELEASE_COMMIT:-}"
artifacts_dir="${ARTIFACTS_DIR:-$root_dir/artifacts}"
packages_dir="$artifacts_dir/packages"
sbom_dir="$artifacts_dir/sbom"
evidence_dir="$artifacts_dir/evidence"

if [[ -z "$package_id" ]]; then
    printf '[ERROR] PackageId could not be resolved from the project\n' >&2
    exit 1
fi
if [[ -z "$commit" ]]; then
    commit="$(git -C "$root_dir" rev-parse HEAD)"
fi

rm -rf "$packages_dir" "$sbom_dir" "$evidence_dir"
mkdir -p "$packages_dir" "$sbom_dir" "$evidence_dir"

printf '[INFO] Creating %s %s from %s\n' "$package_id" "$version" "$commit"

PACKAGE_VERSION="$version" bash "$root_dir/scripts/pack.sh" --no-build

mapfile -t packages < <(find "$packages_dir" -maxdepth 1 -type f -name '*.nupkg' | sort)
mapfile -t symbols < <(find "$packages_dir" -maxdepth 1 -type f -name '*.snupkg' | sort)
if [[ "${#packages[@]}" -ne 1 || "${#symbols[@]}" -ne 1 ]]; then
    printf '[ERROR] The candidate must contain exactly one .nupkg and one .snupkg\n' >&2
    exit 1
fi

dotnet tool restore --configfile "$root_dir/NuGet.Config"
dotnet tool run sbom-tool generate \
    -b "$packages_dir" \
    -bc "$root_dir" \
    -pn "$package_id" \
    -pv "$version" \
    -ps "Organization: EAC Architecture" \
    -nsb "https://github.com/eac-architecture/sbom"
dotnet tool run sbom-tool validate \
    -b "$packages_dir" \
    -o "$evidence_dir/sbom-validation.json" \
    -mi SPDX:2.2

if [[ ! -f "$packages_dir/_manifest/spdx_2.2/manifest.spdx.json" ]]; then
    printf '[ERROR] The SPDX 2.2 manifest was not generated\n' >&2
    exit 1
fi
mv "$packages_dir/_manifest/spdx_2.2" "$sbom_dir/spdx_2.2"
rm -rf "$packages_dir/_manifest"
sbom_file="$sbom_dir/spdx_2.2/manifest.spdx.json"

smoke_dir="$(mktemp -d)"
trap 'rm -rf "$smoke_dir"' EXIT
cat >"$smoke_dir/Smoke.csproj" <<EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="$package_id" Version="$version" />
  </ItemGroup>
</Project>
EOF
cat >"$smoke_dir/Program.cs" <<'EOF'
using EAC.Foundation.SharedKernel.Results;

return Result.Success().IsSuccess ? 0 : 1;
EOF
dotnet restore "$smoke_dir/Smoke.csproj" --source "$packages_dir"
dotnet run \
    --project "$smoke_dir/Smoke.csproj" \
    --configuration Release \
    --no-restore

package_name="$(basename "${packages[0]}")"
symbols_name="$(basename "${symbols[0]}")"
package_sha256="$(sha256sum "${packages[0]}" | awk '{print $1}')"
symbols_sha256="$(sha256sum "${symbols[0]}" | awk '{print $1}')"
sbom_sha256="$(sha256sum "$sbom_file" | awk '{print $1}')"

(
    cd "$artifacts_dir"
    sha256sum \
        "packages/$package_name" \
        "packages/$symbols_name" \
        "sbom/spdx_2.2/manifest.spdx.json"
) >"$evidence_dir/checksums.sha256"

generated_at="$(date -u +'%Y-%m-%dT%H:%M:%SZ')"
cat >"$evidence_dir/release-evidence.json" <<EOF
{
  "packageId": "$package_id",
  "version": "$version",
  "commit": "$commit",
  "generatedAtUtc": "$generated_at",
  "package": {
    "file": "$package_name",
    "sha256": "$package_sha256"
  },
  "symbols": {
    "file": "$symbols_name",
    "sha256": "$symbols_sha256"
  },
  "sbom": {
    "format": "SPDX-2.2",
    "file": "manifest.spdx.json",
    "sha256": "$sbom_sha256"
  },
  "smokeTest": "Succeeded"
}
EOF

printf '[OK] Release candidate created and verified\n'
printf '[RESULT] Package: %s\n' "$package_name"
printf '[RESULT] Package SHA-256: %s\n' "$package_sha256"
printf '[RESULT] Symbols: %s\n' "$symbols_name"
printf '[RESULT] SBOM: %s\n' "$sbom_file"
printf '[RESULT] Evidence: %s\n' "$evidence_dir/release-evidence.json"
