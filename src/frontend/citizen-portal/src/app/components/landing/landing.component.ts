import { AfterViewInit, Component } from '@angular/core';
import { SnowplowService } from '@core/services/snowplow.service';
import { AppConfigService } from 'app/services/app-config.service';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss'],
})
export class LandingComponent implements AfterViewInit {
  // Urls for the various links
  understandYourTicketLink: string;
  paymentOptionsLink: string;
  disputeTicketLink: string;
  roadSafetyBCVisitUsLink: string;
  icbcVisitUsLink: string;
  provincialCourtOfBCVisitUsLink: string;
  courthouseServicesOfBCVisitUsLink: string;

  constructor(
    private appConfigService: AppConfigService,
    private snowplow: SnowplowService
  ) {
    this.understandYourTicketLink = this.appConfigService.understandYourTicketLink;
    this.paymentOptionsLink = this.appConfigService.paymentOptionsLink;
    this.disputeTicketLink = this.appConfigService.resolutionOptionsLink;
    this.roadSafetyBCVisitUsLink = this.appConfigService.roadSafetyBCVisitUsLink;
    this.icbcVisitUsLink = this.appConfigService.icbcVisitUsLink;
    this.provincialCourtOfBCVisitUsLink = this.appConfigService.provincialCourtOfBCVisitUsLink;
    this.courthouseServicesOfBCVisitUsLink = this.appConfigService.courthouseServicesOfBCVisitUsLink;
  }

  ngAfterViewInit(): void {
    // refresh link urls now that we set the links
    this.snowplow.refreshLinkClickTracking();
  }
}
