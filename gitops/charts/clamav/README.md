Chart from: https://github.com/pbolduc/clamav/releases/tag/1.0.5

Depends on image built in tools image stream, see [clamav-build-bcgov.yaml](../../openshift/tools/build-configs/clamav-build-bcgov.yaml)

# Production (multiple instances)
helm upgrade --install clamav clamav-1.0.0.tgz --values prod.yaml

# Dev/Test (single instances)

helm upgrade --install clamav clamav-1.0.0.tgz --values values.yaml

