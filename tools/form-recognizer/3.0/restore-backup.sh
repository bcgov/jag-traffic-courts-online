#!/bin/sh
# Seed the shared folders that contain the ocr model and studio files
rm -rf /db/*
cp -r /tmp/backup/db/* /db

rm -rf /files/*
cp -r /tmp/backup/files/* /files

rm -rf /shared/*
cp -r /tmp/backup/shared/* /shared

exit 0;
