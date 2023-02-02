#!/bin/bash
# Seed the shared folder that contains the ocr model on startup
if [ ! -f /shared/init.lock ]; then
  touch /shared/init.lock
  cp -r /shared-src/* /shared
fi
