import { Component, ChangeDetectionStrategy, Output, EventEmitter, Input, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
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
  public fullName: string;
  todayDate: Date = new Date();
  @Input() public isMobile: boolean;
  @Input() public hasMobileSidemenu: boolean;
  @Output() public toggle: EventEmitter<void>;

  public languageCode: string;
  public languageDesc: string;
  public btnLabel: string = 'IDIR Sign in';
  public btnIcon: string = 'login';

  public environment: string;
  public version: string;
  public isLoggedIn: Boolean = false;
  public userProfile: KeycloakProfile = {};
  public jjRole: boolean = false;
  public vtcRole: boolean = false;
  public headingText: string = "Authenticating...";

  constructor(
    protected logger: LoggerService,
    private appConfigService: AppConfigService,
    private translateService: TranslateService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private authService: AuthService,
    public keycloak: KeycloakService,
  ) {
    this.hasMobileSidemenu = false;
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

  public toggleSidenav(): void {
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

  public onLanguage(): void {
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
