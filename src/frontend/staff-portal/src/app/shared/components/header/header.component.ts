import { Component, ChangeDetectionStrategy, Output, EventEmitter, Input, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AppConfigService } from 'app/services/app-config.service';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';
import { KeycloakService } from 'keycloak-angular';
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
  @Output() toggle: EventEmitter<void>;

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
  headingText: string = "Authenticating...";

  constructor(
    private appConfigService: AppConfigService,
    private translateService: TranslateService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private authService: AuthService,
    private keycloak: KeycloakService,
  ) {
    this.toggle = new EventEmitter<void>();

    this.languageCode = this.translateService.getDefaultLang();
    this.onLanguage();

    this.environment = this.appConfigService.environment;
    this.version = this.appConfigService.version;
  }

  async ngOnInit() {
    this.headingText = this.activatedRoute.snapshot?.data?.title ? this.activatedRoute.snapshot?.data?.title : "Authenticating...";
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.headingText = this.activatedRoute.snapshot?.data?.title ? this.activatedRoute.snapshot?.data?.title : "Authenticating...";
      });

    this.authService.checkAuth().subscribe(() => {
      this.authService.isLoggedIn$.subscribe(isLoggedIn => {
        this.isLoggedIn = isLoggedIn;
      })
    })
  }

  toggleSidenav(): void {
    this.toggle.emit();
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
    this.keycloak.logout();
  }
}
