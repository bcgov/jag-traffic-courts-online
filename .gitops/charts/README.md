# Helm Charts

## Install Helm on your computer

See [Installing Helm](https://helm.sh/docs/intro/install/)

## Testing Traffic Court Helm Charts

`helm template traffic-court-online --values traffic-court-dev-values.yaml`

## Common Object Management Service

To package the COMS helm chart to a tgz, clone the repository to a temporary directory
and run the `helm package` command.

```
git clone https://github.com/bcgov/common-object-management-service.git
helm package common-object-management-service/charts/coms/

Successfully packaged chart and saved it to: common-object-management-service-0.0.7.tgz
```

To install from `tgz`, use the regular helm install/upgrade command and specify the chart using local path.

```
helm upgrade --install coms ./common-object-management-service-0.0.7.tgz --values common-object-management-service-values.yaml
```

## Keycloak

Install Keycloak with Helm. Note the value

`helm install oidc bitnami/keycloak --values keycloak-values.yaml`

Add route

`oc apply -f infrastructure/openshift/dev/oidc-route.yaml`

## RabbitMQ

## Redis
