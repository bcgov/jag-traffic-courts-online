# Local development with OpenShift

CodeReady Containers is the quickest way to get started building OpenShift clusters. It is designed to run on a local computer to simplify setup and testing, and emulate the cloud development environment locally with all of the tools needed to develop container-based applications. 

## Install

To create a minimal cluster on your desktop/laptop for local development and testing, follow the instructions on [Create an OpenShift cluster](https://cloud.redhat.com/openshift/create/local).

## Setup Projects

To mirror the BC Government OpenShift project conventions, create the equivalent `tools`, `dev`, `test` and `prod` namespaces in your local deployment.
We will use the same namespaces as on the OCP4 environment. By using the same namespaces, we mimimize the amount of customization. 

**Warning**: if you are connecting to multiple clusters, OCP4 and CRC, ensure you know which cluster your commands will be run against. Use `oc project`.

### Unix
```bash
oc login --username=developer --password=developer
for SUFFIX in tools dev test prod; do echo oc new-project 0198bb-${SUFFIX}; done;
```

## 

Create the local resources. These commands will create the default network policy, ```platform-services-controlled-deny-by-default``` created in each of your namespaces.

```
cd devops/env/code-ready-containers
oc kustomize tools | oc apply -f -
oc kustomize dev | oc apply -f -
oc kustomize test | oc apply -f -
oc kustomize prod | oc apply -f -
```

## Build Image Locally

Run this command from the root of the source tree.

```bash
docker build ./src/backend/TrafficCourtsApi -t dispute-api:latest
```



## Tag and push the image to OpenShift

This step will tag the local dispute-api and push it to your OpenShift namespace. This step will

1. Determine the DNS entry of the OpenShift respository/registry.
2. Tag the `dispute-api:latest` image to the target image
3. Get the `builder-token` of the `builder` service account used to authenciate to the respository
4. Login to the registry using the builder token
5. Push the image

**Note**: This is normally done by the continous integration (CI) pipeline.  You should not have to perform this step against the BC Government OpenShift environment

```bash

NAMESPACE=0198bb-tools
SA_USERNAME=builder

REGISTRY=$(oc registry info)
docker tag dispute-api:latest ${REGISTRY}/${NAMESPACE}/dispute-api:latest

SA_SECRET_NAME=$(oc get sa ${SA_USERNAME} -n ${NAMESPACE} -o jsonpath='{.secrets}' | jq -r '.[] | select(.name | startswith("builder-token")).name')
SA_PASSWORD=$(oc get secret ${SA_SECRET_NAME} -n ${NAMESPACE} -o jsonpath='{.data.token}' | base64 -d)

echo ${SA_PASSWORD} | docker login ${REGISTRY} --username ${SA_USERNAME} --password-stdin
docker push ${REGISTRY}/${NAMESPACE}/dispute-api:latest
```





## Cleaning Up

To remove the local projects from CodeReady Containers, run the following command.

### Unix
```bash
oc login --username=developer --password=developer
for SUFFIX in tools dev test prod; do oc delete project 0198bb-${SUFFIX}; done;
```
