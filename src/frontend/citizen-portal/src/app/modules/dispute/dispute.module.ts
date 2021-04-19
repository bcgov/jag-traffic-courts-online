import { NgModule } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { MatStepperModule } from '@angular/material/stepper';

import { SharedModule } from '@shared/shared.module';
import { DisputeRoutingModule } from './dispute-routing.module';
import { StepperComponent } from './components/stepper/stepper.component';
import { FindTicketComponent } from './components/find-ticket/find-ticket.component';
import { DisputePageComponent } from './components/dispute-page/dispute-page.component';
import { StepCountComponent } from './components/step-count/step-count.component';
import { StepOverviewComponent } from './components/step-overview/step-overview.component';
import { DisputeSubmitComponent } from './components/dispute-submit/dispute-submit.component';
import { DisputeListComponent } from './components/dispute-list/dispute-list.component';
import { FormatDatePipe } from '@shared/pipes/format-date.pipe';
import { DisputeSummaryComponent } from './components/dispute-summary/dispute-summary.component';
import { DisputeAllStepperComponent } from './components/dispute-all-stepper/dispute-all-stepper.component';
import { StepDisputantComponent } from './components/step-disputant/step-disputant.component';
import { StepAdditionalComponent } from './components/step-additional/step-additional.component';
import { StepSingleCountComponent } from './components/step-single-count/step-single-count.component';

@NgModule({
  declarations: [
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
  ],
  imports: [CommonModule, SharedModule, DisputeRoutingModule, MatStepperModule],
  providers: [CurrencyPipe, FormatDatePipe],
  exports: [DisputeRoutingModule],
})
export class DisputeModule {}
