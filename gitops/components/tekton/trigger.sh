#!/bin/bash

# HMAC generated from payload + GH secret

curl -i \
  -H 'X-GitHub-Event: push' \
  -H 'X-Hub-Signature: sha1=...' \
  -H 'X-Hub-Signature-256: sha256=...'
  -H 'Content-Type: application/json' \
  -d '{"ref":"refs/heads/main","head_commit":{"id":"d9c7ccb0713da882d8373169e2a1d705f5959386"}}' \
  https://el-build-push-image-trigger-0198bb-tools.apps.silver.devops.gov.bc.ca/