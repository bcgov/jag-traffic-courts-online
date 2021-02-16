const {writeFile} = require('fs');

const {argv} = require ('yargs');

require('dotenv').config();

const environment = argv.environment;
const isProduction = environment === 'prod';

if (!process.env.API_URL || !process.env.KEYCLOAK_CLIENT_ID || !process.env.KEYCLOAK_URL || !process.env.KEYCLOAK_REALM || !process.env.USE_KEYCLOAK 
    || !process.env.USE_MOCK_SERVICES)
    {
        console.error('All the required environment variables were not provided!');
        process.exit(-1);
    }

const targetPath = isProduction ? `./src/environments/environment.prod.ts` : `./src/environments/environment.ts`;


const environmentFileContent = `
export const environment = {
  production: ${isProduction},
  version: '${process.env.VERSION}',
  useKeycloak: ${process.env.USE_KEYCLOAK},
  useMockServices: ${process.env.USE_MOCK_SERVICES},
  apiUrl: '${process.env.API_URL}',
  keycloakConfig: {
    config: {
      url: '${process.env.KEYCLOAK_URL}',
      realm: '${process.env.KEYCLOAK_REALM}',
      clientId: '${process.env.KEYCLOAK_CLIENT_ID}',
    },
    initOptions: {
      onLoad: '${process.env.KEYCLOAK_INIT_OPTIONS}',
    },
  },
};`;

writeFile(targetPath, environmentFileContent, function(err){
    if (err){
        console.log(err);
    }
    console.log(`Wrote variables to ${targetPath}`);
});
