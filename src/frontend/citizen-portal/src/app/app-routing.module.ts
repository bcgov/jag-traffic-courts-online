import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DisputeStepperComponent } from '@components/dispute-stepper/dispute-stepper.component';
import { ScanTicketComponent } from '@components/scan-ticket/scan-ticket.component';
import { TcoPageComponent } from '@components/tco-page/tco-page.component';
import { FeatureFlagGuard } from '@core/guards/feature-flag.guard';
import { AppRoutes } from './app.routes';
import { DisputeSubmitSuccessComponent } from './components/dispute-submit-success/dispute-submit-success.component';
import { InitiateResolutionComponent } from './components/initiate-resolution/initiate-resolution.component';
import { FindTicketComponent } from './components/find-ticket/find-ticket.component';
import { EmailVerificationRequiredComponent } from './components/email-verification-required/email-verification-required.component';
import { LandingComponent } from './components/landing/landing.component';
import { EmailVerificationComponent } from '@components/email-verification/email-verification.component';
import { TicketLandingComponent } from '@components/ticket-landing/ticket-landing.component';
import { FindDisputeComponent } from '@components/find-dispute/find-dispute.component';
import { UpdateDisputeLandingComponent } from '@components/update-dispute-landing/update-dispute-landing.component';
import { UpdateDisputeAuthComponent } from '@components/update-dispute-auth/update-dispute-auth.component';
import { UpdateDisputeContactComponent } from '@components/update-dispute-contact/update-dispute-contact.component';

const routes: Routes = [
  {
    path: '',
    component: TcoPageComponent,
    children: [
      {
        path: '',
        component: LandingComponent,
      },
      {
        path: AppRoutes.EMAILVERIFICATIONREQUIRED,
        component: EmailVerificationRequiredComponent,
      },
      {
        path: AppRoutes.EMAILVERIFICATION,
        component: EmailVerificationComponent
      },
      {
        path: AppRoutes.SUBMIT_SUCCESS,
        component: DisputeSubmitSuccessComponent,
      },
      {
        path: AppRoutes.TICKET,
        canActivate: [FeatureFlagGuard],
        data: { featureFlag: 'dispute' },
        children: [
          {
            path: '',
            component: TicketLandingComponent,
          },
          {
            path: AppRoutes.FIND,
            component: FindTicketComponent,
          },
          {
            path: AppRoutes.SCAN,
            component: ScanTicketComponent,
          },
          {
            path: AppRoutes.SUMMARY,
            component: InitiateResolutionComponent,
          },
          {
            path: AppRoutes.STEPPER,
            component: DisputeStepperComponent,
          },
        ],
      },
      {
        path: AppRoutes.DISPUTE,
        canActivate: [FeatureFlagGuard],
        data: { featureFlag: 'dispute' },
        children: [
          {
            path: AppRoutes.FIND_DISPUTE,
            component: FindDisputeComponent,
          },
          {
            path: AppRoutes.UPDATE_DISPUTE,
            component: UpdateDisputeLandingComponent,
          },
          {
            path: AppRoutes.UPDATE_DISPUTE_AUTH,
            component: UpdateDisputeAuthComponent,
          },
          {
            path: AppRoutes.UPDATE_DISPUTE_CONTACT,
            component: UpdateDisputeContactComponent,
          },
          {
            path: '',
            pathMatch: 'full',
            redirectTo: AppRoutes.FIND_DISPUTE
          }
        ],
      },
    ]
  },
  {
    path: '**',
    redirectTo: '/',
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { onSameUrlNavigation: 'reload' })],
  exports: [RouterModule],
})
export class AppRoutingModule { }
