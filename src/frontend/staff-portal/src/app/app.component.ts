import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd, RouterEvent } from '@angular/router';
import { Meta, Title } from '@angular/platform-browser';
import { ConfigService } from '@config/config.service';
import { TranslateService } from '@ngx-translate/core';
import { SnowplowService } from '@core/services/snowplow.service';
import { UtilsService } from '@core/services/utils.service';
import { RouteStateService } from '@core/services/route-state.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  //  debugger;
  constructor(
    private routeStateService: RouteStateService,
    private translateService: TranslateService,
    private titleService: Title,
    private configService: ConfigService,
    private metaTagService: Meta,
    private router: Router,
    private utilsService: UtilsService,
    private snowplow: SnowplowService,
  ) {
    this.router.events.subscribe((evt) => {
      if (evt instanceof NavigationEnd) {
        this.snowplow.trackPageView();
      }
    });
  }

  public ngOnInit(): void {
    const onNavEnd = this.routeStateService.onNavigationEnd();
    this.scrollTop(onNavEnd);

    this.metaTagService.addTags([
      {
        name: 'title',
        content: 'Violation and traffic ticket information in British Columbia',
      },
      {
        name: 'description',
        content:
          'Looking for information on your BC violation or traffic ticket? Understand your ticket options and find out how to pay fines or dispute.',
      },
      {
        name: 'keywords',
        content:
          'Pay ticket, Pay traffic fine, Pay violation ticket, Pay fines, BC fines, Pay court fine, Pay traffic ticket, Pay ticket online, Check traffic fines, How to pay tickets online, Where to pay traffic ticket, Check fines online, Pay ticket online, Ticket fine payment, BC pay ticket, BC ticket dispute, BC ticket resolution, How to understand your ticket, What does my ticket mean, Understand my traffic ticket',
      },
    ]);

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

    /**
   * @description
   * Scroll the page to the top on route event.
   */
  private scrollTop(routeEvent: Observable<RouterEvent>) {
    routeEvent.subscribe(() => this.utilsService.scrollTop());
  }
}
