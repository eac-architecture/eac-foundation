#!/usr/bin/env bash

set -euo pipefail

root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
namespace="${TEKTON_NAMESPACE:-eac-cicd}"
context="${KUBE_CONTEXT:-kind-eac-cicd}"

kubectl --context "$context" get namespace "$namespace" >/dev/null
kubectl --context "$context" \
    --namespace "$namespace" \
    apply \
    --filename "$root_dir/ci/tekton/tasks"
kubectl --context "$context" \
    --namespace "$namespace" \
    apply \
    --filename "$root_dir/ci/tekton/pipelines"

printf '[OK] EAC.Foundation continuous integration resources applied to %s\n' "$namespace"
