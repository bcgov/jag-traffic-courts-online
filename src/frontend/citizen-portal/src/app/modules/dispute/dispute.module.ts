import { NgModule } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { MatStepperModule } from '@angular/material/stepper';

import { SharedModule } from '@shared/shared.module';
import { DisputeRoutingModule } from './dispute-routing.module';
import { StepperComponent } from './components/stepper/stepper.component';
import { FindTicketComponent } from './components/find-ticket/find-ticket.component';
import { DisputePageComponent } from './components/dispute-page/dispute-page.component';
import { StepReviewTicketComponent } from './components/step-review-ticket/step-review-ticket.component';
import { StepCountComponent } from './components/step-count/step-count.component';
import { StepOverviewComponent } from './components/step-overview/step-overview.component';
import { StepCourtComponent } from './components/step-court/step-court.component';
import { DisputeSubmitComponent } from './components/dispute-submit/dispute-submit.component';
import { DisputeListComponent } from './components/dispute-list/dispute-list.component';
import { FormatDatePipe } from '@shared/pipes/format-date.pipe';
import { DisputeSummaryComponent } from './components/dispute-summary/dispute-summary.component';

@NgModule({
  declarations: [
    FindTicketComponent,
    StepperComponent,
    DisputePageComponent,
    StepReviewTicketComponent,
    StepCountComponent,
    StepOverviewComponent,
    StepCourtComponent,
    DisputeSubmitComponent,
    DisputeListComponent,
    DisputeSummaryComponent,
  ],
  imports: [CommonModule, SharedModule, DisputeRoutingModule, MatStepperModule],
  providers: [CurrencyPipe, FormatDatePipe],
  exports: [DisputeRoutingModule],
})
export class DisputeModule {}
