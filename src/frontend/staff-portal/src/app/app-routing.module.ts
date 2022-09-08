import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppRoutes } from './app.routes';
import { LandingComponent } from './components/landing/landing.component';
import { UnauthorizedComponent } from '@components/error/unauthorized/unauthorized.component';
import { AuthorizationGuard } from '@core/guards/auth-guard';
import { JjWorkbenchDashboardComponent } from '@components/jj-workbench-dashboard/jj-workbench-dashboard.component';
import { StaffWorkbenchDashboardComponent } from '@components/staff-workbench-dashboard/staff-workbench-dashboard.component';

let routes: Routes = [
  {
    path: AppRoutes.LANDING,
    component: LandingComponent,
    data: {
      title: "Please sign in"
    }
  },
  {
    path: AppRoutes.TICKET,
    component: StaffWorkbenchDashboardComponent,
    canActivate: [AuthorizationGuard],
    data: {
      expectedRole: "vtc-staff",
      title: "Staff Workbench"
    }
  },
  {
    path: AppRoutes.JJWORKBENCH,
    component: JjWorkbenchDashboardComponent,
    canActivate: [AuthorizationGuard],
    data: {
      expectedRole: "judicial-justice",
      title: "JJ Workbench"
    }
  },
  {
    path: AppRoutes.UNAUTHORIZED,
    component: UnauthorizedComponent,
    data: {
      title: "Unauthorized"
    }
  },
  {
    path: '**',
    redirectTo: AppRoutes.LANDING,
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
