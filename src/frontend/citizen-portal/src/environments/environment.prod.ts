
export const environment = {
  production: true,
  version: '1.0.0',
  useKeycloak: false,
  useMockServices: true,
  apiUrl: 'http://localhost:4300/api',
  keycloakConfig: {
    config: {
      url: 'http://localhost:8081/auth',
      realm: 'traffic-court',
      clientId: 'test',
    },
    initOptions: {
      onLoad: 'check-sso',
    },
  },
};