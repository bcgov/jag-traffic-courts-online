import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LandingComponent } from './landing/landing.component';
import { SharedModule } from '@shared/shared.module';
import { DashboardModule } from '@dashboard/dashboard.module';
import { AuthRoutingModule } from './auth-routing.module';
import { AuthComponent } from './auth/auth.component';

@NgModule({
  declarations: [AuthComponent, LandingComponent],
  imports: [CommonModule, SharedModule, DashboardModule],
  exports: [AuthRoutingModule],
})
export class AuthModule {}
