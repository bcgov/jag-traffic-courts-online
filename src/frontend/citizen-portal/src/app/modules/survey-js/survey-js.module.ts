import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SurveyJsRoutingModule } from './survey-js-routing.module';
import { HomeComponent } from './home/home.component';
import { SurveyPageComponent } from './survey-page/survey-page.component';
import { SharedModule } from '@shared/shared.module';

@NgModule({
  declarations: [HomeComponent, SurveyPageComponent],
  imports: [CommonModule, SurveyJsRoutingModule, SharedModule],
  exports: [SurveyJsRoutingModule],
})
export class SurveyJsModule {}
