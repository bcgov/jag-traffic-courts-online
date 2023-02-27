import { Injectable } from '@angular/core';

export interface IAppConfig {
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
  bcServicesCardInfoLink: string;
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
  understandYourTicketLink: string;
  paymentOptionsLink: string;
  resolutionOptionsLink: string;
  roadSafetyBCVisitUsLink: string;
  icbcVisitUsLink: string;
  provincialCourtOfBCVisitUsLink: string;
  courthouseServicesOfBCVisitUsLink: string;
  bcServicesCardInfoLink: string;
  features: {
    dispute: boolean;
  };
}

@Injectable({
  providedIn: 'root',
})
export class AppConfigService {
  private appConfig: AppConfig

  private UNDERSTAND_YOUR_TICKET_DEFAULT =
    'https://understandmyticket.gov.bc.ca/' as const;
  private PAYMENT_OPTIONS_DEFAULT =
    // eslint-disable-next-line max-len
    'https://www2.gov.bc.ca/gov/content/justice/courthouse-services/fines-payments/pay-dispute-ticket/prov-violation-tickets/pay-ticket' as const;
  private RESOLUTION_OPTIONS_DEFAULT =
    // eslint-disable-next-line max-len
    'https://www2.gov.bc.ca/gov/content/justice/courthouse-services/fines-payments/pay-dispute-ticket/prov-violation-tickets/dispute-ticket' as const;

  private RSBC_VISIT_US_DEFAULT =
    'https://www2.gov.bc.ca/gov/content/transportation/driving-and-cycling/roadsafetybc/intersection-safety-cameras' as const;
  private ICBC_VISIT_US_DEFAULT =
    'https://www.icbc.com/driver-licensing/tickets/Pages/default.aspx' as const;
  private PROV_CRT_VISIT_US_DEFAULT =
    'https://www.provincialcourt.bc.ca/types-of-cases/traffic-and-bylaw-matters' as const;
  private CTH_SERV_VISIT_US_DEFAULT =
    'https://www2.gov.bc.ca/gov/content/justice/courthouse-services/fines-payments' as const;

  private BC_SERVICES_CARD_APP_INFO_DEFAULT =
    'https://www2.gov.bc.ca/gov/content/governments/government-id/bcservicescardapp/download-app' as const;

  constructor(
  ) {
  }

  setAppConfig(appConfig: AppConfig) {
    this.appConfig = appConfig;
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
    const link = this.appConfig?.understandYourTicketLink;
    return link ? link : this.UNDERSTAND_YOUR_TICKET_DEFAULT;
  }

  get paymentOptionsLink(): string {
    const link = this.appConfig?.paymentOptionsLink;
    return link ? link : this.PAYMENT_OPTIONS_DEFAULT;
  }

  get resolutionOptionsLink(): string {
    const link = this.appConfig?.resolutionOptionsLink;
    return link ? link : this.RESOLUTION_OPTIONS_DEFAULT;
  }

  get roadSafetyBCVisitUsLink(): string {
    const link = this.appConfig?.roadSafetyBCVisitUsLink;
    return link ? link : this.RSBC_VISIT_US_DEFAULT;
  }

  get icbcVisitUsLink(): string {
    const link = this.appConfig?.icbcVisitUsLink;
    return link ? link : this.ICBC_VISIT_US_DEFAULT;
  }

  get provincialCourtOfBCVisitUsLink(): string {
    const link = this.appConfig?.provincialCourtOfBCVisitUsLink;
    return link ? link : this.PROV_CRT_VISIT_US_DEFAULT;
  }

  get courthouseServicesOfBCVisitUsLink(): string {
    const link = this.appConfig?.courthouseServicesOfBCVisitUsLink;
    return link ? link : this.CTH_SERV_VISIT_US_DEFAULT;
  }

  get bcServicesCardInfoLink(): string {
    const link = this.appConfig?.bcServicesCardInfoLink;
    return link ? link : this.BC_SERVICES_CARD_APP_INFO_DEFAULT;
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
