import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LandingComponent } from './landing/landing.component';
import { SharedModule } from '@shared/shared.module';
import { AuthRoutingModule } from './auth-routing.module';
import { AuthComponent } from './auth/auth.component';

@NgModule({
  declarations: [AuthComponent, LandingComponent],
  imports: [CommonModule, SharedModule],
  exports: [AuthRoutingModule],
})
export class AuthModule {}
