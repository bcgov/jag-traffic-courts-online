
export const environment = {
  production: true,
  version: '1.0.0',
  useKeycloak: false,
  useMockServices: true,
  apiUrl: 'http://dispute-api:5000/api',
  keycloakConfig: {
    config: {
      url: 'http://keycloak:8081/auth',
      realm: 'traffic-court',
      clientId: 'test',
    },
    initOptions: {
      onLoad: 'check-sso',
    },
  },
};
