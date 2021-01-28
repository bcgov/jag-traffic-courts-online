import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () =>
      import('./modules/auth/auth.module').then((m) => m.AuthModule),
  },
  {
    path: 'survey',
    loadChildren: () =>
      import('./modules/survey-js/survey-js.module').then(
        (m) => m.SurveyJsModule
      ),
  },
  {
    path: 'dispute',
    loadChildren: () =>
      import('./modules/dispute/dispute.module').then((m) => m.DisputeModule),
  },
  {
    path: '',
    redirectTo: '/auth/landing',
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
