import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppRoutes } from './app.routes';
import { LandingComponent } from './components/landing/landing.component';
import { UnauthorizedComponent } from '@components/error/unauthorized/unauthorized.component';
import { AuthorizationGuard } from '@core/guards/auth-guard';
import { JjWorkbenchDashboardComponent } from '@components/jj-workbench/jj-workbench-dashboard/jj-workbench-dashboard.component';
import { StaffWorkbenchDashboardComponent } from '@components/staff-workbench/staff-workbench-dashboard/staff-workbench-dashboard.component';
import { UserGroup } from '@shared/enums/user-group.enum';

let routes: Routes = [
  {
    path: AppRoutes.LANDING,
    component: LandingComponent,
  },
  {
    path: AppRoutes.STAFF,
    canActivate: [AuthorizationGuard],
    data: { roles: [UserGroup.VTC_STAFF, UserGroup.SUPPORT_STAFF] },
    children: [
      {
        path: '',
        component: StaffWorkbenchDashboardComponent,
        title: "Staff Workbench",
      }
    ],
  },
  {
    path: AppRoutes.JJ,
    canActivate: [AuthorizationGuard],
    data: { roles: [UserGroup.JUDICIAL_JUSTICE, UserGroup.ADMIN_JUDICIAL_JUSTICE, UserGroup.SUPPORT_STAFF] },
    children: [
      {
        path: '',
        component: JjWorkbenchDashboardComponent,
        title: "JJ Workbench",
      }
    ],
  },
  {
    path: AppRoutes.UNAUTHORIZED,
    component: UnauthorizedComponent,
    title: "Unauthorized"
  },
  {
    path: 'ticket', // old routing
    redirectTo: AppRoutes.STAFF,
    pathMatch: 'full',
  },
  {
    path: 'jjworkbench', // old routing
    redirectTo: AppRoutes.JJ,
    pathMatch: 'full',
  },
  {
    path: '**',
    redirectTo: AppRoutes.LANDING,
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    onSameUrlNavigation: 'reload',
    bindToComponentInputs: true
  })],
  exports: [RouterModule],
})
export class AppRoutingModule { }
