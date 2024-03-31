helm upgrade --install jaeger jaeger-aio --values values-prod.yaml


Scale to Zero: `helm upgrade --install jaeger jaeger-aio --values values-prod.yaml --set replicaCount=0`
