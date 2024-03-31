helm upgrade --install jaeger jaeger-aio --values values-dev.yaml

helm upgrade --install jaeger jaeger-aio --values values-test.yaml

helm upgrade --install jaeger jaeger-aio --values values-prod.yaml


Scale to Zero: `helm upgrade --install jaeger jaeger-aio --values values-prod.yaml --set replicaCount=0`
