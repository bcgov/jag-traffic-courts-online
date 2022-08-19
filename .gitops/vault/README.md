
References
- https://github.com/bcgov/how-to-workshops/blob/master/vault/README.md
- https://github.com/BCDevOps/openshift-wiki/blob/master/docs/Vault/VaultGettingStarted.md

We organized our secrets as follows:

```
{licenseplate}-nonprod                 #secret engine
 ├── dev                               #secret path
 │   ├── arc-dispute-api               #secret name
 │   ├── citizen-api                   #secret name
 │   ├── oracle-data-api               #secret name
 │   ├── staff-api                     #secret name
 |   └── workflow-service              #secret name
 └── test
     ├── arc-dispute-api               #secret name
     ├── citizen-api                   #secret name
     ├── oracle-data-api               #secret name
     ├── staff-api                     #secret name
     └── workflow-service              #secret name

{licenseplate}-prod                    #secret engine
   ├── arc-dispute-api                 #secret name
   ├── citizen-api                     #secret name
   ├── oracle-data-api                 #secret name
   ├── staff-api                       #secret name
   └── workflow-service                #secret name
```


# Apply secrets to Vault

vault kv put 0198bb-nonprod/%ENV%/%~nS @%S