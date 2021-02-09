export const environment = {
  production: true,
  environmentName: "production",
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
  }
};
