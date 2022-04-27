
## Parameters

### Citizen API (citizen-api)

| Name                      | Description                                              | Value  |
| ------------------------- | -------------------------------------------------------- | ------ |
| citizen-api.enabled       |                                                          | `true` |
| citizen-api.replicaCount     |                                                          | `1`  |
| citizen-api.image.tag     |                                                          |        |
| citizen-api.formRecognizer.existingSecret | Existing secret for FormRecognizer. (must contain a value for `form-recognizer-endpoint` and `form-recognizer-api-key`) 
| citizen-api.jaeger.enabled | | `true` |
| citizen-api.jaeger.exporter.endpoint | | `jaeger` |
| citizen-api.jaeger.exporter.protocol | | `http/thrift.binary` |
| citizen-api.rabbitmq.existingSecret | Existing secret for RabbitMQ (must contain a value for `rabbitmq-username` and `rabbitmq-password`) | |
| citizen-api.objectStorage.existingSecret | Existing secret for ObjectStorage. (must contain a value for `object-storage-access-key`, `object-storage-bucket-name`, `object-storage-endpoint` and `object-storage-secret-key` keys) | |
| citizen-api.splunk.enabled | | `true` |
| citizen-api.splunk.existingSecret | Existing secret for Splunk (must contain a value for `splunk-hec-endpoint` and `splunk-hec-token`) | |
| citizen-api.swagger.enabled | | `false` |
| citizen-api.ticket-search.existingSecret | Existing secret for Ticket Search (must contain a value for `ticket-search-endpoint` and `ticket-search-secure`) | |

### Citizen Web Site (citizen-web)

| Name                      | Description                                              | Value  |
| ------------------------- | -------------------------------------------------------- | ------ |
| citizen-web.enabled       |                                                          | `true` |
| citizen-web.image.tag     |                                                          |        |

### Oracle Data API (oracle-data-api)

| Name                      | Description                                              | Value |
| ------------------------- | -------------------------------------------------------- | ----- |
| oracle-data-api.enabled       |                                                          | `true` |
| oracle-data-api.image.tag     |                                                          |        |

### StaffAPI (staff-api)

| Name                      | Description                                              | Value |
| ------------------------- | -------------------------------------------------------- | ----- |
| staff-api.enabled       |                                                          | `true` |
| staff-api.image.tag     |                                                          |        |
| staff-api.jaeger.enabled | | `true` |
| staff-api.jaeger.exporter.endpoint | | `jaeger` |
| staff-api.jaeger.exporter.protocol | | `http/thrift.binary` |

### Staff Web Site (staff-web)

| Name                      | Description                                              | Value |
| ------------------------- | -------------------------------------------------------- | ----- |
| staff-web.enabled       |                                                          | `true` |
| staff-web.image.tag     |                                                          |        |

### Ticket Search (ticket-search)

| Name                      | Description                                              | Value |
| ------------------------- | -------------------------------------------------------- | ----- |
| ticket-search.enabled       |                                                          | `true` |
| ticket-search.image.tag     |                                                          |        |
| ticket-search.jaeger.enabled | | `true` |
| ticket-search.jaeger.exporter.endpoint | | `jaeger` |
| ticket-search.jaeger.exporter.protocol | | `http/thrift.binary` |
