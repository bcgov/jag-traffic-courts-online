# Override Templates

Open API Generator has its templates stored here:

https://github.com/OpenAPITools/openapi-generator/tree/master/modules/openapi-generator/src/main/resources/typescript-angular

# Template Changes

This section will list any customizations to the default templates. Only the templates that are modified need to be 
stored in this folder. Please ensure you refer to the git sha of the version that was used. This allows the
dev team to understand which version we started from and apply any new changes as fixes are applied. Additionally,
before committing an altered template, commit the original version so we have the base version.

## api.service.mustache

**Base Revision**: 9312ed831f2b8ffc3ee03bb2717f29f572735047

To avoid having to configure the basePath, we change the generated protected basePath on the API Service to be empty string:

`protected basePath = '';`

