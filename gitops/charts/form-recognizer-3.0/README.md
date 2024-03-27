https://github.com/MicrosoftDocs/azure-docs/blob/main/articles/ai-services/document-intelligence/containers/image-tags.md


https://learn.microsoft.com/en-us/azure/ai-services/document-intelligence/containers/install-run?view=doc-intel-3.0.0&preserve-view=true&tabs=read

# Support
To drop and rerun the OCR init job to re-initalize the PVC in the event of a Azure.RequestFailedException: A transient error has occurred. Please try again. Status: 503 (Service Unavailable) ErrorCode: ServiceUnavailable

```shell
oc -n 0198bb-test delete job acs-form-recognizer-init 
helm template test-release . --show-only templates/job-init.yaml | oc -n 0198bb-test apply -f -
```

# Deployment tasks:
```shell
# deploy PVC and init storage
helm template release . --show-only templates/pvc-logs.yaml --values ./values-prod.yaml | oc -n 0198bb-prod apply -f -
helm template release . --show-only templates/pvc-shared.yaml --values ./values-prod.yaml | oc -n 0198bb-prod apply -f -
helm template release . --show-only templates/job-init.yaml --values ./values-prod.yaml | oc -n 0198bb-prod apply -f -

# deploy services
helm template release . --show-only templates/service-layout.yaml --values ./values-prod.yaml | oc -n 0198bb-prod apply -f -
helm template release . --show-only templates/service-custom-template.yaml --values ./values-prod.yaml | oc -n 0198bb-prod apply -f -
helm template release . --show-only templates/service-proxy.yaml --values ./values-prod.yaml | oc -n 0198bb-prod apply -f -

# deploy config
helm template release . --show-only templates/configmap-proxy-config.yaml --values ./values-prod.yaml | oc -n 0198bb-prod apply -f -

# deploy applications
helm template release . --show-only templates/deployment-layout.yaml --values ./values-prod.yaml | oc -n 0198bb-prod apply -f -
helm template release . --show-only templates/deployment-custom-template.yaml --values ./values-prod.yaml | oc -n 0198bb-prod apply -f -
helm template release . --show-only templates/deployment-proxy.yaml --values ./values-prod.yaml | oc -n 0198bb-prod apply -f -

# deploy cleanup cronjobs
helm template release . --show-only templates/cronjobs.yaml --values ./values-prod.yaml | oc -n 0198bb-prod apply -f -
```