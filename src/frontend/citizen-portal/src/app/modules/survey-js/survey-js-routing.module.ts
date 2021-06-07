import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { SurveyJsRoutes } from './survey-js.routes';
import { SurveyPageComponent } from './survey-page/survey-page.component';

const routes: Routes = [
  {
    path: SurveyJsRoutes.MODULE_PATH,
    component: SurveyPageComponent,
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
})
export class SurveyJsRoutingModule {}
