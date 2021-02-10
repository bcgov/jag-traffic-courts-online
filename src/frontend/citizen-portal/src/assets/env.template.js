(function(window) {
    window["env"] = window["env"] || {};
  
    // Environment variables
    window["env"]["apiUrl"] = "${API_URL}";
    window["env"]["production"] = "${IS_PROD}";
    window["env"]["useKeycloak"] = "${USE_KEYCLOAK}";
    window["env"]["useMockServices"] = "${USE_MOCK_SERVICES}";
    window["env"]["environmentName"] = "${ENVIRONMENT}";
    window["env"]["version"] = "${VERSION}";
    window["env"]["keycloakURL"] = "${KEYCLOAK_URL}";;
    window["env"]["keycloackRealm"] = "${KEYCLOAK_REALM}";
    window["env"]["keycloakCClientId"]= "${KEYCLOAK_CLIENT_ID}";
    window["env"]["keycloakOptionsOnLoad"] = "${KEYCLOAK_INIT_OPTIONS}";
  })(this);window["env"]["apiUrl"] 