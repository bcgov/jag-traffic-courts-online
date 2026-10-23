#!/usr/bin/env bash
# Creates database/<db>/definitions symlinks pointing to ../TCO-API-<DB>/dbscripts/trunk (siblings of this repo).
set -euo pipefail

# Make Git Bash/MSYS create real Windows symlinks (requires Developer Mode or admin) instead of copies.
export MSYS=winsymlinks:nativestrict

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
workspace_root="$(cd "$script_dir/../.." && pwd)"

declare -A targets=(
  [occam]="TCO-API-OCCAM"
  [tco]="TCO-API-TCO"
  [occamords]="TCO-API-OCCAMORDS"
  [tcoords]="TCO-API-TCOORDS"
)

for db in "${!targets[@]}"; do
  db_dir="$script_dir/$db"
  link="$db_dir/definitions"
  target="$workspace_root/${targets[$db]}/dbscripts/trunk"

  if [[ ! -d "$target" ]]; then
    echo "Skipping $db: target not found: $target" >&2
    continue
  fi

  mkdir -p "$db_dir"

  if [[ -L "$link" || -f "$link" ]]; then
    rm -f "$link"
  elif [[ -d "$link" ]]; then
    echo "Skipping $db: $link is a real directory; remove it manually first." >&2
    continue
  fi

  ln -s "$target" "$link"
  echo "Linked $link -> $target"
done
