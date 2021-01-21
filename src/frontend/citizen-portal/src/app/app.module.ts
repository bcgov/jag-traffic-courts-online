import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { NgBusyModule } from 'ng-busy';
import { NgxMaterialModule } from './shared/modules/ngx-material/ngx-material.module';
import { DisputeModule } from './modules/dispute/dispute.module';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';
import { AuthModule } from './modules/auth/auth.module';
import { DashboardModule } from '@dashboard/dashboard.module';

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
  ],
  exports: [NgBusyModule, NgxMaterialModule],
  providers: [MockDisputeService],
  bootstrap: [AppComponent],
})
export class AppModule {}
