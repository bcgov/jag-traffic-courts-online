# Helm Charts

## Testing Traffic Court Helm Charts

`helm template traffic-court-online --values traffic-court-dev-values.yaml`

## Keycloak

Install Keycloak with Helm. Note the value

`helm install oidc bitnami/keycloak --values keycloak-values.yaml`

Add route

`oc apply -f infrastructure/openshift/dev/oidc-route.yaml`

## RabbitMQ

## Redis
