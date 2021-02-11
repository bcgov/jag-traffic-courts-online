
export const environment = {
    production: false,
    environmentName: "demo",
    version: "1.0.0",
    useKeycloak: "false",
    useMockServices: "true",
    apiUrl: "http://localhost:4300/api",
    keycloakConfig: {
        config: {
          url: "http://keycloak:8081/auth",
          realm: "traffic-court",
          clientId: "test",
        },
        initOptions: {
          onLoad: "check-sso",
        },
      }
}