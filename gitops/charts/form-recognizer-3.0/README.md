https://github.com/MicrosoftDocs/azure-docs/blob/main/articles/ai-services/document-intelligence/containers/image-tags.md


https://learn.microsoft.com/en-us/azure/ai-services/document-intelligence/containers/install-run?view=doc-intel-3.0.0&preserve-view=true&tabs=read

# To drop and rerun the OCR init job to re-initalize the PVC in the event of a 
# Azure.RequestFailedException: A transient error has occurred. Please try again. Status: 503 (Service Unavailable) ErrorCode: ServiceUnavailable
oc -n 0198bb-test delete job acs-form-recognizer-init 
helm template test-release . --show-only templates/job-init.yaml | oc -n 0198bb-test apply -f -