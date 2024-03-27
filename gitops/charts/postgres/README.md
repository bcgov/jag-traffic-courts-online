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
helm install postgres-app postgrescluster-5.4.3.tgz `
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


## Production Environment

The production environment is the same as test, except using the `prod` bootstrap and values files.

## Keycloak Database

Create the Keycloak database initalization config map and create the cluster.

Ensure your project is set correct `oc project 0198bb-prod`

```powershell
# PowerShell
kubectl create -f postgres-prod-keycloak-bootstrap-sql-configmap.yaml
helm install postgres-keycloak postgrescluster-5.4.3.tgz `
     --values postgres-prod-keycloak-values.yaml `
     --set databaseInitSQL.name=keycloak-bootstrap-sql `
     --set databaseInitSQL.key=bootstrap.sql
```

After database has been initialized, remove the bootstrap config map and patch the deployment removing the databaseInitSQL

```powershell
# PowerShell
kubectl delete -f postgres-prod-keycloak-bootstrap-sql-configmap.yaml
helm upgrade postgres-keycloak postgrescluster-5.4.3.tgz `
     --values postgres-prod-keycloak-values.yaml
```


Scale to One: 

```powershell
# PowerShell
helm upgrade postgres-keycloak postgrescluster-5.4.3.tgz `
     --values postgres-prod-keycloak-values.yaml `
     --set instances[0].replicas=1
```

### Application and COMS Databases

Ensure your project is set correct `oc project 0198bb-prod`

Create the application database initalization config map and create the cluster.

```powershell
# PowerShell
kubectl create -f postgres-prod-application-bootstrap-sql-configmap.yaml
helm install postgres-app postgrescluster-5.4.3.tgz `
     --values postgres-prod-application-values.yaml `
     --set databaseInitSQL.name=application-bootstrap-sql `
     --set databaseInitSQL.key=bootstrap.sql
```

After database has been initialized, remove the bootstrap config map and patch the deployment removing the databaseInitSQL

```powershell
# PowerShell
kubectl delete -f postgres-prod-application-bootstrap-sql-configmap.yaml
helm upgrade postgres-app postgrescluster-5.4.3.tgz `
     --values .\postgres-prod-application-values.yaml
```


# S3 Backup

Example: https://github.com/bcgov/fin-pay-transparency/tree/main/charts/fin-pay-transparency/charts/crunchy-postgres

Will have to translate from this example to the crunchydb chart.

```yaml
  dataSource:
    pgbackrest:
      stanza: {{ .Values.instances.name }}
      configuration:
        - secret:
            name: {{ .Release.Name }}-s3-secret
      global:
        repo1-s3-uri-style: path # This is mandatory since the backups are path based.
        repo1-path: {{ .Values.clone.path }} # path to the backup where cluster will bootstrap from
      repo:
        name: repo1 # hardcoded since it is always backed up to object storage.
        s3:
          bucket: {{ .Values.pgBackRest.s3.bucket }}
          endpoint: {{ .Values.pgBackRest.s3.endpoint }}
          region: "ca-central-1"
```