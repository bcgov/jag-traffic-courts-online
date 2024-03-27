

helm upgrade --install rabbitmq rabbitmq-13.0.0.tgz --values values-prod.yaml


when testing, you can use

--set persistentVolumeClaimRetentionPolicy.whenDeleted=Delete


WARNING: There are "resources" sections in the chart not set. Using "resourcesPreset" is not recommended for production. For production installations, please set the following values according to your workload needs:
  - resources
  