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

### [jq](https://stedolan.github.io/jq/)

jq is like sed for JSON data - you can use it to slice and filter and map and transform structured data with the same ease that sed, awk, grep and friends let you play with text.

jq will make it easier to process the output of command line tools in an automated fashio.

## Local Development

See [CodeReady Container](env/code-ready-containers/README.md) instructions for running OpenShift locally and testing.