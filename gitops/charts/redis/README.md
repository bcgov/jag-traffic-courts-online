
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


# Resize PVC

```
helm upgrade redis redis-cluster-10.0.1.tgz --values values-prod.yaml --set password=5zFRG1rhuu
Error: UPGRADE FAILED: cannot patch "redis" with kind StatefulSet: StatefulSet.apps "redis" is invalid: spec: Forbidden: updates to statefulset spec for fields other than 'replicas', 'ordinals', 'template', 'updateStrategy', 'persistentVolumeClaimRetentionPolicy' and 'minReadySeconds' are forbidden
```

```powershell
$newsize='{\"spec\":{\"resources\":{\"requests\":{\"storage\":\"1Gi\"}}}}'
kubectl patch pvc redis-data-redis-0 --namespace 0198bb-prod --type merge --patch $newsize
kubectl patch pvc redis-data-redis-1 --namespace 0198bb-prod --type merge --patch $newsize
kubectl patch pvc redis-data-redis-2 --namespace 0198bb-prod --type merge --patch $newsize
kubectl patch pvc redis-data-redis-3 --namespace 0198bb-prod --type merge --patch $newsize
kubectl patch pvc redis-data-redis-4 --namespace 0198bb-prod --type merge --patch $newsize
kubectl patch pvc redis-data-redis-5 --namespace 0198bb-prod --type merge --patch $newsize
```
