
# Assumptions
- You have access to an OpenShift namespace to deploy to
- You have access to Vault to manage secrets - https://vault.developer.gov.bc.ca/
- You have access to the Common Hosted Single Sign-on SSO Requests system
- You have access to Sysdig - https://app.sysdigcloud.com/
- You have access to an Azure Subscription authorized to run Azure Form Recognizer on premise

# Prerequisites
- OpenShift Container Platform CLI, see [Installing the OpenShift CLI](https://docs.openshift.com/container-platform/4.13/cli_reference/openshift_cli/getting-started-cli.html#installing-openshift-cli)

- A local configured copy of kubectl

## Helm
### Install Helm

Helm is a tool for managing Kubernetes charts. Charts are packages of pre-configured Kubernetes resources.

To install Helm, refer to the [Helm install guide](https://github.com/helm/helm#install) and ensure that the `helm` binary is in the `PATH` of your shell.

# Setup

This project has standardized on using Helm charts for deployment. 

## Setup Image Pull Secret

In the `tools` namespace, ensure Docker Hub [image pull secret](https://docs.openshift.com/container-platform/4.12/openshift_images/managing_images/using-image-pull-secrets.html) 
has been configured. The image streams below expect an image pull secret to be configured with the name `pipeline-docker-hub-pull`. 
The pull secret requires an docker account and the configuration of a personal access token. See [Create and manage access tokens](https://docs.docker.com/docker-hub/access-tokens/).

1. Setup a personal access token using the documenation
1. Use the docker cli to login to docker hub using your PAT
1. Create the secret using the generated configuration file, for example

```bash
oc create secret generic pipeline-docker-hub-pull \
  -from-file=.dockerconfigjson=<path/to/.docker/config.json> \
  --type=kubernetes.io/dockerconfigjson
  -n 0198bb-tools
```

## Setup Image Streams

The `tools` namespace contains the image stream and image stream tags used in otheur deployments. The manifests for configuring 
the image streams are found in the folder 'infrastructure/openshift/tools/image-streams/'. For each component deployed, ensure 
the image stream and tag exists. During deployment be sure to specific they correct `image.repository` value.
Regardless of the OpenShift you deploy to, the image registry for the internal image stream will always be  `registry.openshift-image-registry.svc:5000`.

1. Create image streams
1. Create a role binding allowing any service account from the dev, test or prod namespaces to pull images.

[0198bb:image-pullers](/infrastructure/openshift\tools/role-bindings/0198bb-image-pullers.yaml) role binding.

```
oc apply -n 0198bb-tools -f ./infrastructure/openshift/tools/image-streams/
oc apply -n 0198bb-tools -f ./infrastructure/openshift/tools/role-bindings/0198bb-image-pullers.yaml
```

## Get uid-range and supplemental-groups

Get the uid-range and supplemental-groups for your namespace, `oc get ns your-namespace -o yaml`. 
These values will be different for each namespace. The uid-range and supplemental-groups are 
normally fall into the same range in namespace. The `uid-range` value is used for `runAsUser`.
The `supplemental-groups` value is used for `fsGroup` if required.

```yaml
apiVersion: v1
kind: Namespace
metadata:
  annotations:
    openshift.io/sa.scc.supplemental-groups: 1000650000/10000
    openshift.io/sa.scc.uid-range: 1000650000/10000
```

In the example above, the valid values for `runAsUser` or `fsGroup` would be `1000650000` - `1000659999`.
You can use the same value for all the deployments.

# Deployment

## Redis

In `dev` and `test` environments, a single Redis node can be used. In `production`, a Redis Cluster must be used for high availability.

https://hub.docker.com/r/bitnamicharts/redis
https://hub.docker.com/r/bitnamicharts/redis-cluster

1. Pull helm chart, `helm pull oci://registry-1.docker.io/bitnamicharts/redis-cluster`
1. Create a values file `redis-cluster-values.yaml` using the correct values for `runAsUser` and `fsGroup` from above.  See the default [values.yaml](https://github.com/bitnami/charts/blob/main/bitnami/redis-cluster/values.yaml) for reference.
1. Install the helm chart. Note in the command below, we override the name so the services otherwise the service would have name `deployment-redis-cluster`.

```
helm install redis redis-cluster-8.6.2.tgz --values redis-cluster-values.yaml --set nameOverride=redis
```

You can the connect to redis.namespace.svc.cluster.local on port 6379. The password will be generated into secret `redis` in the `redis-password` key.

## RabbitMQ

In `dev` and `test` environments, a single RabbitMQ node can be used. In `production`, a RabbitMQ cluster must be used for high availability.  The RabbitMQ chart will need to create a role binding, allowing the service account to create events and get endpoints. If you do not have permission to create the role and role binding, it can be created a before deployment and skipped as part of the helm chart by setting `rbac.create=false`

1. Ensure the service account named used, defaults to `rabbitmq`, has permissions to pull images from the tools namespace
1. Pull helm chart, `helm pull oci://registry-1.docker.io/bitnamicharts/rabbitmq`
1. Create a values file `rabbitmq-values.yaml` using the correct values for `runAsUser` and `fsGroup` from above.  See the default [values.yaml](https://github.com/bitnami/charts/blob/main/bitnami/rabbitmq/values.yaml) for reference.
1. Install the helm chart.

```
helm install rabbitmq rabbitmq-12.0.2.tgz --values rabbitmq-values.yaml
```

RabbitMQ can be connected to at rabbitmq.namespace.svc.cluster.local on port 5672 (amqp). The password will be generated into secret `rabbitmq` in the `rabbitmq-password` key. The username is `user`.

## Postgres

In `dev` and `test` environments, a single Postgres server can be used. In `production`, a Postgres [patroni](https://github.com/zalando/patroni) cluster must be used for high availability.

1. Deploy 


## Common Object Management Service (COMS)

TODO: create a proper Helm chart to deploy the deployment

COMS does not have a helm chart we can use. The one in the source project wants to deployed postgres at the same time. We have manifests at
`infrastructure\openshift\common-object-management-service` that can be applied.

## ClamAV

[ClamAV®](https://www.clamav.net/) is an open-source antivirus engine for detecting trojans, viruses, malware & other malicious threats.
The ClamAV image is build from source using the [bvgov/clamav](https://github.com/bcgov/clamav) Github repository.

1. Create the [](openshift/tools/build-configs/clamav-build-bcgov.yaml)
    ```
    oc apply -n 0198bb-tools -f openshift/tools/build-configs/clamav-build-bcgov.yaml
    ```
1. Start a build to build the image stream tag
    ```
    oc start-build -n 0198bb-tools clamav-build-bcgov
    ```
1. Verify the build completed successfully
    ```
    oc get build -n 0198bb-tools clamav-build-bcgov-1
    ```
   The status column should say `Complete`.
1. Deploy ClamAV

 TODO:

## Virus Scan API

The Virus Scan API provides an API to interact with ClamAV. The Virus Scan API image is build from source 
using this repository. 

1. Create the [](openshift/tools/build-configs/virus-scan-api-build.yaml)
    ```
    oc apply -n 0198bb-tools -f openshift/tools/build-configs/virus-scan-api-build.yaml
    ```
1. Start a build to build the image stream tag
    ```
    oc start-build -n 0198bb-tools virus-scan-api-build
    ```
1. Verify the build completed successfully
    ```
    oc get build -n 0198bb-tools virus-scan-api-build-1
    ```
   The status column should say `Complete`.
1. Deploy Virus Scan API, from the charts subdirectory,
    ```
    oc project 0198bb-dev
    helm upgrade virus-scan virus-scan --install --values virus-scan-dev-values.yaml
    ```
   or run the equivalent Powershell script,
    ```
    .\deploy-virus-scan.ps1 dev
    ```

## Jaeger

From the charts subdirectory,

```
oc project 0198bb-dev
helm upgrade jaeger jaeger-aio --install --values jaeger-values.yaml
```

or run the equivalent Powershell script,

```
.\deploy-jaeger-aio.ps1 dev
```

## Keycloak

Deploying keycloak depends on a posgres database. Generate a random postgres database password for keycloak. Be sure to generate new passwords
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
1. Pull helm chart, `helm pull oci://registry-1.docker.io/bitnamicharts/keycloak`
1. Install the helm chart

```
helm install keycloak keycloak-15.1.3.tgz --values keycloak-values.yaml
```

# Application Services 

## Azure Form Recognizer on premise

## Traffic Court Online