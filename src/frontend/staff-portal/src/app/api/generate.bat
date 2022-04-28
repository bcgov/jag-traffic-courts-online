REM Fetch the latest version of the staff api spec
REM 
REM    curl -o swagger.json https://localhost:6001/swagger/v1/swagger.json
REM

REM Config Options for typescript-angular
REM https://openapi-generator.tech/docs/generators/typescript-angular/

docker run --rm ^
  -v %CD%:/local openapitools/openapi-generator-cli generate ^
  -i /local/swagger.json ^
  -g typescript-angular ^
  -t /local/templates ^
  --additional-properties=modelFileSuffix=.model ^
  -o /local