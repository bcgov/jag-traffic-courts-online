import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '@core/guards/auth.guard';
import { HomeComponent } from './home/home.component';
import { SurveyJsRoutes } from './survey-js.routes';
import { SurveyPageComponent } from './survey-page/survey-page.component';

const routes: Routes = [
  {
    path: SurveyJsRoutes.MODULE_PATH,
    component: SurveyPageComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: SurveyJsRoutes.HOME,
        component: HomeComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [AuthGuard]
})
export class SurveyJsRoutingModule {}
