### Maintenance Site (maintenance-site)

| Name                      | Description                                              | Value |
| ------------------------- | -------------------------------------------------------- | ----- |
| maintenance-site.replicaCount     |                                                          | `1`  |
| maintenance-site.image.tag     |                                                          |        |

helm upgrade --install maintenance maintenance-site --values values-dev.yaml

helm upgrade --install maintenance maintenance-site --values values-test.yaml

helm upgrade --install maintenance maintenance-site --values values-prod.yaml


Scale Dev to Zero: `helm upgrade --install maintenance maintenance-site --values values-dev.yaml --set replicaCount=0`
Scale Test to Zero: `helm upgrade --install maintenance maintenance-site --values values-test.yaml --set replicaCount=0`
Scale Prod to Zero: `helm upgrade --install maintenance maintenance-site --values values-prod.yaml --set replicaCount=0`
