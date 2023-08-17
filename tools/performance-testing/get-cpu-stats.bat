@echo off

REM This script will retrieve the CPU specifications of the azure-cognitive-service-custom-layout instance running in DEV, TEST, or PROD.
REM Note: before calling this script, one must first login to OpenShift on the command line, ie, oc login https://console.pathfinder.gov.bc.ca:8443 --token=********
REM
REM Note: Running a command remotely in OpenShift does not currently work. Instead, this script will log you in and output the command you must run manually.
REM 
REM Usage: 
REM   get-cpu-stats.bat (dev|test|prod)
REM     then run the command: grep 'model name' /proc/cpuinfo | uniq

if "%~1"=="" (
	echo  "Usage:"
	echo  "  get-cpu-stats.bat (dev|test|prod)"
	echo  "Example:"
	echo  "  get-cpu-stats.bat dev"
	echo  "  get-cpu-stats.bat prod"
	goto :eof
)
set env=%1%
set server=0198bb-%env%
set cmd="grep 'model name' /proc/cpuinfo | uniq"

REM Find podname
for /F %%i in ('oc get pods -n %server% -o "custom-columns=POD:.metadata.name" --no-headers --selector "app.kubernetes.io/instance=azure-cognitive-service-custom-layout"') do set podname=%%i

echo oc -n %server% rsh %podname%
echo run this: %cmd%

oc -n %server% rsh %podname%
