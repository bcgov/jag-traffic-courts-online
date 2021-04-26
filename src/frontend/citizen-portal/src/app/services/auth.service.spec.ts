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

  it('login should invoke keycloak login', (done) => {
    spyOn(keycloakService, 'login').and.callThrough();
    authService.login().then(() => {
      expect(keycloakService.login).toHaveBeenCalled();
      done();
    });
  });

  it('isLoggedIn should invoke keycloak isLoggedIn', (done) => {
    spyOn(keycloakService, 'isLoggedIn').and.returnValue(Promise.resolve(true));
    authService.isLoggedIn().then((result) => {
      expect(keycloakService.isLoggedIn).toHaveBeenCalled();
      expect(result).toEqual(true);
      done();
    });
  });

  it('getUser should getUserInfo when loggedIn', (done) => {
    spyOn(keycloakService, 'loadUserProfile').and.callThrough();
    authService.getUser().then((result) => {
      expect(keycloakService.loadUserProfile).toHaveBeenCalled();
      expect(result.firstName).toEqual('mockFirstName');
      expect(result.lastName).toEqual('mockLastName');
      done();
    });
  });

  it('getUser should reject when not loggedIn', (done) => {
    spyOn(keycloakService, 'isLoggedIn').and.returnValue(
      Promise.resolve(false)
    );
    authService.getUser().catch((error) => {
      expect(error).toEqual('user not logged in.');
      done();
    });
  });

  it('getUser$ should get userInfo', (done) => {
    authService.getUser$().subscribe((result) => {
      expect(result.firstName).toEqual('mockFirstName');
      expect(result.lastName).toEqual('mockLastName');
      done();
    });
  });

  it('logout should invoke keycloak logout', (done) => {
    spyOn(keycloakService, 'logout').and.callThrough();
    authService.logout().then(() => {
      expect(keycloakService.logout).toHaveBeenCalled();
      done();
    });
  });
});
