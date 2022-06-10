@echo off

REM This script will port-forward jaeger in DEV, TEST, or PROD so it can be accessed locally.
REM Note: before calling this script, one must first login to OpenShift on the command line, ie, oc login https://console.pathfinder.gov.bc.ca:8443 --token=********

REM Usage: 
REM   port-forward-jaeger (tools|dev|test|prod)

if "%~1"=="" (
	echo  "Usage:"
	echo  "  port-forward-jaeger.bat (tools|dev|test|prod)"
	echo  "Example:"
	echo  "  port-forward-jaeger.bat dev"
	echo  "  port-forward-jaeger.bat test"
	goto :eof
)

set env=%1%
set server=0198bb-%env%
set port=16686

REM find podname
for /F %%i in ('oc get pods -n %server% -o "custom-columns=POD:.metadata.name" --no-headers --selector "app.kubernetes.io/instance=jaeger-%env%"') do set podname=%%i

REM run port-forward OpenShift command
echo oc -n %server% port-forward %podname% %port%:16686
echo.
echo Access jaeger here: http://localhost:16686/
echo.
oc -n %server% port-forward %podname% %port%:16686
