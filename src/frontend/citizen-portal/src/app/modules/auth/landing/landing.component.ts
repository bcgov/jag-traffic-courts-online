import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RouteUtils } from '@core/utils/route-utils.class';
import { SurveyJsRoutes } from '@survey/survey-js.routes';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss'],
})
export class LandingComponent {
  constructor(private route: Router) {}

  public onStep1(): void {
    this.route.navigate([SurveyJsRoutes.routePath(SurveyJsRoutes.HOME)]);
  }
}
