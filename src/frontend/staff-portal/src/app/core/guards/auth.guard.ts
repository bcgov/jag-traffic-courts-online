import { Injectable } from '@angular/core';
import {
  CanActivate,
  CanActivateChild,
  Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { KeycloakService } from 'keycloak-angular';

@Injectable()
export class AuthGuard implements CanActivate, CanActivateChild {
  private authenticated: boolean;

  constructor(
    protected router: Router,
    protected keycloakService: KeycloakService,
    private logger: LoggerService
  ) {}

  public get isAuthenticated(): boolean {
    return this.authenticated;
  }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Promise<boolean> {
    return this.checkLogin(state.url);
  }

  canActivateChild(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Promise<boolean> {
    return this.checkLogin(state.url);
  }

  private checkLogin(url: string): Promise<boolean> {
    return new Promise(async (resolve, reject) => {
      try {
        this.authenticated = await this.keycloakService.isLoggedIn();
        if (!this.authenticated) {
          this.keycloakService.login().catch((e) => {
            this.logger.error('keycloakService.login failure', e);
            this.router.navigate(['/']);
          });
        }
        const result = await this.canAccess(this.authenticated, url);
        resolve(result);
      } catch (error) {
        const destination = url ? ` to ${url} ` : ' ';
        const message = `Route access ${destination} has been denied`;
        reject(`${message}: ${error}`);
      }
    });
  }

  protected canAccess(
    authenticated: boolean,
    routePath: string = null
  ): Promise<boolean> {
    return new Promise((resolve, reject) =>
      authenticated ? resolve(true) : reject(false)
    );
  }
}
