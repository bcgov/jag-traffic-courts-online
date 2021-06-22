import { Component, OnInit } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';
import { ConfigService } from '@config/config.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  constructor(
    private translateService: TranslateService,
    private titleService: Title,
    private configService: ConfigService,
    private metaTagService: Meta
  ) {}

  public ngOnInit(): void {
    this.metaTagService.addTags([
      { name: 'title', content: 'Violation and traffic ticket information in British Columbia' },
      { name: 'description', content: 'Looking for information on your BC violation or traffic ticket? Understand your ticket options and find out how to pay fines or dispute.' },
      { name: 'keywords', content: 'Pay ticket, Pay traffic fine, Pay violation ticket, Pay fines, BC fines, Pay court fine, Pay traffic ticket, Pay ticket online, Check traffic fines, How to pay tickets online, Where to pay traffic ticket, Check fines online, Pay ticket online, Ticket fine payment, BC pay ticket, BC ticket dispute, BC ticket resolution, How to understand your ticket, What does my ticket mean, Understand my traffic ticket' },
    ]);

    this.translateService
      .get([
        'app_heading',
        'toaster.dispute_submitted',
        'toaster.dispute_validation_error',
        'toaster.ticket_error',
        'toaster.dispute_create_error',
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
        this.configService.dispute_create_error$.next(
          this.translateService.instant('toaster.dispute_create_error')
        );
      });
  }
}
