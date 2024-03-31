
# Manual Configuration

Create the secrets by updating the values in `gitops/openshift/<env>`. Create both the object storage and basic auth secrets. You will need to modify the templates. Do NOT check in the modified files.  For the basic auth, use a secure password generator for both the username and password. For the object storage, the values will need to come from the object storage admin.

```
oc create -f common-object-management-service-basicauth.yaml
oc create -f common-object-management-service-objectstorage.yaml
```

# Deploy

```powershell
helm upgrade --install coms common-object-management-service --values values-prod.yaml
```
