import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { ConfigService } from '@config/config.service';
import { TranslateService } from '@ngx-translate/core';
import { SnowplowService } from '@core/services/snowplow.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  //  debugger;
  constructor(
    private translateService: TranslateService,
    private titleService: Title,
    private configService: ConfigService,
    private router: Router,
    private snowplow: SnowplowService,
  ) {
    this.router.events.subscribe((evt) => {
      if (evt instanceof NavigationEnd) {
        this.snowplow.trackPageView();
      }
    });
  }

  public ngOnInit(): void {
    this.translateService
      .get([
        'app_heading',
        'toaster.dispute_submitted',
        'toaster.dispute_validation_error',
        'toaster.ticket_error',
        'toaster.dispute_create_error',
        'toaster.dispute_error',
        'toaster.statute_error',
        'toaster.language_error'
      ])
      .subscribe((translations) => {
        this.titleService.setTitle(
          this.translateService.instant('app_heading')
        );

        this.configService.dispute_submitted$.next(
          this.translateService.instant('toaster.dispute_submitted')
        );
        this.configService.dispute_validation_error$.next(
          this.translateService.instant('toaster.dispute_validation_error')
        );
        this.configService.ticket_error$.next(
          this.translateService.instant('toaster.ticket_error')
        );
        this.configService.dispute_error$.next(
          this.translateService.instant('toaster.dispute_error')
        );
        this.configService.dispute_create_error$.next(
          this.translateService.instant('toaster.dispute_create_error')
        );
        this.configService.statute_error$.next(
          this.translateService.instant('toaster.statute_error')
        );
        this.configService.language_error$.next(
          this.translateService.instant('toaster.language_error')
        );
      });
  }
}
