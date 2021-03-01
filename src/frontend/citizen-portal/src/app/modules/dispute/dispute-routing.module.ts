import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DisputeRoutes } from './dispute.routes';
import { StepperComponent } from './components/stepper/stepper.component';
import { FindTicketComponent } from './components/find-ticket/find-ticket.component';
import { DisputePageComponent } from './components/dispute-page/dispute-page.component';
import { DisputeSubmitComponent } from './components/dispute-submit/dispute-submit.component';

const routes: Routes = [
  {
    path: DisputeRoutes.DISPUTE,
    component: DisputePageComponent,
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
        path: DisputeRoutes.SUCCESS,
        component: DisputeSubmitComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DisputeRoutingModule {}
