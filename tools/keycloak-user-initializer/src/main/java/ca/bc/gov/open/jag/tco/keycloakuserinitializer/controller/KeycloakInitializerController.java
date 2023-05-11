package ca.bc.gov.open.jag.tco.keycloakuserinitializer.controller;

import org.keycloak.representations.idm.UserRepresentation;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import ca.bc.gov.open.jag.tco.keycloakuserinitializer.idir.api.model.EnvironmentIdirUsersGet200ResponseDataInner;
import ca.bc.gov.open.jag.tco.keycloakuserinitializer.service.KeycloakService;

@RestController
@RequestMapping(path = "api/user")
public class KeycloakInitializerController {
	
	@Autowired
    KeycloakService service;
	
	@GetMapping("/ping")
	public String index() {
		return "Greetings from Keycloak App!";
	}
	
	/**
	 * GET endpoint that retrieves the user data from the Keycloak for the given userName
	 * @param userName
	 * @return {@link UserRepresentation}
	 */
	@GetMapping(path = "/{userName}")
    public UserRepresentation getUser(@PathVariable("userName") String userName){
        return service.getUser(userName);
    }
	
	/**
	 * GET endpoint that retrieves the user data from the IDIR API for the given userName
	 * @param userName
	 * @return {@link EnvironmentIdirUsersGet200ResponseDataInner}
	 */
	@GetMapping(path = "/idir/{userName}")
    public EnvironmentIdirUsersGet200ResponseDataInner getIdirUser(@PathVariable("userName") String userName){
		return service.getIdirUser(userName);
    }
}
