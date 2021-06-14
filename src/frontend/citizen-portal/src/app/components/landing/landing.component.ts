import { AfterViewInit, Component } from '@angular/core';
import { UtilsService } from '@core/services/utils.service';
import { AppConfigService } from 'app/services/app-config.service';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss'],
})
export class LandingComponent implements AfterViewInit {
  // Urls for the various links
  public understandYourTicketLink: string;
  public paymentOptionsLink: string;
  public resolutionOptionsLink: string;

  constructor(
    private utilsService: UtilsService,
    private appConfigService: AppConfigService
  ) {
    this.understandYourTicketLink =
      this.appConfigService.understandYourTicketLink;
    this.paymentOptionsLink = this.appConfigService.paymentOptionsLink;
    this.resolutionOptionsLink = this.appConfigService.resolutionOptionsLink;
  }

  public ngAfterViewInit(): void {
    this.utilsService.scrollTop();
  }
}
