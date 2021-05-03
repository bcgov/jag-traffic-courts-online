export const environment = {
  production: false,
  version: '1.0.0',
  useKeycloak: false,
  useMockServices: false,
  apiUrl: '/api',
  keycloakConfig: {
    config: {
      url: 'http://localhost:8082/auth',
      realm: 'TCO',
      clientId: 'citizenPortal',
    },
    initOptions: {
      onLoad: 'check-sso',
    },
  },
};

// apiUrl is set to '/api' to use proxy.conf.json and avoid the CORS Policy error
// start local server with: ng serve --proxy-config proxy.conf.json
