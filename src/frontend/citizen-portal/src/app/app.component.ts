import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
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
    private configService: ConfigService
  ) {}

  public ngOnInit(): void {
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
        console.log('translations', translations);

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
