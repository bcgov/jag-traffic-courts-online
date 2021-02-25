import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DisputeRoutes } from './dispute.routes';
import { StepperComponent } from './components/stepper/stepper.component';
import { DisputeDashboardComponent } from './components/dispute-dashboard/dispute-dashboard.component';
import { FindTicketComponent } from './components/find-ticket/find-ticket.component';
import { DisputePageComponent } from './components/dispute-page/dispute-page.component';
import { StepOverviewComponent } from './components/step-overview/step-overview.component';
import { StepCountComponent } from './components/step-count/step-count.component';
import { StepReviewTicketComponent } from './components/step-review-ticket/step-review-ticket.component';
import { StepCourtComponent } from './components/step-court/step-court.component';

const routes: Routes = [
  {
    path: DisputeRoutes.DISPUTE,
    component: DisputePageComponent, //DisputeDashboardComponent,
    children: [
      {
        path: DisputeRoutes.FIND,
        component: FindTicketComponent,
      },
      {
        path: DisputeRoutes.STEPPER,
        component: StepperComponent,
      },
      {
        path: DisputeRoutes.REVIEW_TICKET,
        component: StepReviewTicketComponent,
      },
      {
        path: DisputeRoutes.STEP_COUNT,
        component: StepCountComponent,
      },
      {
        path: DisputeRoutes.STEP_COURT,
        component: StepCourtComponent,
      },
      {
        path: DisputeRoutes.STEP_OVERVIEW,
        component: StepOverviewComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DisputeRoutingModule {}
