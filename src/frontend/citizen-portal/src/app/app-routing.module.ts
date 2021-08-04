import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DisputeStepperComponent } from '@components/dispute-stepper/dispute-stepper.component';
import { ShellTicketComponent } from '@components/shell-ticket/shell-ticket.component';
import { TicketImageComponent } from '@components/ticket-image/ticket-image.component';
import { TicketPageComponent } from '@components/ticket-page/ticket-page.component';
import { FeatureFlagGuard } from '@core/guards/feature-flag.guard';
import { AppRoutes } from './app.routes';
import { DisputeSubmitComponent } from './components/dispute-submit/dispute-submit.component';
import { DisputeSummaryComponent } from './components/dispute-summary/dispute-summary.component';
import { FindTicketComponent } from './components/find-ticket/find-ticket.component';
import { LandingComponent } from './components/landing/landing.component';

const routes: Routes = [
  {
    path: AppRoutes.TICKET,
    component: TicketPageComponent,
    canActivate: [FeatureFlagGuard],
    data: { featureFlag: 'dispute' },
    children: [
      {
        path: AppRoutes.SHELL,
        component: ShellTicketComponent,
      },
      {
        path: AppRoutes.IMAGE,
        component: TicketImageComponent,
      },
      {
        path: AppRoutes.SUCCESS,
        component: DisputeSubmitComponent,
      },
      {
        path: AppRoutes.SUMMARY,
        component: DisputeSummaryComponent,
      },
      {
        path: AppRoutes.STEPPER,
        component: DisputeStepperComponent,
      },
    ],
  },
  {
    path: AppRoutes.TICKET,
    component: TicketPageComponent,
    canActivate: [FeatureFlagGuard],
    data: { featureFlag: 'dispute' },
    children: [
      {
        path: AppRoutes.FIND,
        component: FindTicketComponent,
      },
    ],
  },
  {
    path: '',
    component: LandingComponent,
  },
  {
    path: '**',
    redirectTo: '/',
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
