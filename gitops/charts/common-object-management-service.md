# common-object-management-service


## Deployment Instructions

1. Deploy Postgres database using Crunchy Postgres Operator Helm Chart. The release name, `postgres-tco`, 
will be used for the database credential secrets in `common-object-management-service-values.yaml`

```
helm install postgres-tco postgrescluster-5.4.2.tgz --values .\postgres-tco-dev-values.yaml 
```

2. Grant coms user access to public schema of the coms database by connecting to the shell of the primary node,

```
psql
\c comsdb
GRANT ALL ON SCHEMA public to coms;
```

3. Create a secret for the basic auth username/password info. These are the credentials TCO will connect to COMS with

```
kind: Secret
apiVersion: v1
type: kubernetes.io/basic-auth
metadata:
  name: common-object-management-service-basicauth
  labels:
    app: common-object-management-service
data:
  password: base64-password
  username: base64-username
```

4. Create a secret for the object store connection info

```
kind: Secret
apiVersion: v1
type: Opaque
metadata:
  name: common-object-management-service-objectstorage
  labels:
    app: common-object-management-service
data:
  password: base64-password
  username: base64-username
  bucket: base64-bucket
  endpoint: base64-endpoint
```

5. Ensure the name and keys match the `common-object-management-service-values.yaml` file in the basicAuth and 
objectStorage sections

6. Install the common-object-management-service Helm chart, 

```
helm install coms common-object-management-service --values .\common-object-management-service-values.yaml
```
