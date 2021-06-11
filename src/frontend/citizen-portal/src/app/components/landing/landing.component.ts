import { AfterViewInit, Component } from '@angular/core';
import { Router } from '@angular/router';
import { UtilsService } from '@core/services/utils.service';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss'],
})
export class LandingComponent implements AfterViewInit {
  constructor(private route: Router, private utilsService: UtilsService) {}

  public ngAfterViewInit(): void {
    this.utilsService.scrollTop();
  }
}
