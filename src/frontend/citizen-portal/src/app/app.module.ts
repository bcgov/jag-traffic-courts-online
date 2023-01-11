import { HttpClient, HttpClientModule, } from '@angular/common/http';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe, registerLocaleData } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { CdkAccordionModule } from '@angular/cdk/accordion';
import { MatStepperModule } from '@angular/material/stepper';
import { FlexLayoutModule } from '@angular/flex-layout';
import { NgBusyModule } from 'ng-busy';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ConfigModule } from './config/config.module';
import { CoreModule } from './core/core.module';
import { WindowRefService } from '@core/services/window-ref.service';
import { NgxMaterialModule } from './shared/modules/ngx-material/ngx-material.module';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import localeEn from '@angular/common/locales/en';
import localeFr from '@angular/common/locales/fr';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { PdfViewerModule } from 'ng2-pdf-viewer';
import { SharedModule } from './shared/shared.module';
import { TicketTypePipe } from '@shared/pipes/ticket-type.pipe';
import { AppConfig, AppConfigService } from './services/app-config.service';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { DisputeStore, reducers } from './store';
import { LandingComponent } from './components/landing/landing.component';
import { TcoPageComponent } from './components/tco-page/tco-page.component';
import { DisputeTicketSummaryComponent } from './components/dispute-ticket-summary/dispute-ticket-summary.component';
import { ScanTicketComponent } from '@components/scan-ticket/scan-ticket.component';
import { DisputeStepperComponent } from '@components/dispute-stepper/dispute-stepper.component';
import { CountSummaryComponent } from './components/initiate-resolution/count-summary/count-summary.component';
import { CountItemSummaryComponent } from './components/initiate-resolution/count-item-summary/count-item-summary.component';
import { CountItemDisputeSummaryComponent } from './components/dispute-ticket-summary/count-item-dispute-summary/count-item-dispute-summary.component';
import { FindTicketComponent } from './components/find-ticket/find-ticket.component';
import { DisputeSubmitSuccessComponent } from './components/dispute-submit-success/dispute-submit-success.component';
import { InitiateResolutionComponent } from './components/initiate-resolution/initiate-resolution.component';
import { EmailVerificationRequiredComponent } from './components/email-verification-required/email-verification-required.component';
import { EmailVerificationComponent } from './components/email-verification/email-verification.component';
import { TicketLandingComponent } from './components/ticket-landing/ticket-landing.component';
import { FindDisputeComponent } from '@components/find-dispute/find-dispute.component';
import { UpdateDisputeLandingComponent } from '@components/update-dispute-landing/update-dispute-landing.component';
import { DisputantFormComponent } from '@components/disputant-form/disputant-form.component';
import { UpdateDisputeContactComponent } from '@components/update-dispute-contact/update-dispute-contact.component';
import { AuthModule } from './auth/auth.module';

registerLocaleData(localeEn, 'en');
registerLocaleData(localeFr, 'fr');

export function HttpLoaderFactory(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

@NgModule({
  declarations: [
    AppComponent,
    LandingComponent,
    FindTicketComponent,
    DisputeSubmitSuccessComponent,
    InitiateResolutionComponent,
    DisputeStepperComponent,
    TcoPageComponent,
    DisputeTicketSummaryComponent,
    ScanTicketComponent,
    CountSummaryComponent,
    CountItemSummaryComponent,
    CountItemDisputeSummaryComponent,
    EmailVerificationRequiredComponent,
    EmailVerificationComponent,
    TicketLandingComponent,
    FindDisputeComponent,
    UpdateDisputeLandingComponent,
    DisputantFormComponent,
    UpdateDisputeContactComponent,
  ],
  imports: [
    CommonModule,
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    CoreModule,
    SharedModule,
    ConfigModule,
    HttpClientModule,
    MatStepperModule,
    CdkAccordionModule,
    NgxMaterialTimepickerModule,
    FormsModule,
    PdfViewerModule,
    FlexLayoutModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: false,
      extend: true,
    }),
    StoreModule.forRoot(reducers),
    EffectsModule.forRoot([DisputeStore.Effects]),
    AuthModule,
  ],
  exports: [
    NgBusyModule,
    NgxMaterialModule,
    TranslateModule
  ],
  providers: [
    DatePipe,
    CurrencyPipe,
    TicketTypePipe,
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { showError: true }
    },
    WindowRefService,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
  bootstrap: [AppComponent],
})

export class AppModule {
  private availableLanguages = ['en', 'fr'];

  constructor(
    private translateService: TranslateService,
    private appConfigService: AppConfigService,
    private appConfig: AppConfig
  ) {
    // Get from main.ts, no need to fetch again
    this.appConfigService.setAppConfig(this.appConfig);
    this.translateService.addLangs(['en', 'fr']);

    const currentLanguage = window.navigator.language.substring(0, 2);

    let defaultLanguage = 'en';
    if (this.availableLanguages.includes(currentLanguage)) {
      defaultLanguage = currentLanguage;
    }
    this.translateService.setDefaultLang(defaultLanguage);
  }
}
