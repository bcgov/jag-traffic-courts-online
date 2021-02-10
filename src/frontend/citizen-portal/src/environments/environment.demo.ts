// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: window["env"]["production"] ,
  environmentName: window["env"]["environmentName"] ,
  version: window["env"]["version"],
  useKeycloak: window["env"]["useKeycloak"],
  useMockServices: window["env"]["useMockServices"],
  apiUrl: window["env"]["apiUrl"] ,
  keycloakConfig: {
    config: {
      url: window["env"]["keycloakURL"],
      realm: window["env"]["keycloackRealm"],
      clientId: window["env"]["keycloakCClientId"],
    },
    initOptions: {
      onLoad: window["env"]["keycloakOptionsOnLoad"],
    },
  },
};
