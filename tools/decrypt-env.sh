#!/bin/bash
script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

age --decrypt \
  -o $script_dir/../.env \
  -i $AGE_KEY_FILE \
  $script_dir/env.age
