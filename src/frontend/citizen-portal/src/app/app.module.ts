import {
  HttpClient,
  HttpClientModule,
} from '@angular/common/http';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgBusyModule } from 'ng-busy';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ConfigModule } from './config/config.module';
import { CoreModule } from './core/core.module';
import { NgxMaterialModule } from './shared/modules/ngx-material/ngx-material.module';
import { SharedModule } from './shared/shared.module';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { LandingComponent } from './components/landing/landing.component';
import { MatStepperModule } from '@angular/material/stepper';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { FindTicketComponent } from './components/find-ticket/find-ticket.component';
import { DisputeSubmitSuccessComponent } from './components/dispute-submit-success/dispute-submit-success.component';
import { InitiateResolutionComponent } from './components/initiate-resolution/initiate-resolution.component';
import { EmailVerificationRequiredComponent } from './components/email-verification-required/email-verification-required.component';
import { EmailVerificationComponent } from './components/email-verification/email-verification.component';
import { AppConfigService } from './services/app-config.service';

import localeEn from '@angular/common/locales/en';
import localeFr from '@angular/common/locales/fr';
import { registerLocaleData } from '@angular/common';
import { WindowRefService } from '@core/services/window-ref.service';
import { TicketPageComponent } from './components/ticket-page/ticket-page.component';
import { DisputeTicketSummaryComponent } from './components/dispute-ticket-summary/dispute-ticket-summary.component';
import { ScanTicketComponent } from '@components/scan-ticket/scan-ticket.component';
import { DisputeTicketStepperComponent } from '@components/dispute-ticket-stepper/dispute-ticket-stepper.component';
import { CountSummaryComponent } from './components/count-summary/count-summary.component';
import { CountItemSummaryComponent } from './components/count-item-summary/count-item-summary.component';
import { CountItemDisputeSummaryComponent } from './components/count-item-dispute-summary/count-item-dispute-summary.component';
import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { CdkAccordionModule } from '@angular/cdk/accordion';
import { TicketTypePipe } from '@shared/pipes/ticket-type.pipe';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { FormsModule } from '@angular/forms';
import { PdfViewerModule } from 'ng2-pdf-viewer';

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

@NgModule({
  declarations: [
    AppComponent,
    LandingComponent,
    FindTicketComponent,
    DisputeSubmitSuccessComponent,
    InitiateResolutionComponent,
    DisputeTicketStepperComponent,
    TicketPageComponent,
    DisputeTicketSummaryComponent,
    ScanTicketComponent,
    CountSummaryComponent,
    CountItemSummaryComponent,
    CountItemDisputeSummaryComponent,
    EmailVerificationRequiredComponent,
    EmailVerificationComponent,
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
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: false,
      extend: true,
    })
  ],
  exports: [
    NgBusyModule,
    NgxMaterialModule,
    TranslateModule
  ],
  providers: [
    DatePipe,
    CurrencyPipe,
    AppConfigService,
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

  constructor(private translateService: TranslateService) {
    this.translateService.addLangs(['en', 'fr']);

    const currentLanguage = window.navigator.language.substring(0, 2);

    let defaultLanguage = 'en';
    if (this.availableLanguages.includes(currentLanguage)) {
      defaultLanguage = currentLanguage;
    }
    this.translateService.setDefaultLang(defaultLanguage);
  }
}
