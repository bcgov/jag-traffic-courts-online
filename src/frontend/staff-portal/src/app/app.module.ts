import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { APP_INITIALIZER, CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { NgBusyModule } from 'ng-busy';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ConfigModule } from './config/config.module';
import { Configuration } from './api';
import { SharedModule, } from './shared/shared.module';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { LandingComponent } from './components/landing/landing.component';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { KeycloakAngularModule, KeycloakService } from 'keycloak-angular';

import localeEn from '@angular/common/locales/en';
import localeFr from '@angular/common/locales/fr';
import { registerLocaleData } from '@angular/common';
import { CoreModule } from './core/core.module';

import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { CdkAccordionModule } from '@angular/cdk/accordion';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TicketPageComponent } from '@components/ticket-page/ticket-page.component';
import { UnauthorizedComponent } from '@components/error/unauthorized/unauthorized.component';
import { TicketInfoComponent } from '@components/ticket-info/ticket-info.component';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { ContactInfoComponent } from './components/contact-info/contact-info.component';
import { TicketStatusComponent } from './components/ticket-status/ticket-status.component';
import { TicketRequestComponent } from '@components/ticket-request/ticket-request.component';
import { JjWorkbenchDashboardComponent } from '@components/jj-workbench-dashboard/jj-workbench-dashboard.component';
import { StaffWorkbenchDashboardComponent } from '@components/staff-workbench-dashboard/staff-workbench-dashboard.component';
import { JJDisputeInboxComponent } from '@components/jj-dispute-inbox/jj-dispute-inbox.component';
import { JJDisputeDecisionInboxComponent } from '@components/jj-dispute-decision-inbox/jj-dispute-decision-inbox.component';
import { JJDisputeAssignmentsComponent } from '@components/jj-dispute-assignments/jj-dispute-assignments.component';
import { JJDisputeComponent } from '@components/jj-dispute/jj-dispute.component';
import { JJCountComponent } from '@components/jj-count/jj-count.component';
import { AuthService } from './services/auth.service';

registerLocaleData(localeEn, 'en');
registerLocaleData(localeFr, 'fr');

// export function appInit(appConfigService: AppConfigService) {
//   return () => {
//     return appConfigService.loadAppConfig();
//   };
// }

export function HttpLoaderFactory(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

function initializeKeycloak(keycloak: KeycloakService): () => Promise<void> {
  return async() => {
    await keycloak.init({
      config: {
        // url: 'https://dev.oidc.gov.bc.ca/auth',
        // realm: 'ezb8kej4',
        // clientId: 'tco-staff-portal',
        url: "https://oidc-0198bb-dev.apps.silver.devops.gov.bc.ca",
        realm: "traffic-court",
        clientId: "staff-portal"
      },
      initOptions: {
        onLoad: 'check-sso',
        silentCheckSsoRedirectUri: window.location.origin + '/assets/silent-check-sso.html',
      }
    });
  }
}

@NgModule({
  declarations: [
    AppComponent,
    LandingComponent,
    TicketPageComponent,
    UnauthorizedComponent,
    TicketInfoComponent,
    ContactInfoComponent,
    TicketStatusComponent,
    TicketRequestComponent,
    JjWorkbenchDashboardComponent,
    StaffWorkbenchDashboardComponent,
    JJDisputeInboxComponent,
    JJDisputeDecisionInboxComponent,
    JJDisputeAssignmentsComponent,
    JJDisputeComponent,
    JJCountComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,
    AppRoutingModule,
    KeycloakAngularModule,
    CoreModule,
    SharedModule,
    ConfigModule,
    HttpClientModule,
    CdkAccordionModule,
    BrowserAnimationsModule,
    NgxMaterialTimepickerModule,
    FormsModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: false,
      extend: true,
    }),
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  exports: [NgBusyModule, TranslateModule],
  providers: [
    CurrencyPipe,
    DatePipe,
    MockConfigService,
    {
      provide: APP_INITIALIZER,
      useFactory: initializeKeycloak,
      multi: true,
      deps: [KeycloakService]
    },
    AuthService,
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { showError: true }
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  private availableLanguages = ['en', 'fr'];

  constructor(private translateService: TranslateService) {
    this.translateService.addLangs(['en', 'fr']);

    const currentLanguage = window.navigator.language.substring(0, 2);
    // console.log('Current Browser Language', currentLanguage);

    let defaultLanguage = 'en';
    if (this.availableLanguages.includes(currentLanguage)) {
      defaultLanguage = currentLanguage;
    }
    this.translateService.setDefaultLang(defaultLanguage);
  }
}
