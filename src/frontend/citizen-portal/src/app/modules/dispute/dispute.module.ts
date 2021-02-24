import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatStepperModule } from '@angular/material/stepper';

import { SharedModule } from '@shared/shared.module';
import { DashboardModule } from '../dashboard/dashboard.module';
import { DisputeRoutingModule } from './dispute-routing.module';
import { DisputeDashboardComponent } from './components/dispute-dashboard/dispute-dashboard.component';
import { PartDComponent } from './components/part-d/part-d.component';
import { PartCComponent } from './components/part-c/part-c.component';
import { PartBComponent } from './components/part-b/part-b.component';
import { PartAComponent } from './components/part-a/part-a.component';
import { StepperComponent } from './components/stepper/stepper.component';
import { FindTicketComponent } from './components/find-ticket/find-ticket.component';
import { DisputePageComponent } from './components/dispute-page/dispute-page.component';
import { ReviewTicketComponent } from './components/review-ticket/review-ticket.component';
import { StepReviewTicketComponent } from './components/step-review-ticket/step-review-ticket.component';
import { StepCountComponent } from './components/step-count/step-count.component';
import { StepOverviewComponent } from './components/step-overview/step-overview.component';

@NgModule({
  declarations: [
    DisputeDashboardComponent,
    FindTicketComponent,
    StepperComponent,
    PartAComponent,
    PartBComponent,
    PartCComponent,
    PartDComponent,
    DisputePageComponent,
    ReviewTicketComponent,
    StepReviewTicketComponent,
    StepCountComponent,
    StepOverviewComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    DashboardModule,
    DisputeRoutingModule,
    MatStepperModule,
  ],
  exports: [DisputeRoutingModule],
})
export class DisputeModule {}
