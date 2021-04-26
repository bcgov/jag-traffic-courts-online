import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UtilsService } from '@core/services/utils.service';
import { RouteUtils } from '@core/utils/route-utils.class';
import { SurveyJsRoutes } from '@survey/survey-js.routes';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss'],
})
export class LandingComponent implements AfterViewInit {
  constructor(private route: Router, private utilsService: UtilsService) {}

  public onStep1(): void {
    this.route.navigate([SurveyJsRoutes.routePath(SurveyJsRoutes.HOME)]);
  }

  public ngAfterViewInit(): void {
    this.utilsService.scrollTop();
  }
}
