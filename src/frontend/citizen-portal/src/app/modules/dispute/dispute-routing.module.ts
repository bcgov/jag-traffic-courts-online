import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DisputeRoutes } from './dispute.routes';
import { StepperComponent } from './components/stepper/stepper.component';
import { DisputeDashboardComponent } from './components/dispute-dashboard/dispute-dashboard.component';
import { PartAComponent } from './components/part-a/part-a.component';
import { PartBComponent } from './components/part-b/part-b.component';
import { PartCComponent } from './components/part-c/part-c.component';
import { PartDComponent } from './components/part-d/part-d.component';
import { FindTicketComponent } from './components/find-ticket/find-ticket.component';
import { DisputePageComponent } from './components/dispute-page/dispute-page.component';
import { StepOverviewComponent } from './components/step-overview/step-overview.component';
import { StepCountComponent } from './components/step-count/step-count.component';
import { ReviewTicketComponent } from './components/review-ticket/review-ticket.component';

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
        path: DisputeRoutes.PART_A,
        component: PartAComponent,
      },
      {
        path: DisputeRoutes.PART_B,
        component: PartBComponent,
      },
      {
        path: DisputeRoutes.PART_C,
        component: PartCComponent,
      },
      {
        path: DisputeRoutes.PART_D,
        component: PartDComponent,
      },
      {
        path: DisputeRoutes.REVIEW_TICKET,
        component: ReviewTicketComponent,
      },
      {
        path: DisputeRoutes.STEP_COUNT,
        component: StepCountComponent,
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
