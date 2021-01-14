import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

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

@NgModule({
  declarations: [
    DisputeDashboardComponent,
    HomeComponent,
    PartAComponent,
    PartBComponent,
    PartCComponent,
    PartDComponent,
    OverviewComponent,
  ],
  imports: [CommonModule, SharedModule, DashboardModule, DisputeRoutingModule],
  exports: [DisputeRoutingModule],
})
export class DisputeModule {}
