#!/bin/bash
script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

age --encrypt \
  -o $script_dir/env.age \
  -R $script_dir/age-recipients.txt \
  $script_dir/../.env
