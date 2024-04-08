
# Dev

Note: in dev, we run single instance Redis, not cluster like in test/prod. This is mainly to 
be resource friendly on the cluster.

oc project 0198bb-dev
helm upgrade --install redis redis-18.1.2.tgz  --values values-dev.yaml

# Test

oc project 0198bb-test
helm install redis redis-cluster-10.0.1.tgz --values values-test.yaml

Note: on `upgrade` you will need to supply the redis password. 

helm upgrade redis redis-cluster-10.0.1.tgz --values values-test.yaml --set password=redis-password-from-redis-secret

# Prod

oc project 0198bb-prod
helm install redis redis-cluster-10.0.1.tgz --values values-prod.yaml

Note: on `upgrade` you will need to supply the redis password. 

helm upgrade redis redis-cluster-10.0.1.tgz --values values-prod.yaml --set password=redis-password-from-redis-secret



# Notes:



WARNING: There are "resources" sections in the chart not set. Using "resourcesPreset" is not recommended for production. For production installations, please set the following values according to your workload needs:
  - metrics.resources
  - redis.resources
  - updateJob.resources


Service Account: default

If service account created, then it needs to be granted to pull from tools

Failed to pull image "image-registry.openshift-image-registry.svc:5000/0198bb-tools/redis-exporter:1.58.0-debian-12-r3": rpc error: code = Unknown desc = reading manifest 1.58.0-debian-12-r3 in image-registry.openshift-image-registry.svc:5000/0198bb-tools/redis-exporter: authentication required