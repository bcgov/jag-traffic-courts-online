import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '@core/guards/auth.guard';
import { AuthRoutes } from './auth.routes';
import { AuthComponent } from './auth/auth.component';
import { LandingComponent } from './landing/landing.component';

const routes: Routes = [
  {
    path: AuthRoutes.MODULE_PATH,
    component: AuthComponent,
    children: [
      {
        path: AuthRoutes.LANDING,
        component: LandingComponent,
      },
      {
        path: AuthRoutes.FIND,
        component: FindTicketComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard]
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [AuthGuard]
})
export class AuthRoutingModule {}
