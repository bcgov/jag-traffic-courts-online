import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export class AppConfig {
  production: boolean;
  environment: string;
  version: string;
  useMockServices: boolean;
  apiBaseUrl: string;
  understandYourTicketLink: string;
  paymentOptionsLink: string;
  resolutionOptionsLink: string;
  roadSafetyBCVisitUsLink: string;
  icbcVisitUsLink: string;
  provincialCourtOfBCVisitUsLink: string;
  courthouseServicesOfBCVisitUsLink: string;
}

@Injectable({
  providedIn: 'root',
})
export class AppConfigService {
  private appConfig: AppConfig;

  private RSBC_VISIT_US_DEFAULT =
    'https://www2.gov.bc.ca/gov/content/transportation/driving-and-cycling/roadsafetybc/intersection-safety-cameras' as const;
  private ICBC_VISIT_US_DEFAULT =
    'https://www.icbc.com/driver-licensing/tickets/Pages/default.aspx' as const;
  private PROV_CRT_VISIT_US_DEFAULT =
    'https://www.provincialcourt.bc.ca/types-of-cases/traffic-and-bylaw-matters' as const;
  private CTH_SERV_VISIT_US_DEFAULT =
    'https://www2.gov.bc.ca/gov/content/justice/courthouse-services/fines-payments' as const;

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

  get understandYourTicketLink(): string {
    return this.appConfig?.understandYourTicketLink;
  }

  get paymentOptionsLink(): string {
    return this.appConfig?.paymentOptionsLink;
  }

  get resolutionOptionsLink(): string {
    return this.appConfig?.resolutionOptionsLink;
  }

  get roadSafetyBCVisitUsLink(): string {
    const link = this.appConfig?.roadSafetyBCVisitUsLink;
    if (link) {
      return link;
    }
    return this.RSBC_VISIT_US_DEFAULT;
  }

  get icbcVisitUsLink(): string {
    const link = this.appConfig?.icbcVisitUsLink;
    if (link) {
      return link;
    }
    return this.ICBC_VISIT_US_DEFAULT;
  }

  get provincialCourtOfBCVisitUsLink(): string {
    const link = this.appConfig?.provincialCourtOfBCVisitUsLink;
    if (link) {
      return link;
    }
    return this.PROV_CRT_VISIT_US_DEFAULT;
  }

  get courthouseServicesOfBCVisitUsLink(): string {
    const link = this.appConfig?.courthouseServicesOfBCVisitUsLink;
    if (link) {
      return link;
    }
    return this.CTH_SERV_VISIT_US_DEFAULT;
  }
}
