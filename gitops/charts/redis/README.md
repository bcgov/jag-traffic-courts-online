

oc project 0198bb-prod
helm upgrade --install redis redis-cluster-10.0.0.tgz --values values-prod.yaml


WARNING: There are "resources" sections in the chart not set. Using "resourcesPreset" is not recommended for production. For production installations, please set the following values according to your workload needs:
  - metrics.resources
  - redis.resources
  - updateJob.resources


Service Account:

Failed to pull image "image-registry.openshift-image-registry.svc:5000/0198bb-tools/redis-exporter:1.58.0-debian-12-r3": rpc error: code = Unknown desc = reading manifest 1.58.0-debian-12-r3 in image-registry.openshift-image-registry.svc:5000/0198bb-tools/redis-exporter: authentication required