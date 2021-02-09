// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  environmentName: "development",
  version: '1.0.0',
  useKeycloak: true,
  useMockServices: false,
  apiUrl: 'http://localhost:4300/api',
  keycloakConfig: {
    config: {
      url: 'http://localhost:8080/auth',
      realm: 'tco',
      clientId: 'tco-client',
    },
    initOptions: {
      onLoad: 'check-sso',
    },
  },
};
