import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TicketPageComponent } from '@components/ticket-page/ticket-page.component';
import { AppRoutes } from './app.routes';
import { LandingComponent } from './components/landing/landing.component';
import { UnauthorizedComponent } from '@components/error/unauthorized/unauthorized.component';
import { AuthorizationGuard } from '@core/guards/auth-guard';
import { JjWorkbenchDashboardComponent } from '@components/jj-workbench-dashboard/jj-workbench-dashboard.component';

let routes: Routes = [
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
    path: AppRoutes.JJWORKBENCH,
    component: JjWorkbenchDashboardComponent,
    canActivate: [AuthorizationGuard],
    data: { expectedRole: "vtc-user" } // TODO change to jj-user
  },
  {
    path: AppRoutes.UNAUTHORIZED,
    component: UnauthorizedComponent,
  },
  {
    path: '**',
    redirectTo: '/',
    pathMatch: 'full',
  },
];

// Replcae the starting "/" since it is not needed in RouterModule
routes.forEach(route => {
  if (route.path?.startsWith("/")) {
    route.path = route.path.replace("/", "");
  }
})

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
