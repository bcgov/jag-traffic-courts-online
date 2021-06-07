import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export class AppConfig {
  production: boolean;
  version: string;
  useMockServices: boolean;
  apiBaseUrl: string;
}

@Injectable({
  providedIn: 'root',
})
export class AppConfigService {
  private appConfig: AppConfig;

  constructor(private http: HttpClient) {}

  public loadAppConfig() {
    return this.http
      .get('/assets/app.config.json')
      .toPromise()
      .then((data: AppConfig) => {
        this.appConfig = data;
        console.log('AppConfigService.loadAppConfig', data);
      });
  }

  get production(): boolean {
    return this.appConfig?.production;
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
}
