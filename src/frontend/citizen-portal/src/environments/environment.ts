export const environment = {
  production: false,
  version: '1.0.0',
  useKeycloak: true,
  useMockServices: true,
  apiUrl: 'http://dispute-api:5000/api',
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
