import { provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ConfigModule } from './config/config.module';
import { SharedModule } from './shared/shared.module';
import { provideNgProgressOptions } from 'ngx-progressbar';
import { provideTranslateService, TranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { LandingComponent } from './components/landing/landing.component';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { CustomDatePipe as DatePipe } from '@shared/pipes/custom-date.pipe';
import { INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG, IncludeBearerTokenCondition, createInterceptorCondition, includeBearerTokenInterceptor, provideKeycloak, withAutoRefreshToken, AutoRefreshTokenService, UserActivityService } from 'keycloak-angular';
import { KeycloakConfig } from 'keycloak-js';

import localeEn from '@angular/common/locales/en';
import localeFr from '@angular/common/locales/fr';
import { registerLocaleData } from '@angular/common';
import { CoreModule } from './core/core.module';

import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { CdkAccordionModule } from '@angular/cdk/accordion';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TicketInboxComponent } from '@components/staff-workbench/ticket-inbox/ticket-inbox.component';
import { UpdateRequestInboxComponent } from '@components/staff-workbench/update-request-inbox/update-request-inbox.component';
import { UnauthorizedComponent } from '@components/error/unauthorized/unauthorized.component';
import { TicketInfoComponent } from '@components/staff-workbench/ticket-info/ticket-info.component';
import { ManualDisputeEntryComponent } from '@components/staff-workbench/manual-dispute-entry/manual-dispute-entry.component';
import { UpdateRequestInfoComponent } from '@components/staff-workbench/update-request-info/update-request-info.component';
import { PhoneUpdateRequestInfoComponent } from '@components/staff-workbench/update-request-info/phone-update-request-info/phone-update-request-info.component';
import { NameUpdateRequestInfoComponent } from '@components/staff-workbench/update-request-info/name-update-request-info/name-update-request-info.component';
import { AddressUpdateRequestInfoComponent } from '@components/staff-workbench/update-request-info/address-update-request-info/address-update-request-info.component';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { ContactInfoComponent } from './components/staff-workbench/contact-info/contact-info.component';
import { TicketStatusComponent } from './components/staff-workbench/ticket-status/ticket-status.component';
import { TicketRequestComponent } from '@components/staff-workbench/ticket-request/ticket-request.component';
import { JjWorkbenchDashboardComponent } from '@components/jj-workbench/jj-workbench-dashboard/jj-workbench-dashboard.component';
import { StaffWorkbenchDashboardComponent } from '@components/staff-workbench/staff-workbench-dashboard/staff-workbench-dashboard.component';
import { JJDisputeWRInboxComponent } from '@components/jj-workbench/jj-dispute-wr-inbox/jj-dispute-wr-inbox.component';
import { JJDisputeHearingInboxComponent } from '@components/jj-workbench/jj-dispute-hearing-inbox/jj-dispute-hearing-inbox.component';
import { DisputeDecisionInboxComponent } from '@components/staff-workbench/dispute-decision-inbox/dispute-decision-inbox.component';
import { JJDisputeWRAssignmentsComponent } from '@components/jj-workbench/jj-dispute-wr-assignments/jj-dispute-wr-assignments.component';
import { JJDisputeComponent } from '@components/jj-dispute-info/jj-dispute/jj-dispute.component';
import { JJCountComponent } from '@components/jj-dispute-info/jj-count/jj-count.component';
import { JJDisputeRemarksComponent } from '@components/jj-dispute-info/jj-dispute-remarks/jj-dispute-remarks.component';
import { JJDisputeCourtAppearancesComponent } from '@components/jj-dispute-info/jj-dispute-court-appearances/jj-dispute-court-appearances.component';
import { JJFileHistoryComponent } from '@components/jj-dispute-info/jj-file-history/jj-file-history.component';
import { JJDisputeDigitalCaseFileComponent } from '@components/jj-workbench/jj-dispute-digital-case-file/jj-dispute-digital-case-file.component';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { reducers, JJDisputeStore } from './store';
import { CourtOptionsUpdateRequestInfoComponent } from '@components/staff-workbench/update-request-info/court-options-update-request-info/court-options-update-request-info.component';
import { CountUpdateRequestInfoComponent } from '@components/staff-workbench/update-request-info/count-update-request-info/count-update-request-info.component';
import { DocumentUpdateRequestInfoComponent } from '@components/staff-workbench/update-request-info/document-update-request-info/document-update-request-info.component';
import { TableFiltersComponent } from '@components/table-filters/table-filters.component';
import { JjDisputeUpdatesComponent } from './components/jj-dispute-info/jj-dispute-updates/jj-dispute-updates.component';
import { PagingComponent } from '@components/paging/paging.component';
import { UploadComponent } from './components/staff-workbench/upload/upload.component';
import { BsDatepickerConfig, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { ClickOutsideDirective } from './directives/click-outside.directive';
import { ProgressOptions } from '@shared/modules/ngx-progress/ngx-progress.options';
import { progressInterceptor } from 'ngx-progressbar/http';

export function createAppModule(keycloakConfig: KeycloakConfig) {

registerLocaleData(localeEn, 'en');
registerLocaleData(localeFr, 'fr');

@NgModule({
  declarations: [
    AppComponent,
    LandingComponent,
    TicketInboxComponent,
    UpdateRequestInboxComponent,
    UnauthorizedComponent,
    TicketInfoComponent,
    ManualDisputeEntryComponent,
    UpdateRequestInfoComponent,
    PhoneUpdateRequestInfoComponent,
    DocumentUpdateRequestInfoComponent,
    NameUpdateRequestInfoComponent,
    AddressUpdateRequestInfoComponent,
    CourtOptionsUpdateRequestInfoComponent,
    CountUpdateRequestInfoComponent,
    ContactInfoComponent,
    TicketStatusComponent,
    TicketRequestComponent,
    JjWorkbenchDashboardComponent,
    StaffWorkbenchDashboardComponent,
    JJDisputeWRInboxComponent,
    JJDisputeHearingInboxComponent,
    DisputeDecisionInboxComponent,
    JJDisputeWRAssignmentsComponent,
    JJDisputeComponent,
    JJCountComponent,
    JJDisputeRemarksComponent,
    JJDisputeCourtAppearancesComponent,
    JJFileHistoryComponent,
    JJDisputeDigitalCaseFileComponent,
    TableFiltersComponent,
    JjDisputeUpdatesComponent,
    PagingComponent,
    UploadComponent,
    ClickOutsideDirective
  ],
  imports: [
    CommonModule,
    BrowserModule,
    AppRoutingModule,
    CoreModule,
    SharedModule,
    ConfigModule,
    CdkAccordionModule,
    BrowserAnimationsModule,
    FormsModule,
    StoreModule.forRoot(reducers),
    EffectsModule.forRoot([JJDisputeStore.Effects]),
    BsDatepickerModule.forRoot(),
    NgMultiSelectDropDownModule.forRoot()
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  exports: [],
  providers: [
    CurrencyPipe,
    DatePipe,
    MockConfigService,
    provideKeycloak({
      config: keycloakConfig,
      initOptions: {
        onLoad: "check-sso",
        silentCheckSsoRedirectUri: window.location.origin + "/assets/silent-check-sso.html",
      },
      features: [
        withAutoRefreshToken({
          onInactivityTimeout: 'login',
          sessionTimeout: 3600000 // 60 minutes
        })
      ],
      providers: [AutoRefreshTokenService, UserActivityService]
    }),
    {
      provide: INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG,
      useValue: [
        createInterceptorCondition<IncludeBearerTokenCondition>({
          urlPattern: new RegExp(`^/api/.*$`, "i"),
        }),
      ]
    },
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { showError: true }
    },
    BsDatepickerConfig,
    provideNgProgressOptions(ProgressOptions),
    provideHttpClient(withInterceptorsFromDi(), withInterceptors([includeBearerTokenInterceptor, progressInterceptor])),
    provideTranslateService({
      loader: provideTranslateHttpLoader({ prefix: './assets/i18n/', suffix: '.json'}),
      extend: true,
    })
  ],
  bootstrap: [AppComponent]
})
class AppModule {
  private availableLanguages = ['en', 'fr'];

  constructor(private translateService: TranslateService) {
    this.translateService.addLangs(['en', 'fr']);

    const currentLanguage = window.navigator.language.substring(0, 2);
    // console.log('Current Browser Language', currentLanguage);

    let fallbackLanguage = 'en';
    if (this.availableLanguages.includes(currentLanguage)) {
      fallbackLanguage = currentLanguage;
    }
    this.translateService.setFallbackLang(fallbackLanguage);
  }
}

return AppModule;
}
