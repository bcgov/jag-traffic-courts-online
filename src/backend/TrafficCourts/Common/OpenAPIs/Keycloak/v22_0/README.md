This Keycloak OpenAPI spec was obtained from https://github.com/dahag-ag/keycloak-openapi/tree/main/OpenApiDefinitions and modified so the generated C# client would actually compile.


Note in 22 version of the OpenAPI spec they use response code 2XX to represent 200-299 which NSwag current has issues with (https://github.com/RicoSuter/NSwag/issues/4383).
A customized version of the Client.Class.liquid template is used. The base version  is based off of commit 202f3c2d7e413f329d66dd0e18743578151cefa0. It was modified per
issue number 4383.