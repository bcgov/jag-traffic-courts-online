# Introduction
This document is here to guide the reader in setting up the Traffic Courts Online application on OpenShift. It assumes the project will be deployed on the BC Government OpenShift platform. This document also provides the steps requires to deploy an equivalent environment on your local development machine using CodeReady Containers (CRC).  If you are testing or deploying to CRC, be sure to follow the instructions on setting up CRC to mimic the default  BC Government OpenShift environemnt.

This document will focus primarily on the command line interfaces as opposed to the web console. 

All commands assume the current working directory is the `devops` directory. 

# Prerequisites

## Knowledge Prerequisites
This guide assumes the reader is familar with Kubernetes and OpenShift.  The reader is not expected to be an expert in Kubernetes or OpenShift.

## Software Prerequisites

The following software will be required to be installed to following sections.

### OpenShift Command Line Tools

The OpenShift Command Line Tools can be downloaded from the OpenShift web console.  ```https://console.apps.silver.devops.gov.bc.ca/command-line-tools``` or on CRC, ```https://console-openshift-console.apps-crc.testing/command-line-tools```

### Kubernetes command-line tool

The Kubernetes command-line tool, kubectl, allows you to run commands against Kubernetes clusters.  The instructions to install for various platforms can be found at the offical [Kubernetes Tools Istall](https://console-openshift-console.apps-crc.testing/command-line-tools) page.  You will need to install the ```kubectl``` tool.

### [jq](https://stedolan.github.io/jq/)

jq is like sed for JSON data - you can use it to slice and filter and map and transform structured data with the same ease that sed, awk, grep and friends let you play with text.

jq will make it easier to process the output of command line tools in an automated fashio.

# Local development with OpenShift

CodeReady Containers is the quickest way to get started building OpenShift clusters. It is designed to run on a local computer to simplify setup and testing, and emulate the cloud development environment locally with all of the tools needed to develop container-based applications. 

## Install

To create a minimal cluster on your desktop/laptop for local development and testing, follow the instructions on [Create an OpenShift cluster](https://cloud.redhat.com/openshift/create/local).

## Setup Projects

To mirror the BC Government OpenShift project conventions, create the equivalent `tools`, `dev`, `test` and `prod` namespaces in your local deployment.

### Unix
```bash
oc login --username=developer --password=developer
for SUFFIX in tools dev test prod; do echo oc new-project local-${SUFFIX}; done;
```
### Windows
```cmd
oc login --username=developer --password=developer
@FOR %i IN (tools dev test prod) DO oc new-project local-%i
```

## 

Create the local resources. These commands will create the default network policy, ```platform-services-controlled-deny-by-default``` created in each of your namespaces.
```
oc kustomize local/tools | oc apply -f -
oc kustomize local/dev | oc apply -f -
oc kustomize local/test | oc apply -f -
oc kustomize local/prod | oc apply -f -
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

NAMESPACE=local-tools
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
for SUFFIX in tools dev test prod; do echo oc delete project local-${SUFFIX}; done;
```
### Windows
```cmd
oc login --username=developer --password=developer
@FOR %i IN (tools dev test prod) DO oc delete project local-%i
```
