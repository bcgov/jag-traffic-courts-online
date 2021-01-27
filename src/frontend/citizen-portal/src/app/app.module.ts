import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BackendHttpInterceptor } from '@core/interceptors/backend-http.interceptor';
import { NgBusyModule } from 'ng-busy';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { AuthModule } from './modules/auth/auth.module';
import { DisputeModule } from './modules/dispute/dispute.module';
import { SurveyJsModule } from './modules/survey-js/survey-js.module';
import { NgxMaterialModule } from './shared/modules/ngx-material/ngx-material.module';
import { SharedModule } from './shared/shared.module';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    CoreModule,
    SharedModule,
    AuthModule,
    DisputeModule,
    HttpClientModule,
    SurveyJsModule,
  ],
  exports: [NgBusyModule, NgxMaterialModule],
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
export class AppModule {}
