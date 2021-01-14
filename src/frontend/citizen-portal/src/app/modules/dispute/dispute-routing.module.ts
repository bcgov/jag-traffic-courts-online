import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DisputeRoutes } from './dispute.routes';
import { HomeComponent } from './components/home/home.component';
import { DisputeDashboardComponent } from './components/dispute-dashboard/dispute-dashboard.component';
import { PartAComponent } from './components/part-a/part-a.component';
import { PartBComponent } from './components/part-b/part-b.component';
import { PartCComponent } from './components/part-c/part-c.component';
import { PartDComponent } from './components/part-d/part-d.component';
import { OverviewComponent } from './components/overview/overview.component';

const routes: Routes = [
  {
    path: DisputeRoutes.DISPUTE,
    component: DisputeDashboardComponent,
    children: [
      {
        path: DisputeRoutes.HOME,
        component: HomeComponent,
      },
      {
        path: DisputeRoutes.PART_A,
        component: PartAComponent,
      },
      {
        path: DisputeRoutes.PART_B,
        component: PartBComponent,
      },
      {
        path: DisputeRoutes.PART_C,
        component: PartCComponent,
      },
      {
        path: DisputeRoutes.PART_D,
        component: PartDComponent,
      },
      {
        path: DisputeRoutes.OVERVIEW,
        component: OverviewComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DisputeRoutingModule {}
