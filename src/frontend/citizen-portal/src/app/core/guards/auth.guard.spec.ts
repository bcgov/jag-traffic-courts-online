import { getTestBed, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { KeycloakService } from 'keycloak-angular';
import { MockKeycloakService } from 'tests/mocks/mock-keycloak.service';

import { AuthGuard } from './auth.guard';

describe('AuthGuard', () => {
  let guard: AuthGuard;
  let injector: TestBed;
  let keycloakService: KeycloakService;
  const routeMock: any = { snapshot: {} };
  const routeStateMock: any = { snapshot: {}, url: '/cookies' };
  const routerMock = { navigate: jasmine.createSpy('navigate') };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AuthGuard,
        { provide: Router, useValue: routerMock },
        { provide: KeycloakService, useClass: MockKeycloakService },
      ],
    });
    injector = getTestBed();
    guard = injector.inject(AuthGuard);
    keycloakService = injector.inject(KeycloakService);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });

  // it('should redirect an unauthenticated user to the keycloak login route', () => {
  //   spyOn(keycloakService, 'isLoggedIn').and.returnValue(
  //     Promise.resolve(false)
  //   );
  //   spyOn(keycloakService, 'login').and.callThrough();
  //   guard.canActivate(routeMock, routeStateMock).then((value) => {
  //     expect(value).toEqual(false);
  //     expect(keycloakService.login).toHaveBeenCalled();
  //   });
  // });

  // it('should allow the loggedIn user to access app', () => {
  //   spyOn(keycloakService, 'isLoggedIn').and.returnValue(Promise.resolve(true));
  //   guard.canActivate(routeMock, routeStateMock).then((value) => {
  //     expect(value).toEqual(true);
  //   });
  // });
});
