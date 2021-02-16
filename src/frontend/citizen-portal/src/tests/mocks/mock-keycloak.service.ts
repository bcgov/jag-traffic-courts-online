import { Injectable } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';

@Injectable({
  providedIn: 'root',
})
export class MockKeycloakService extends KeycloakService {
  public init(): Promise<boolean> {
    return Promise.resolve(true);
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
