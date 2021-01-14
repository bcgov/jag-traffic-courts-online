import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'dispute',
    loadChildren: () =>
      import('./modules/dispute/dispute.module').then((m) => m.DisputeModule),
  },
  {
    path: '',
    redirectTo: '/dispute/home',
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
