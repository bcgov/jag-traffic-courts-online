import { Component } from '@angular/core';
import { AppConfigService } from 'app/services/app-config.service';

@Component({
  selector: 'app-ticket-landing',
  templateUrl: './ticket-landing.component.html',
  styleUrls: ['./ticket-landing.component.scss']
})

export class TicketLandingComponent {
  roadSafetyBCVisitUsLink: string;
  constructor(
    private appConfigService: AppConfigService
  ) {
    this.roadSafetyBCVisitUsLink = this.appConfigService.roadSafetyBCVisitUsLink;
  }
}
