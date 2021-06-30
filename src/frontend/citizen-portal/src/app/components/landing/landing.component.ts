import { AfterViewInit, Component } from '@angular/core';
import { UtilsService } from '@core/services/utils.service';
import { AppConfigService } from 'app/services/app-config.service';
import { SnowplowService } from '../../snowplow.service';

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
  public roadSafetyBCVisitUsLink: string;
  public icbcVisitUsLink: string;
  public provincialCourtOfBCVisitUsLink: string;
  public courthouseServicesOfBCVisitUsLink: string;

  constructor(
    private utilsService: UtilsService,
    private appConfigService: AppConfigService,
    private snowplow: SnowplowService
  ) {
    this.understandYourTicketLink = this.appConfigService.understandYourTicketLink;
    this.paymentOptionsLink = this.appConfigService.paymentOptionsLink;
    this.resolutionOptionsLink = this.appConfigService.resolutionOptionsLink;
    this.roadSafetyBCVisitUsLink = this.appConfigService.roadSafetyBCVisitUsLink;
    this.icbcVisitUsLink = this.appConfigService.icbcVisitUsLink;
    this.provincialCourtOfBCVisitUsLink = this.appConfigService.provincialCourtOfBCVisitUsLink;
    this.courthouseServicesOfBCVisitUsLink = this.appConfigService.courthouseServicesOfBCVisitUsLink;
  }

  public ngAfterViewInit(): void {
    // refresh link urls now that we set the links
    this.snowplow.refreshLinkClickTracking();
    this.utilsService.scrollTop();
  }
}
