import { AfterViewInit, Component } from '@angular/core';
import { Router } from '@angular/router';
import { UtilsService } from '@core/services/utils.service';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss'],
})
export class LandingComponent implements AfterViewInit {
  // Urls for the various links
  public understandYourTicketLink =
    'https://staging-bcptb.cs138.force.com/apex/ES_Launch?tn=BCTC';
  public paymentOptionsLink =
    'https://www2.gov.bc.ca/gov/content/justice/courthouse-services/fines-payments/pay-dispute-ticket/prov-violation-tickets/pay-ticket';
  public resolutionOptionsLink =
    'https://www2.gov.bc.ca/gov/content/justice/courthouse-services/fines-payments/pay-dispute-ticket/prov-violation-tickets/dispute-ticket';

  constructor(private route: Router, private utilsService: UtilsService) {}

  public ngAfterViewInit(): void {
    this.utilsService.scrollTop();
  }
}
