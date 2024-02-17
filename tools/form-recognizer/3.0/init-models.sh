#!/bin/sh
# Seed the shared folders that contain the ocr models

rm -rf /shared/*
cp -r /tmp/backup/shared/* /shared
chmod -R 777 /shared

exit 0;
