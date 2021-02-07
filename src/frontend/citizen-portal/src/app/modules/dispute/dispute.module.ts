import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {MatStepperModule } from '@angular/material/stepper'

import { HomeComponent } from './components/home/home.component';
import { SharedModule } from '@shared/shared.module';
import { DashboardModule } from '../dashboard/dashboard.module';
import { DisputeRoutingModule } from './dispute-routing.module';
import { DisputeDashboardComponent } from './components/dispute-dashboard/dispute-dashboard.component';
import { OverviewComponent } from './components/overview/overview.component';
import { PartDComponent } from './components/part-d/part-d.component';
import { PartCComponent } from './components/part-c/part-c.component';
import { PartBComponent } from './components/part-b/part-b.component';
import { PartAComponent } from './components/part-a/part-a.component';
import { StartComponent } from './components/start/start.component';

@NgModule({
  declarations: [
    DisputeDashboardComponent,
    HomeComponent,
    PartAComponent,
    PartBComponent,
    PartCComponent,
    PartDComponent,
    OverviewComponent,
    StartComponent,
  ],
  imports: [CommonModule, SharedModule, DashboardModule, DisputeRoutingModule, MatStepperModule],
  exports: [DisputeRoutingModule],
})
export class DisputeModule {}
