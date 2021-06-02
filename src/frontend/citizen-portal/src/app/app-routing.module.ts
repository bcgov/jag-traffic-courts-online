import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PhotoComponent } from '@components/photo/photo.component';
import { AuthGuard } from '@core/guards/auth.guard';
import { AppRoutes } from './app.routes';
import { DisputeAllStepperComponent } from './components/dispute-all-stepper/dispute-all-stepper.component';
import { DisputeListComponent } from './components/dispute-list/dispute-list.component';
import { DisputePageComponent } from './components/dispute-page/dispute-page.component';
import { DisputeSubmitComponent } from './components/dispute-submit/dispute-submit.component';
import { DisputeSummaryComponent } from './components/dispute-summary/dispute-summary.component';
import { FindTicketComponent } from './components/find-ticket/find-ticket.component';
import { LandingComponent } from './components/landing/landing.component';
import { StepperComponent } from './components/stepper/stepper.component';

const routes: Routes = [
  {
    path: AppRoutes.DISPUTE,
    component: DisputePageComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: AppRoutes.LIST,
        component: DisputeListComponent,
      },
      {
        path: AppRoutes.FIND,
        component: FindTicketComponent,
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
        path: AppRoutes.PHOTO,
        component: PhotoComponent,
      },
    ],
  },
  {
    path: AppRoutes.STEPPER,
    component: StepperComponent,
  },
  {
    path: AppRoutes.ALL_STEPPER,
    component: DisputeAllStepperComponent,
  },
  {
    path: AppRoutes.LANDING,
    component: LandingComponent,
  },
  {
    path: 'survey',
    loadChildren: () =>
      import('./modules/survey-js/survey-js.module').then(
        (m) => m.SurveyJsModule
      ),
  },
  {
    path: '',
    redirectTo: '/landing',
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
