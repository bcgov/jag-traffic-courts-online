# Postgres Database



```
===========================================================
= WARNING WARNING WARNING WARNING WARNING WARNING WARNING =
===========================================================
=                                                         =
= Do NOT uninstall the CrunchyData Helm chart.            =
=                                                         =
= Uninstalling the CrunchyData Helm will delete all data  =
= and backup PVC! You must ensure you have backups stored =
= outside of the Helm chart managed volumes!              =
=
===========================================================
= WARNING WARNING WARNING WARNING WARNING WARNING WARNING =
===========================================================
```

Chart Source: https://github.com/CrunchyData/postgres-operator-examples/tree/main/helm/postgres

In `dev`, we use a single Postgres cluster to conserve resources. In `dev` and `test` we split
keycloak from the coms and masstransit databases. This allows the ability to scale the coms and
masstransit databases separately. It also allows to have different backup schedule.

Note: as of Oct 30, 2023, the Silver Cluster has PGO operator 5.2. The helm charts have not changed and it is safe to use chart 5.4.3.


## Dev Environment

In the development environment, the all databases are deployed to the same cluster to minimize resource usage.

### Databases

```powershell
# PowerShell
kubectl create -f postgres-dev-bootstrap-sql-configmap.yaml
helm install postgres-dev postgrescluster-5.4.3.tgz `
     --values postgres-dev-values.yaml `
     --set databaseInitSQL.name=bootstrap-sql `
     --set databaseInitSQL.key=bootstrap.sql
```

After database has been initialized, remove the bootstrap config map and patch the deployment removing the databaseInitSQL

```powershell
# PowerShell
kubectl delete -f postgres-dev-bootstrap-sql-configmap.yaml
helm upgrade postgres-dev postgrescluster-5.4.3.tgz `
     --values postgres-dev-values.yaml
```

Note: If you testing in another environment like OpenShift local, you may want to override the storage class by resetting to the default storage class like:

```powershell
# PowerShell
kubectl create -f postgres-dev-bootstrap-sql-configmap.yaml
helm install postgres postgrescluster-5.4.3.tgz `
     --values postgres-dev-values.yaml `
     --set databaseInitSQL.name=bootstrap-sql `
     --set databaseInitSQL.key=bootstrap.sql `
     --set instances[0].dataVolumeClaimSpec.storageClassName=default `
     --set pgBackRestConfig.repos[0].volume.volumeClaimSpec.storageClassName=default
```


## Test Environment

In the test and production environment, the `keycloak` and other databases are deployed to separate clusters.

### Keycloak Database

Create the Keycloak database initalization config map and create the cluster.

```powershell
# PowerShell
kubectl create -f postgres-test-keycloak-bootstrap-sql-configmap.yaml
helm install postgres-keycloak postgrescluster-5.4.3.tgz `
     --values postgres-test-keycloak-values.yaml `
     --set databaseInitSQL.name=keycloak-bootstrap-sql `
     --set databaseInitSQL.key=bootstrap.sql
```

After database has been initialized, remove the bootstrap config map and patch the deployment removing the databaseInitSQL

```powershell
# PowerShell
kubectl delete -f postgres-test-keycloak-bootstrap-sql-configmap.yaml
helm upgrade postgres-keycloak postgrescluster-5.4.3.tgz `
     --values postgres-test-keycloak-values.yaml
```


### Application and COMS Databases

Create the application database initalization config map and create the cluster.

```powershell
# PowerShell
kubectl create -f postgres-test-application-bootstrap-sql-configmap.yaml
helm upgrade postgres-app postgrescluster-5.4.3.tgz `
     --values postgres-test-application-values.yaml `
     --set databaseInitSQL.name=application-bootstrap-sql `
     --set databaseInitSQL.key=bootstrap.sql
```

After database has been initialized, remove the bootstrap config map and patch the deployment removing the databaseInitSQL

```powershell
# PowerShell
kubectl delete -f postgres-test-application-bootstrap-sql-configmap.yaml
helm upgrade postgres-app postgrescluster-5.4.3.tgz `
     --values .\postgres-test-application-values.yaml
```

