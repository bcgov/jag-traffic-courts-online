import { Injectable } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { KeycloakService } from 'keycloak-angular';
import Keycloak, { KeycloakLoginOptions } from 'keycloak-js';
import { BehaviorSubject, from, Observable } from 'rxjs';
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
    const loggedIn = await this.keycloakService.isLoggedIn();
    console.log('loggedIn', loggedIn);
    // this.keycloakService.isLoggedIn().then((loggedIn) => {
    //   console.log('loggedIn', loggedIn);
    //   if (loggedIn) {
    if (loggedIn) {
      return this.keycloakService.loadUserProfile(forceReload) as Promise<User>;
    }

    return Promise.reject();
    //   }
    // });
    // this.keycloakService.loadUserProfile(forceReload).then((profile) => {
    //   console.log('profile', profile);
    // const firstName = profile.firstName;
    // const lastName = profile.lastName;
    // const profile: User = {
    //   firstName: 'mockFirstName',
    //   lastName: 'mockLastName',
    // };
    //   return Promise.resolve(profile);
    // });
    // }
    // })
    // .catch((error: any) => {
    //   this.logger.error(`Error occurred during attempted authentication`);
    // });
    // } catch (e) {
    //   this.logger.error('Failed to load user details', e);
    //   return null;
    // }
    // return Promise.resolve({ firstName: 'unknown', lastName: 'unknown' });
    // return ({ firstname: 'unknown', lastName: 'unknown' } as unknown) as User;
  }

  public getUser$(forceReload?: boolean): Observable<User> {
    return from(this.getUser(forceReload)).pipe(take(1));
  }
}
