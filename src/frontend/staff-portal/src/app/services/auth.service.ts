import { Injectable } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { User } from '@shared/models/user.model';
import { KeycloakService } from 'keycloak-angular';
import Keycloak, { KeycloakLoginOptions } from 'keycloak-js';
import { BehaviorSubject, from, Observable } from 'rxjs';
import { take } from 'rxjs/operators';

export interface IAuthService {
  login(options?: KeycloakLoginOptions): Promise<void>;
  isLoggedIn(): Promise<boolean>;
  logout(redirectUri: string): Promise<void>;
  getUser(forceReload?: boolean): Promise<User>;
  getUser$(forceReload?: boolean): Observable<User>;
  getToken(): Observable<string>;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(
    private keycloakService: KeycloakService,
    private logger: LoggerService
  ) {}

  public login(options?: any): Promise<void> {
    return this.keycloakService.login(options);
  }

  public getToken(): Promise<string> {
    return this.keycloakService.getToken();
  }

  public isLoggedIn(): Promise<boolean> {
    return this.keycloakService.isLoggedIn();
  }

  public logout(redirectUri: string = '/'): Promise<void> {
    return this.keycloakService.logout(redirectUri);
  }

  public async getUser(forceReload?: boolean): Promise<User> {
    const loggedIn = await this.keycloakService.isLoggedIn();
    this.logger.info('isLoggedIn', loggedIn);
    if (loggedIn) {
      return this.keycloakService.loadUserProfile(forceReload) as Promise<User>;
    }
    return Promise.reject('user not logged in.');
  }

  public getUser$(forceReload?: boolean): Observable<User> {
    return from(this.getUser(forceReload)).pipe(take(1));
  }
}
