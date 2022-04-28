docker pull openapitools/openapi-generator-cli:latest

docker run --rm -v "$(pwd)":/local openapitools/openapi-generator-cli generate -i /local/client-api.json -g typescript-angular -t /local/templates --additional-properties=modelFileSuffix=.model -o /local
