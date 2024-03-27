

```powershell
helm upgrade --install rabbitmq rabbitmq-13.0.0.tgz `
   --values values-prod.yaml
```

when testing, you can use `--set persistentVolumeClaimRetentionPolicy.whenDeleted=Delete` to delete the PVC when deleting deployment

Scale to One. Scaling from helm chart is not possible due to this error:
 
 UPGRADE FAILED: cannot patch "rabbitmq" with kind StatefulSet: StatefulSet.apps "rabbitmq" is invalid: spec: Forbidden: updates to statefulset spec for fields other than 'replicas', 'ordinals', 'template', 'updateStrategy', 'persistentVolumeClaimRetentionPolicy' and 'minReadySeconds' are forbidden

```powershell
kubectl scale statefulsets rabbitmq --replicas=1
```

WARNING: There are "resources" sections in the chart not set. Using "resourcesPreset" is not recommended for production. For production installations, please set the following values according to your workload needs:
  - resources
  