import { Injectable } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { KeycloakService } from 'keycloak-angular';
import Keycloak, { KeycloakLoginOptions } from 'keycloak-js';
import { from, Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { User } from '../models/user.model';

export interface IAuthService {
  login(options?: KeycloakLoginOptions): Promise<void>;
  isLoggedIn(): Promise<boolean>;
  logout(redirectUri: string): Promise<void>;
  getUser(forceReload?: boolean): Promise<User>;
  getUser$(forceReload?: boolean): Observable<User>;
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

  public isLoggedIn(): Promise<boolean> {
    return this.keycloakService.isLoggedIn();
  }

  public logout(redirectUri: string = '/'): Promise<void> {
    return this.keycloakService.logout(redirectUri);
  }

  public async getUser(forceReload?: boolean): Promise<User> {
    // try {
    // let profile = await this.keycloakService
    //   .loadUserProfile()
    //   .catch((error: any) => {
    //     this.logger.error(`Error occurred during attempted authentication`);
    //   });

    this.keycloakService
      .isLoggedIn()
      .then((result) => {
        if (result) {
          this.keycloakService.loadUserProfile().then((profile) => {
            let firstName;
            let lastName;
            firstName = profile.firstName;
            lastName = profile.lastName;
            return { firstName, lastName } as User;
          });
        }
      })
      .catch((error: any) => {
        this.logger.error(`Error occurred during attempted authentication`);
      });
    // } catch (e) {
    //   this.logger.error('Failed to load user details', e);
    //   return null;
    // }
    return ({ firstname: '', lastName: '' } as unknown) as User;
  }

  public getUser$(forceReload?: boolean): Observable<User> {
    return from(this.getUser(forceReload)).pipe(take(1));
  }
}
