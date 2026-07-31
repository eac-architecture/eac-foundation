#!/usr/bin/env bash

set -euo pipefail

root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
namespace="${TEKTON_NAMESPACE:-eac-cicd}"
context="${KUBE_CONTEXT:-kind-eac-cicd}"
service_account="${TEKTON_SERVICE_ACCOUNT:-eac-ci}"
git_url="${1:-}"
git_revision="${2:-main}"

[[ -n "$git_url" ]] || {
    printf 'Usage: %s <git-url> [git-revision]\n' "$0" >&2
    exit 2
}

if ! GIT_TERMINAL_PROMPT=0 git ls-remote \
    --exit-code \
    "$git_url" \
    "$git_revision" >/dev/null 2>&1; then
    printf '[ERROR] Git revision is not anonymously accessible: %s (%s)\n' \
        "$git_url" \
        "$git_revision" >&2
    printf '[INFO] Create the public repository, push the revision, and run the command again.\n' >&2
    exit 3
fi

bash "$root_dir/scripts/apply-tekton-ci.sh"
tkn pipeline start eac-foundation-continuous-integration \
    --context "$context" \
    --namespace "$namespace" \
    --serviceaccount "$service_account" \
    --param "git-url=$git_url" \
    --param "git-revision=$git_revision" \
    --use-param-defaults \
    --workspace \
        "name=source,volumeClaimTemplateFile=$root_dir/ci/tekton/workspaces/source-volume-claim-template.yaml" \
    --pod-template "$root_dir/ci/tekton/pod-template.yaml" \
    --prefix-name eac-foundation-ci- \
    --showlog \
    --exit-with-pipelinerun-error
