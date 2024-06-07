import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { AppRoutes } from 'app/app.routes';
import { AuthService } from 'app/services/auth.service';
import { KeycloakAuthGuard, KeycloakService } from 'keycloak-angular';

@Injectable({
  providedIn: 'root',
})
export class AuthorizationGuard extends KeycloakAuthGuard {
  constructor(
    protected readonly router: Router,
    protected readonly keycloak: KeycloakService,
    protected readonly authService: AuthService,
  ) {
    super(router, keycloak);
  }

  public async isAccessAllowed(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    // Force the user to log in if currently unauthenticated.
    let permission;
    if (!this.authenticated) {
      this.authService.login();
    }

    // Get the roles required from the route.
    const requiredRoles = route.data.roles;

    // Allow the user to to proceed if no additional roles are required to access the route.
    if (!requiredRoles || requiredRoles.length === 0) {
      permission = true;
    } else {
      if (!this.roles || this.roles.length === 0) {
        permission = false;
      }
      // Allow the user to proceed if any of the required role(s) is/are present.
      if (requiredRoles.some((role) => this.roles.indexOf(role) > -1))
      {
        permission = true;
      } else {
        permission = false;
      };
    }

    if(!permission){
      let application;
      if(state.url.indexOf(AppRoutes.JJ) > -1) {
        application = "JJ";
      } else {
        application = "Staff";
      }
      this.router.navigate([AppRoutes.UNAUTHORIZED], {queryParams: {application: application}});
    }

    return permission;
  }
}
