import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LandingComponent } from './landing/landing.component';
import { SharedModule } from '@shared/shared.module';
import { DashboardModule } from '@dashboard/dashboard.module';
import { AuthRoutingModule } from './auth-routing.module';
import { AuthComponent } from './auth/auth.component';
import { FindTicketComponent } from './find-ticket/find-ticket.component';
import { CoreModule } from 'keycloak-angular';

@NgModule({
  declarations: [AuthComponent, LandingComponent, FindTicketComponent],
  imports: [CommonModule, SharedModule, DashboardModule],
  exports: [AuthRoutingModule],
})
export class AuthModule {}
