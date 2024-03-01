#!/bin/sh
# Seed the shared folders that contain the ocr models

echo "Cleaning shared folder"

rm -rf /shared/*
cp -R /tmp/backup/shared/* /shared
chmod -R 777 /shared

ls -all /shared/formrecognizer
echo "Successfully copied Form Recognizer models to shared folder"
exit 0;
