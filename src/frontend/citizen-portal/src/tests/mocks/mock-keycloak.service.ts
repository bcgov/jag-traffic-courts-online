import { Injectable } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';

@Injectable({
  providedIn: 'root',
})
export class MockKeycloakService {
  public init(): Promise<boolean> {
    return Promise.resolve(true);
  }

  public isLoggedIn(): Promise<boolean> {
    return Promise.resolve(true);
  }

  public async login(): Promise<void> {
    // empty, just for mock login function.
  }

  public async logout(redirectUrl): Promise<void> {
    // empty, just for mock logout function.
  }

  public loadUserProfile(): Promise<UserProfile> {
    const profile: UserProfile = {
      firstName: 'mockFirstName',
      lastName: 'mockLastName',
    };
    return Promise.resolve(profile);
  }

  public getKeycloakInstance(): Keycloak.KeycloakInstance {
    return {
      onTokenExpired: () => {},
      loadUserInfo: () => {
        let callback;
        Promise.resolve().then(() => {
          callback({
            userName: 'name',
          });
        });
        return {
          success: (fn) => (callback = fn),
        };
      },
    } as any;
  }

  public updateToken(minValidity?: number): Promise<boolean> {
    return Promise.resolve(true);
  }
}

class UserProfile {
  firstName: string;
  lastName: string;
}
