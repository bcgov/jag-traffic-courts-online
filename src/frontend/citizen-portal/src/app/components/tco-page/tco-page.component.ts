import { Component, OnInit } from '@angular/core';
import { NavigationStart, Router } from '@angular/router';
import { AppConfigService } from 'app/services/app-config.service';
import { BehaviorSubject, filter } from 'rxjs';

@Component({
  selector: 'app-tco-page',
  templateUrl: './tco-page.component.html',
  styleUrls: ['./tco-page.component.scss'],
})
export class TcoPageComponent implements OnInit {
  isLanding$: BehaviorSubject<boolean> = new BehaviorSubject(true);
  understandYourTicketLink: string;
  paymentOptionsLink: string;

  constructor(
    private appConfigService: AppConfigService,
    private router: Router,
  ) {
    this.understandYourTicketLink = this.appConfigService.understandYourTicketLink;
    this.paymentOptionsLink = this.appConfigService.paymentOptionsLink;
  }

  ngOnInit(): void {
    let url = window.location.pathname;
    this.detectUrl(url);
    this.router.events.pipe(
      filter(event => event instanceof NavigationStart)
    ).subscribe((event: NavigationStart) => {
      this.detectUrl(event.url);
    });
  }

  private detectUrl(url) {
    if (url === "/") {
      this.isLanding$.next(true);
    } else {
      this.isLanding$.next(false);
    }
  }
}
