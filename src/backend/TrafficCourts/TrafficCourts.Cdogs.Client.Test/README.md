# Common Document Generation Service (CDOGS) testing

# Credentials

Use the `Request Access` link on the [Common Service Showcase](https://bcgov.github.io/common-service-showcase/services/cdogs.html) page. Add these values to the user secrets 
of this project.

# Example User Secrets file

```
{
  "Cdogs": {
    "ClientId": "",
    "ClientSecret": "",
    "TokenEndpoint": "https://dev.loginproxy.gov.bc.ca/auth/realms/comsvcauth/protocol/openid-connect/token",
    "Endpoint": "https://cdogs-dev.api.gov.bc.ca/api"
  }
}
```
