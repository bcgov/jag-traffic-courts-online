import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

export interface IAppConfig {
  production: boolean;
  environment: string;
  version: string;
  useMockServices: boolean;
  apiBaseUrl: string;
  features: {
    [name: string]: boolean;
  };
}

export class AppConfig implements IAppConfig {
  production: boolean;
  environment: string;
  version: string;
  useMockServices: boolean;
  apiBaseUrl: string;
  features: {
    dispute: boolean;
  };
}

@Injectable({
  providedIn: 'root',
})
export class AppConfigService {
  private appConfig: AppConfig;

  constructor(private http: HttpClient) { }

  public loadAppConfig(): Observable<any> {
    return this.http.get('/assets/app.config.json').pipe(
      map((config: AppConfig) => {
        this.appConfig = config;
      })
    );
  }

  get production(): boolean {
    return this.appConfig?.production;
  }

  get environment(): string {
    return this.appConfig?.environment;
  }

  get version(): string {
    return this.appConfig?.version;
  }

  get useMockServices(): boolean {
    return this.appConfig?.useMockServices;
  }

  get apiBaseUrl(): string {
    return this.appConfig?.apiBaseUrl;
  }

  get featureFlagDispute(): boolean {
    const flag = this.appConfig?.features?.dispute;
    return flag ? flag : false;
  }

  public isFeatureFlagEnabled(featureName: string): boolean {
    // Read the value from the config service
    if (this.appConfig?.features?.hasOwnProperty(featureName)) {
      return this.appConfig?.features[featureName];
    }
    return false; // if feature not found, default to turned off
  }
}
