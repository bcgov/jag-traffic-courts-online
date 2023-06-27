package ca.bc.gov.open.jag.tco.keycloakuserinitializer.controller;

import java.util.List;

import org.keycloak.representations.idm.UserRepresentation;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import ca.bc.gov.open.jag.tco.keycloakuserinitializer.idir.api.model.EnvironmentIdirUsersGet200ResponseDataInner;
import ca.bc.gov.open.jag.tco.keycloakuserinitializer.service.KeycloakService;

@RestController
@RequestMapping(path = "api/user")
public class KeycloakInitializerController {
	
	@Autowired
    KeycloakService service;
	
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
	
	/**
	 * POST endpoint that initializes users in Keycloak with assigned groups based on the tco-user-list.csv and IDIR (IDP) data
	 * @return
	 */
	@PostMapping(path = "/create")
    public List<String> createKeycloakUser(){
		return service.createKeycloakUsers();
    }
}
