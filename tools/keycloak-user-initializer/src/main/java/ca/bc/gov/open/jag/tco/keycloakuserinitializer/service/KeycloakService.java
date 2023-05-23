package ca.bc.gov.open.jag.tco.keycloakuserinitializer.service;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.ws.rs.core.Response;

import org.apache.commons.lang3.StringUtils;
import org.keycloak.admin.client.Keycloak;
import org.keycloak.admin.client.resource.UsersResource;
import org.keycloak.representations.idm.FederatedIdentityRepresentation;
import org.keycloak.representations.idm.GroupRepresentation;
import org.keycloak.representations.idm.UserRepresentation;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import ca.bc.gov.open.jag.tco.keycloakuserinitializer.config.IdirApiClientConfigProperties;
import ca.bc.gov.open.jag.tco.keycloakuserinitializer.config.KeycloakConfig;
import ca.bc.gov.open.jag.tco.keycloakuserinitializer.idir.api.UsersApi;
import ca.bc.gov.open.jag.tco.keycloakuserinitializer.idir.api.model.EnvironmentIdirUsersGet200Response;
import ca.bc.gov.open.jag.tco.keycloakuserinitializer.idir.api.model.EnvironmentIdirUsersGet200ResponseDataInner;
import ca.bc.gov.open.jag.tco.keycloakuserinitializer.idir.api.model.EnvironmentIdirUsersGet200ResponseDataInnerAttributes;
import ca.bc.gov.open.jag.tco.keycloakuserinitializer.model.TcoUser;
import ca.bc.gov.open.jag.tco.keycloakuserinitializer.util.TcoUserDataLoader;

import static ca.bc.gov.open.jag.tco.keycloakuserinitializer.util.Keys.*;

@Service
public class KeycloakService {
	
	private static final Logger logger = LoggerFactory.getLogger(KeycloakService.class);
	
	private final Keycloak keycloak;
	
	// Delegate, OpenAPI generated client
	private final UsersApi usersApi;
	
	@Autowired
	private TcoUserDataLoader userLoader;
	
	@Autowired
	private IdirApiClientConfigProperties idirProperties;
	
	public KeycloakService (Keycloak keycloak, UsersApi usersApi) {
		this.keycloak = keycloak;
		this.usersApi = usersApi;
	}
	
	public UserRepresentation getUser(String userName) {
        UsersResource usersResource = getInstance();
        List<UserRepresentation> userResult = usersResource.search(userName, true);

        if (userResult != null && !userResult.isEmpty()) {
        	if (userResult.size() > 1) {
				logger.warn("More than one user is found for the given userName {} ", userName);
			}
			return userResult.get(0);
		}
        return null;
    }
	
	public EnvironmentIdirUsersGet200ResponseDataInner getIdirUser(String userName) {
		EnvironmentIdirUsersGet200Response response = usersApi.environmentIdirUsersGet(idirProperties.getIdirApiClientEnv(), null, null, userName, null);
		if (response.getData() != null) {
			List<EnvironmentIdirUsersGet200ResponseDataInner> data = response.getData();
			if (data != null && !data.isEmpty()) {
				if (data.size() > 1) {
					logger.warn("More than one user is found for the given userName {} ", userName);
				}
				return data.get(0);
			}
		}
		logger.warn("User is not found in IDIR for the given userName {} ", userName);
        return null;
	}
	
	public List<TcoUser> getTcoUsers() {

        List<TcoUser> userResult = userLoader.getTcoUsers();

        if (userResult != null && !userResult.isEmpty()) {
        	for (TcoUser tcoUser : userResult) {
				logger.info(tcoUser.toString());
			}
			return userResult;
		}
        return null;
    }
	
