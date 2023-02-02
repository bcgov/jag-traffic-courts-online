@echo off
SETLOCAL
SET ENV=%1

oc project 0198bb-%ENV%
IF ERRORLEVEL 1 GOTO done
@echo "Running helm upgrade"
helm upgrade traffic-court-online traffic-court-online --install --values traffic-court-%ENV%-values.yaml

:done
