
# Deploy Template Commands

This directory contains the OpenShift template for deploying the Citizen Portal.

Note: The template creates a public route based on the `.apps.silver.devops.gov.bc.ca` name. This route should be deleted. In the future,
we can create environment specific routes using a cleaner approach. 

## Dev

```bash
oc process -f infrastructure\openshift\traffic-courts-online-app.yml -n 0198bb-dev -p FRONTEND_URL=tickets -p OC_ENV=dev | oc apply -f -
```

The Citizen Portal is available at `https://tickets-0198bb-dev.apps.silver.devops.gov.bc.ca/`

## Test

```bash
oc process -f infrastructure\openshift\traffic-courts-online-app.yml -n 0198bb-test -p FRONTEND_URL=tickets -p OC_ENV=test | oc apply -f -      
oc delete route citizen-web
oc create -f tickets-test.gov.bc.ca-route.yaml
```

## Prod

```bash
oc process -f infrastructure\openshift\traffic-courts-online-app.yml -n 0198bb-prod -p FRONTEND_URL=tickets -p OC_ENV=prod | oc apply -f -
oc delete route citizen-web
oc create -f tickets.gov.bc.ca-route.yaml
```
