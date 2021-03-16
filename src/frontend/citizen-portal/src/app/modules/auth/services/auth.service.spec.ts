import { TestBed } from '@angular/core/testing';
import { LoggerService } from '@core/services/logger.service';
import { KeycloakService } from 'keycloak-angular';
import { MockKeycloakService } from 'tests/mocks/mock-keycloak.service';

import { AuthService } from './auth.service';

describe('AuthService', () => {
  let authService: AuthService;
  let keycloakService: KeycloakService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AuthService,
        { provide: KeycloakService, useClass: MockKeycloakService },
        LoggerService,
      ],
    });
    authService = TestBed.inject(AuthService);
    keycloakService = TestBed.inject(KeycloakService);
  });

  it('should be created', () => {
    expect(authService).toBeTruthy();
  });

  it('login should invoke keycloak login', () => {
    spyOn(keycloakService, 'login').and.callThrough();
    authService.login().then(() => {
      expect(keycloakService.login).toHaveBeenCalled();
    });
  });

  it('isLoggedIn should invoke keycloak isLoggedIn', () => {
    spyOn(keycloakService, 'isLoggedIn').and.returnValue(Promise.resolve(true));
    authService.isLoggedIn().then((result) => {
      expect(keycloakService.isLoggedIn).toHaveBeenCalled();
      expect(result).toEqual(true);
    });
  });

  it('getUser should getUserInfo when loggedIn', () => {
    spyOn(keycloakService, 'loadUserProfile').and.callThrough();
    authService.getUser().then((result) => {
      expect(keycloakService.loadUserProfile).toHaveBeenCalled();
      expect(result.firstName).toEqual('mockFirstName');
      expect(result.lastName).toEqual('mockLastName');
    });
  });

  it('getUser should reject when not loggedIn', () => {
    spyOn(keycloakService, 'isLoggedIn').and.returnValue(
      Promise.resolve(false)
    );
    authService.getUser().catch((error) => {
      expect(error).toEqual('user not logged in.');
    });
  });

  it('getUser$ should get userInfo', () => {
    authService.getUser$().subscribe((result) => {
      expect(result.firstName).toEqual('mockFirstName');
      expect(result.lastName).toEqual('mockLastName');
    });
  });

  it('logout should invoke keycloak logout', () => {
    spyOn(keycloakService, 'logout').and.callThrough();
    authService.logout().then(() => {
      expect(keycloakService.logout).toHaveBeenCalled();
    });
  });
});
