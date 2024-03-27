
One replica, no Pod Disruption Budget

```powershell
helm upgrade --install keycloak keycloak-19.3.4.tgz `
  --values keycloak-prod-values.yaml
```

Two replica with a Pod Disruption Budget (minAvailable 1)

```powershell
helm upgrade --install keycloak keycloak-19.3.4.tgz `
  --values keycloak-prod-values.yaml `
  --set replicaCount=2 `
  --set pdb.create=true
```

Scale to Zero, no Pod Disruption Budget

```powershell
helm upgrade --install keycloak keycloak-19.3.4.tgz `
  --values keycloak-prod-values.yaml `
  --set replicaCount=0
```

