import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
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
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AuthRoutingModule {}
