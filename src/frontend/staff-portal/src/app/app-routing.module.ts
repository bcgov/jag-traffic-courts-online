import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TicketPageComponent } from '@components/ticket-page/ticket-page.component';
// import { DisputeStepperComponent } from '@components/dispute-stepper/dispute-stepper.component';
// import { ShellTicketComponent } from '@components/shell-ticket/shell-ticket.component';
// import { TicketPageComponent } from '@components/ticket-page/ticket-page.component';
// import { TicketPaymentCompleteComponent } from '@components/ticket-payment-complete/ticket-payment-complete.component';
// import { TicketPaymentComponent } from '@components/ticket-payment/ticket-payment.component';
import { FeatureFlagGuard } from '@core/guards/feature-flag.guard';
import { AppRoutes } from './app.routes';
// import { DisputeSubmitSuccessComponent } from './components/dispute-submit-success/dispute-submit-success.component';
// import { DisputeSummaryComponent } from './components/dispute-summary/dispute-summary.component';
// import { FindTicketComponent } from './components/find-ticket/find-ticket.component';
import { LandingComponent } from './components/landing/landing.component';
import { AutoLoginPartialRoutesGuard } from 'angular-auth-oidc-client';

const routes: Routes = [
  {
    path: '',
    component: LandingComponent,
  },
  {
    path: AppRoutes.TICKET,
    component: TicketPageComponent,
    // canActivate: [AutoLoginPartialRoutesGuard],
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
