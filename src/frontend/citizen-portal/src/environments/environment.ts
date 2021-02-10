// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  environmentName: "local-dev",
  version: '1.0.0',
  useKeycloak: false,
  useMockServices: true,
  apiUrl: 'http://localhost:4300/api',
  keycloakConfig: {
    config: {
      url: 'http://localhost:8081/auth',
      realm: 'tco',
      clientId: 'tco-client',
    },
    initOptions: {
      onLoad: 'check-sso',
    },
  }
};



    
// export const environment= {​​​​​
// production: false,
// version: '1.0.0',
// useKeycloak: false,
// useMockServices: true,
// apiUrl: 'http://localhost:4300/api',
// keycloakConfig: {​​​​​
// config: {​​​​​
// url: 'http://localhost:8081/auth',
// realm: 'tco',
// clientId: 'tco-client',
//     }​​​​​,
// initOptions: {​​​​​
// onLoad: 'check-sso',
//     }​​​​​,
//   }​​​​​,
// }​​​​​;


