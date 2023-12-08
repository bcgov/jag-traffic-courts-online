import { Component, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { NavigationEnd, Router, Scroll } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AppConfigService } from 'app/services/app-config.service';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';
import { filter } from 'rxjs';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
})
export class HeaderComponent implements OnInit {
  fullName: string;
  todayDate: Date = new Date();

  languageCode: string;
  languageDesc: string;
  btnLabel: string = 'IDIR Sign in';
  btnIcon: string = 'login';

  environment: string;
  version: string;
  isLoggedIn: Boolean = false;
  userProfile: KeycloakProfile = {};
  jjRole: boolean = false;
  vtcRole: boolean = false;
  headingText: string;

  constructor(
    private appConfigService: AppConfigService,
    private translateService: TranslateService,
    private router: Router,
    private title: Title,
    private authService: AuthService,
  ) {
    this.languageCode = this.translateService.getDefaultLang();
    this.onLanguage();

    this.environment = this.appConfigService.environment;
    this.version = this.appConfigService.version;
  }

  ngOnInit() {
    this.router.events
      .pipe(filter((event: Scroll) => event.routerEvent instanceof NavigationEnd))
      .subscribe(() => {
        this.headingText = this.title.getTitle();
      });

    this.authService.checkAuth().subscribe(() => {
      this.authService.isLoggedIn$.subscribe(isLoggedIn => {
        this.isLoggedIn = isLoggedIn;
        this.fullName = this.authService.userProfile?.fullName;
      })
    })
  }

  private toggleLanguage(lang: string): {
    languageCode: string;
    languageDesc: string;
  } {
    const toggleLang = lang === 'en' ? 'fr' : 'en';
    return {
      languageCode: toggleLang,
      languageDesc: toggleLang === 'en' ? 'English' : 'Français',
    };
  }

  onLanguage(): void {
    this.translateService.setDefaultLang(this.languageCode);
    const { languageCode, languageDesc } = this.toggleLanguage(
      this.languageCode
    );
    this.languageCode = languageCode;
    this.languageDesc = languageDesc;
  }

  login() {
    this.authService.login();
  }

  logout() {
    this.authService.logout();
  }
}
