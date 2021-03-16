import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DisputeRoutes } from './dispute.routes';
import { StepperComponent } from './components/stepper/stepper.component';
import { FindTicketComponent } from './components/find-ticket/find-ticket.component';
import { DisputePageComponent } from './components/dispute-page/dispute-page.component';
import { DisputeSubmitComponent } from './components/dispute-submit/dispute-submit.component';
import { DisputeListComponent } from './components/dispute-list/dispute-list.component';
import { AuthGuard } from '@core/guards/auth.guard';

const routes: Routes = [
  {
    path: DisputeRoutes.DISPUTE,
    component: DisputePageComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: DisputeRoutes.LIST,
        component: DisputeListComponent,
      },
      {
        path: DisputeRoutes.FIND,
        component: FindTicketComponent,
      },
      {
        path: DisputeRoutes.SUCCESS,
        component: DisputeSubmitComponent,
      },
    ],
  },
  {
    path: DisputeRoutes.STEPPER,
    component: StepperComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DisputeRoutingModule {}
