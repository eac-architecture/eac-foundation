#!/usr/bin/env bash

set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
docs="$root/docs"
index="$docs/INDICE_DOCUMENTAL.md"
errors="$(mktemp)"
orders="$(mktemp)"
trap 'rm -f "$errors" "$orders"' EXIT

[[ -f "$index" ]] || {
  printf 'Documentation index does not exist: %s\n' "$index" >&2
  exit 1
}

count=0
while IFS= read -r -d '' file; do
  ((count += 1))

  if [[ "$file" != "$index" ]]; then
    order_count="$(grep -cE '^> \*\*Orden documental:\*\* DOC-[0-9]{3} · \*\*Etapa:\*\* ' "$file" || true)"
    if [[ "$order_count" -ne 1 ]]; then
      printf '%s: documentation identifier is missing or duplicated\n' "$file" >> "$errors"
    else
      grep -oE 'DOC-[0-9]{3}' "$file" | head -n 1 >> "$orders"
    fi
  fi

  while IFS= read -r target; do
    target="${target%%#*}"
    [[ -z "$target" ]] && continue
    case "$target" in http://*|https://*|mailto:*) continue ;; esac
    [[ -e "$(dirname "$file")/$target" ]] \
      || printf '%s: local link does not exist: %s\n' "$file" "$target" >> "$errors"
  done < <(grep -oE '\[[^]]+\]\([^)]+\)' "$file" 2>/dev/null | sed -E 's/^.*\]\(([^)]+)\)$/\1/' || true)

  fence_count="$(grep -c '^```' "$file" || true)"
  (( fence_count % 2 == 0 )) || printf '%s: incomplete Markdown fences\n' "$file" >> "$errors"
done < <(find "$docs" -type f -name '*.md' -print0)

while IFS= read -r order; do
  [[ -z "$order" ]] && continue
  [[ "$(grep -cE "^\| ${order} \|" "$index" || true)" -eq 1 ]] \
    || printf '%s: must appear exactly once in the index\n' "$order" >> "$errors"
done < <(sort -u "$orders")

while IFS= read -r duplicate; do
  [[ -z "$duplicate" ]] || printf 'Duplicated identifier: %s\n' "$duplicate" >> "$errors"
done < <(sort "$orders" | uniq -d)

if [[ -s "$errors" ]]; then
  cat "$errors" >&2
  exit 1
fi

printf '[OK] Component documentation is valid: %d Markdown files\n' "$count"
