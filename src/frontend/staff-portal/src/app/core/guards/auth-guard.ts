import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { _getFocusedElementPierceShadowDom } from '@angular/cdk/platform';

@Injectable({ providedIn: 'root' })
export class AuthorizationGuard implements CanActivate {
  passAuthGuard: Boolean = false;
  constructor(private oidcSecurityService: OidcSecurityService, private router: Router, public jwtHelper :JwtHelperService) {

  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> {
    return this.oidcSecurityService.isAuthenticated$.pipe(
      map(({ isAuthenticated }) => {
        // allow navigation if authenticated
        if (isAuthenticated) {

            // this will be passed from the route config
            // on the data property
            const expectedRole = route.data.expectedRole;
    
            // decode the token to get its payload
            const tokenPayload = this.jwtHelper.decodeToken(this.oidcSecurityService.getAccessToken());
            let resource_access = tokenPayload.resource_access["tco-staff-portal"];
            if (resource_access) {
                let roles = resource_access.roles;
                if (roles) roles.forEach(role => {
                  if (role == expectedRole) { 
                     this.passAuthGuard = true;
                  }
                });
              }

            if (this.passAuthGuard == true) return true;
            else return this.router.parseUrl('/unauthorized');
        }

        // redirect if not authenticated
        return this.router.parseUrl('/unauthorized');
      })
    );
  }
}