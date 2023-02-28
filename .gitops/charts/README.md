# Helm Charts

## Install Helm on your computer

See [Installing Helm](https://helm.sh/docs/intro/install/)

## Testing Traffic Court Helm Charts

`helm template traffic-court-online --values traffic-court-dev-values.yaml`


## Postgres

```
helm install postgres spilo-0.3.0.tgz --values spilo-postgres-values.yaml
```

## Keycloak

Generate a random postgres database password for keycloak. Be sure to generate new passwords
for each environment.

```
tr -dc A-Za-z0-9 </dev/urandom | head -c 32 ; echo ''
ujk7CnGm5U6JLppkW7WXmVfPj6de3fUY
```

Create a database user. Connect to the leader postgres instance terminal,

```
psql
CREATE USER keycloak WITH PASSWORD 'ujk7CnGm5U6JLppkW7WXmVfPj6de3fUY';
CREATE DATABASE keycloak;
ALTER DATABASE keycloak OWNER TO keycloak;
```

Create a secret for keycloak deployment, note, the secret name `keycloak-postgres` is arbitrary and must match Helm chart value `externalDatabase.existingSecret`

```
kubectl create -n 0198bb-dev secret generic keycloak-postgres --from-literal=password=ujk7CnGm5U6JLppkW7WXmVfPj6de3fUY
```

Deploy Keycloak

```
helm install keycloak keycloak-13.1.1.tgz --values keycloak-values.yaml
```

## Common Object Management Service

To package the COMS helm chart to a tgz, clone the repository into a **temporary** directory
and run the `helm package` command.

```
git clone https://github.com/bcgov/common-object-management-service.git
helm package --dependency-update common-object-management-service/charts/coms/

Successfully packaged chart and saved it to: common-object-management-service-0.0.8.tgz
```

Generate a random postgres database password for keycloak. Be sure to generate new passwords
for each environment.

```
tr -dc A-Za-z0-9 </dev/urandom | head -c 32 ; echo ''
EzkVJNlFdVCvKsEnd8IU8QFdcBF7237j
```

Create a database user. Connect to the leader postgres instance termial,

```
psql
CREATE USER coms WITH PASSWORD 'EzkVJNlFdVCvKsEnd8IU8QFdcBF7237j';
CREATE DATABASE coms;
ALTER DATABASE coms OWNER TO coms;
```



## Pull Charts

```
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update
helm pull bitnami/keycloak
helm pull bitnami/rabbitmq
helm pull bitnami/redis-cluster
```

### Installation

#### Prerequisites

Before installing the default configuration, there are some prerequisites that need to be deployed manually.
A secret must be created to hold the `Access Key` and `Secret Key`. The name of this secret depends if you
are using release scoped configuration or not. The default is not release scoped.

| Release Scoped | Secret Name |
| --- | --- | 
| No | *name*-objectstorage |
| Yes | *fullname*-objectstorage |

Where:
* *name* is chart name or nameOverride value.
* *fullname* is fullnameOverride value, or if not set, the chart name or nameOverride value

The chart name is 'common-object-management-service'.

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: common-object-management-service-objectstorage
data:
  username: ACCESSKEYID
  password: SECRETACCESSKEY
```

#### Deploy

To install from `tgz`, use the regular helm install/upgrade command and specify the chart using local path.
You will need to supply the correct values file for your environemtn.

```
helm upgrade coms ./common-object-management-service-0.0.8.tgz --install --values values-file.yaml
```

A PowerShell script is available to deply,

```powershell
.\deploy-coms.ps1 dev
```

## Keycloak

Install Keycloak with Helm. Note the value

`helm install oidc bitnami/keycloak --values keycloak-values.yaml`

Add route

`oc apply -f infrastructure/openshift/dev/oidc-route.yaml`

## RabbitMQ

## Redis
