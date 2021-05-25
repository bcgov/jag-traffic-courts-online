import {
  HttpClient,
  HttpClientModule,
  HTTP_INTERCEPTORS,
} from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BackendHttpInterceptor } from '@core/interceptors/backend-http.interceptor';
import { NgBusyModule } from 'ng-busy';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ConfigModule } from './config/config.module';
import { CoreModule } from './core/core.module';
import { SurveyJsModule } from './modules/survey-js/survey-js.module';
import { NgxMaterialModule } from './shared/modules/ngx-material/ngx-material.module';
import { SharedModule } from './shared/shared.module';
import {
  TranslateModule,
  TranslateLoader,
  TranslateService,
} from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { LandingComponent } from './components/landing/landing.component';
import { MatStepperModule } from '@angular/material/stepper';
import { CommonModule } from '@angular/common';
import { FindTicketComponent } from './components/find-ticket/find-ticket.component';
import { StepperComponent } from './components/stepper/stepper.component';
import { DisputePageComponent } from './components/dispute-page/dispute-page.component';
import { StepCountComponent } from './components/step-count/step-count.component';
import { StepOverviewComponent } from './components/step-overview/step-overview.component';
import { StepAdditionalComponent } from './components/step-additional/step-additional.component';
import { DisputeSubmitComponent } from './components/dispute-submit/dispute-submit.component';
import { DisputeListComponent } from './components/dispute-list/dispute-list.component';
import { DisputeSummaryComponent } from './components/dispute-summary/dispute-summary.component';
import { DisputeAllStepperComponent } from './components/dispute-all-stepper/dispute-all-stepper.component';
import { StepDisputantComponent } from './components/step-disputant/step-disputant.component';
import { StepSingleCountComponent } from './components/step-single-count/step-single-count.component';
import localeEn from '@angular/common/locales/en';
import localeFr from '@angular/common/locales/fr';
import { registerLocaleData } from '@angular/common';
import { WebcamModule } from 'ngx-webcam';
import { PhotoComponent } from './components/photo/photo.component';

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
    StepperComponent,
    DisputePageComponent,
    StepCountComponent,
    StepOverviewComponent,
    StepAdditionalComponent,
    DisputeSubmitComponent,
    DisputeListComponent,
    DisputeSummaryComponent,
    DisputeAllStepperComponent,
    StepDisputantComponent,
    StepSingleCountComponent,
    PhotoComponent,
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
    SurveyJsModule,
    MatStepperModule,
    WebcamModule,
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
  exports: [NgBusyModule, NgxMaterialModule, TranslateModule],
  providers: [
    MockDisputeService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: BackendHttpInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {
  private availableLanguages = ['en', 'fr'];

  constructor(private translateService: TranslateService) {
    translateService.addLangs(['en', 'fr']);

    const currentLanguage = window.navigator.language.substring(0, 2);
    console.log('Current Browser Language', currentLanguage);

    let defaultLanguage = 'en';
    if (this.availableLanguages.includes(currentLanguage)) {
      defaultLanguage = currentLanguage;
    }
    translateService.setDefaultLang(defaultLanguage);
  }
}
