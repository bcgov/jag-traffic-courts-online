#!/bin/sh
# Seed the shared folders that contain the studio files
rm -rf /db/*
cp -r /tmp/backup/db/* /db
chmod -R 777 /db

rm -rf /files/*
cp -r /tmp/backup/files/* /files
chmod -R 777 /files
exit 0;
