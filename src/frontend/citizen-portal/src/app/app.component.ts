import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  constructor(
    private translateService: TranslateService,
    private titleService: Title
  ) {}

  public ngOnInit(): void {
    this.translateService.get(['app_heading']).subscribe((translations) => {
      this.titleService.setTitle(this.translateService.instant('app_heading'));
    });
  }
}