	public List<String> createKeycloakUsers() {
		// Get user and group data from the CSV file
		List<TcoUser> userResult = userLoader.getTcoUsers();
		List<String> createdUsers = new ArrayList<>();
		for (TcoUser tcoUser : userResult) {
			if (!StringUtils.isBlank(tcoUser.getEmail())) {
				// Get IDIR user data
				EnvironmentIdirUsersGet200ResponseDataInner idirUser = getIdirUser(tcoUser.getEmail());
				if (idirUser != null && !isUserInKeycloak(idirUser.getUsername())) {
					List<String> groups = new ArrayList<>();
					// Add groups to assign based on the CSV data
					if (tcoUser.getRealmAdmin()) {
						groups.add(getUserGroup(REALM_ADMINS).getName());
					}
					if (tcoUser.getAdminJudicialJustice()) {
						groups.add(getUserGroup(ADMIN_JJ).getName());
					}
					if (tcoUser.getAdminVtcStaff()) {
						groups.add(getUserGroup(ADMIN_VTC).getName());
					}
					if (tcoUser.getJudicialJustice()) {
						groups.add(getUserGroup(JJ).getName());
					}
					if (tcoUser.getVtcStaff()) {
						groups.add(getUserGroup(VTC).getName());
					}
					//Add user
	                logger.info("creating user {} ", idirUser.getUsername());
	                UserRepresentation newUser = new UserRepresentation();
	                newUser.setUsername(idirUser.getUsername());
	                newUser.setFirstName(idirUser.getFirstName());
                    newUser.setLastName(idirUser.getLastName());
                    newUser.setEmail(idirUser.getEmail());
                    newUser.setFederatedIdentities(getFederationLink(idirUser.getUsername()));
                    newUser.setEnabled(true);
                    newUser.setGroups(groups);
                    newUser.setAttributes(getAttributes(idirUser, tcoUser));
                    Response response = getInstance().create(newUser);
                    if (Response.Status.CREATED.equals(response.getStatusInfo())) {
						createdUsers.add(tcoUser.getEmail());
					}
				} else if(idirUser != null && isUserInKeycloak(idirUser.getUsername())) {
					logger.info("IDIR user already exists in Keycloak for the email address {} ", tcoUser.getEmail());
				} else {
					logger.debug("Failed to return the IDIR user for the email address {} ", tcoUser.getEmail());
				}	
			} else {
				logger.debug("Email address that is read from the CSV file is blank");
			}
		}
		return createdUsers;
	}
	
	private boolean isUserInKeycloak(String userName) {
		
		UserRepresentation user = getUser(userName);
		if(user != null) {
			return true;
		}
		return false;
	}
	
	public GroupRepresentation getUserGroup(String groupName) {
		
		 GroupRepresentation representation = keycloak.realm(KeycloakConfig.realm).groups().groups().stream()
				 .filter(groupRepresentation -> groupRepresentation.getName().equals(groupName))
				 .findFirst().get();
		 logger.info("assigning group {} ", representation.getName());
		 return representation;
	}
	
	private List<FederatedIdentityRepresentation> getFederationLink(String idirGuid) {

        FederatedIdentityRepresentation idirLink = new FederatedIdentityRepresentation();
        idirLink.setIdentityProvider(IDIR_IDP);
        idirLink.setUserId(idirGuid);
        idirLink.setUserName(idirGuid);

        return Collections.singletonList(idirLink);
    }
	
	private Map<String, List<String>> getAttributes(EnvironmentIdirUsersGet200ResponseDataInner idirUser, TcoUser tcoUser) {
		
        Map<String, List<String>> attributes = new HashMap<>();
        List<String> givenNameValue = new ArrayList<>();
        List<String> familyNameValue = new ArrayList<>();
        List<String> partIdValue = new ArrayList<>();
        
        EnvironmentIdirUsersGet200ResponseDataInnerAttributes idirAttributes = idirUser.getAttributes();
        attributes.put("display_name", idirAttributes.getDisplayName());
        attributes.put("idir_username", idirAttributes.getIdirUsername());
        attributes.put("useridentifier", idirAttributes.getIdirUserGuid());
        
        givenNameValue.add(idirUser.getFirstName());
        attributes.put("given_name", givenNameValue);
        familyNameValue.add(idirUser.getLastName());
        attributes.put("family_name", familyNameValue);
        
        if(!StringUtils.isBlank(tcoUser.getPartId())) {
        	partIdValue.add(tcoUser.getPartId());
            attributes.put("partid", partIdValue);
        }
        
        return attributes;
    }

	public UsersResource getInstance(){
        return keycloak.realm(KeycloakConfig.realm).users();
    }
}
