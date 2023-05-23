#Traffic Court Online Keycloak User Initializer
This spring-boot Java project is a tool for initializing users in a Keycloak environment through reading users' email and group data from the provided CSV file (tco-user-list.csv)
and lookup their IDIR data by their email from CSS SSO API (https://api.loginproxy.gov.bc.ca/openapi/swagger) and link it as an external IDP account in Keycloak.

## Columns must be specified in the CSV file (tco-user-list.csv)
Email,Realm Administrator,admin-judicial-justice,admin-vtc-staff,judicial-justice,vtc-staff,part-id

example@email.com,TRUE,TRUE,TRUE,TRUE,TRUE,190225.0866

## How to build
From the /tools folder,
 
`docker-compose up -d --build keycloak-user-init`

## Swagger
Visit [http://localhost:8081/swagger-ui/index.html](http://localhost:8081/swagger-ui/index.html)

The main endpoint is `/api/user/create` in order to trigger initializing users in Keycloak from the provided CSV file.

```
curl -X 'POST' \
  'http://localhost:8081/api/user/create' \
  -H 'accept: */*' \
  -d ''
```

The response returns the list of emails of the successfully initialized users in Keycloak. If all users in the CSV are already in keycloak, it returns an empty list.

The tool also allows querying specified users in both Keycloak and IDIR through the following endpoints:

GET request to `/api/user/{userName}` by providing Keycloak username of the user as URL param returns the Keycloak user data.

```
curl -X 'GET' \
  'http://localhost:8081/api/user/d29c519aa0e04de5aa1e5e39f27299da%40idir' \
  -H 'accept: */*'
```

Example response:
```
{
  "self": "string",
  "id": "string",
  "origin": "string",
  "createdTimestamp": 0,
  "username": "string",
  "enabled": true,
  "emailVerified": true,
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "federationLink": "string",
  "serviceAccountClientId": "string",
  "attributes": {
    "additionalProp1": [
      "string"
    ],
    "additionalProp2": [
      "string"
    ],
    "additionalProp3": [
      "string"
    ]
  },
  "credentials": [
    {
      "id": "string",
      "type": "string",
      "userLabel": "string",
      "createdDate": 0,
      "secretData": "string",
      "credentialData": "string",
      "priority": 0,
      "value": "string",
      "temporary": true
    }
  ],
  "disableableCredentialTypes": [
    "string"
  ],
  "requiredActions": [
    "string"
  ],
  "federatedIdentities": [
    {
      "identityProvider": "string",
      "userId": "string",
      "userName": "string"
    }
  ],
  "realmRoles": [
    "string"
  ],
  "clientRoles": {
    "additionalProp1": [
      "string"
    ],
    "additionalProp2": [
      "string"
    ],
    "additionalProp3": [
      "string"
    ]
  },
  "clientConsents": [
    {
      "clientId": "string",
      "grantedClientScopes": [
        "string"
      ],
      "createdDate": 0,
      "lastUpdatedDate": 0
    }
  ],
  "notBefore": 0,
  "groups": [
    "string"
  ],
  "access": {
    "additionalProp1": true,
    "additionalProp2": true,
    "additionalProp3": true
  }
}
```

GET request to `/api/user/idir/{userName}` by providing email of the IDIR user as URL param returns the IDIR user data.

```
curl -X 'GET' \
  'http://localhost:8081/api/user/idir/example%40email.com \
  -H 'accept: */*'
```

Example response:
```
{
  "username": "string",
  "email": "string",
  "firstName": "string",
  "lastName": "string",
  "attributes": {
    "display_name": [
      "string"
    ],
    "idir_user_guid": [
      "string"
    ],
    "idir_username": [
      "string"
    ]
  }
}
```