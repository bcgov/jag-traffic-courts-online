import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TicketPageComponent } from '@components/ticket-page/ticket-page.component';
import { FeatureFlagGuard } from '@core/guards/feature-flag.guard';
import { AppRoutes } from './app.routes';
import { LandingComponent } from './components/landing/landing.component';
import { UnauthorizedComponent } from '@components/error/unauthorized/unauthorized.component';
import { AuthorizationGuard } from '@core/guards/auth-guard';
import { AutoLoginPartialRoutesGuard } from 'angular-auth-oidc-client';

const routes: Routes = [
  {
    path: '',
    component: LandingComponent,
  },
  {
    path: AppRoutes.TICKET,
    component: TicketPageComponent,
    canActivate: [AuthorizationGuard],
    data: { expectedRole: "vtc-user"}
  },
  {
    path: 'unauthorized',
    component: UnauthorizedComponent,
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
