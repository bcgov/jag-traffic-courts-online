import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { KeycloakOnLoad } from 'keycloak-js';

export interface IKeycloakConfig {
  config: {
    url: string;
    realm: string;
    clientId: string;
  },
  initOptions: {
    onLoad: string;
    silentCheckSsoRedirectUri;
  }
}

export class KeycloakConfig implements IKeycloakConfig {
  config: {
    url: string;
    realm: string;
    clientId: string;
  };
  initOptions: {
    onLoad: KeycloakOnLoad;
    silentCheckSsoRedirectUri: string;
  };
}

@Injectable({
  providedIn: 'root',
})
export class KeycloakConfigService {
  private keycloakConfig: KeycloakConfig;

  constructor(private http: HttpClient) { }

  public loadKeycloakConfig(): Observable<any> {
    return this.http.get('/assets/config/keycloak.config.json').pipe(
      map((config: KeycloakConfig) => {
        this.keycloakConfig = config;
      })
    );
  }

  get url(): string {
    return this.keycloakConfig.config.url;
  }

  get realm(): string {
    return this.keycloakConfig.config.realm;
  }

  get clientId(): string {
    return this.keycloakConfig.config.clientId;
  }

  get onLoad(): KeycloakOnLoad {
    return this.keycloakConfig.initOptions.onLoad;
  }

  get silentCheckSsoRedirectUri(): string {
    return this.keycloakConfig.initOptions.silentCheckSsoRedirectUri;
  }
}
