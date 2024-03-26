helm upgrade --install jaeger jaeger-1.11.2.tgz --values values-prod.yaml


WARNING: There are "resources" sections in the chart not set. Using "resourcesPreset" is not recommended for production. For production installations, please set the following values according to your workload needs:
  - agent.resources
  - collector.resources
  - migration.resources
  - query.resources

  