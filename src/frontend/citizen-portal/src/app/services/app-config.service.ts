import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export class AppConfig {
  version: string;
  useKeycloak: boolean;
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

  get version(): string {
    return this.appConfig?.version;
  }

  get useKeycloak(): boolean {
    return this.appConfig?.useKeycloak;
  }

  get apiBaseUrl(): string {
    return this.appConfig?.apiBaseUrl;
  }
}
