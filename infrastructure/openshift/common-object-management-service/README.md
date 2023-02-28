# Deploy Common Object Management Service

The Common Object Management Service [helm chart](https://github.com/bcgov/common-object-management-service/tree/master/charts/coms)
can deploy the service, however, it does not support using a separate postgres database.

## Prerequisites

These instructions assume you already have a Postgres database deployed.
The DeploymentConfig assumes the postgres database is deployed as service postgres. 
See DB_HOST in deploymentconfig.yaml. The commands below use `bash` line continuation "`\`".
For Powershell, change to back tick.

## Create Database User and Database

Connect to the postgres database and create the user, database and
change the ownership of the database to the coms user.  In the examples below
simple password is used, however, you should create a secure password,
A secure random password can be created using the following command in Linux,

```
tr -dc A-Za-z0-9 </dev/urandom | head -c 32 ; echo ''
```


```
psql
CREATE USER coms WITH PASSWORD 'coms-password';
CREATE DATABASE coms;
ALTER DATABASE coms OWNER TO coms;
\q
```

## Create Secrets

There are three secrets that need to be manually configured,

1. Postgres database connection parameters, `database`, `username` and `password`.

```
kubectl create secret generic common-object-management-service-database \
  --from-literal=app-db-name=coms \
  --from-literal=app-db-username=coms \
  --from-literal=app-db-password=coms-password

oc label secret common-object-management-service-database \
  app=common-object-management-service \
  app.kubernetes.io/part-of=common-object-management-service \
  app.kubernetes.io/instance=common-object-management-service \
  app.kubernetes.io/name=common-object-management-service
```

2. Object Store (S3) credentials, `access key id` and `secret access key`.

```
kubectl create secret generic common-object-management-service-objectstorage \
  --from-literal=username=access-key-id \
  --from-literal=password=secret-access-key

oc label secret common-object-management-service-objectstorage \
  app=common-object-management-service \
  app.kubernetes.io/part-of=common-object-management-service \
  app.kubernetes.io/instance=common-object-management-service \
  app.kubernetes.io/name=common-object-management-service
```

3. Service Basic Authentication credentials, `username` and `password`. These are the 
credentials the application needs use to call COMS.

```
kubectl create secret generic common-object-management-service-basicauth \
  --type=kubernetes.io/basic-auth \
  --from-literal=username=username \
  --from-literal=password=password

oc label secret common-object-management-service-basicauth \
  app=common-object-management-service \
  app.kubernetes.io/part-of=common-object-management-service \
  app.kubernetes.io/instance=common-object-management-service \
  app.kubernetes.io/name=common-object-management-service
```

## Create ConfigMap

```
oc create -f configmap.yaml
```

## Create DeploymentConfig

```
oc create -f deploymentconfig.yaml
```
