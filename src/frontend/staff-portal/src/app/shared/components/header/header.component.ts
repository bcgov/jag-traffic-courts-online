import {
  Component,
  ChangeDetectionStrategy,
  Output,
  EventEmitter,
  Input,
  OnInit,
  OnChanges,
} from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { TranslateService } from '@ngx-translate/core';
import { AppConfigService } from 'app/services/app-config.service';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
})
export class HeaderComponent implements OnInit, OnChanges {
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
    public keycloak: KeycloakService,
    ) {
    this.hasMobileSidemenu = false;
    this.toggle = new EventEmitter<void>();

    this.languageCode = this.translateService.getDefaultLang();
    this.onLanguage();

    this.environment = this.appConfigService.environment;
    this.version = this.appConfigService.version;
  }

  ngOnChanges() {
    if (this.router.url.includes("jjworkbench")) this.headingText = "JJ Workbench";
    else if (this.router.url.includes("ticket")) this.headingText = "Staff Workbench";
    else if (this.router.url.includes("unauthorized")) this.headingText = "Unauthorized";
    else if (!this.isLoggedIn) this.headingText = "Please sign in";
  }

  async ngOnInit() {
    this.isLoggedIn = await this.keycloak.isLoggedIn();

    if (this.isLoggedIn) {
      this.userProfile = await this.keycloak.loadUserProfile();
      this.fullName = this.userProfile.firstName + " " + this.userProfile.lastName;

      if (this.router.url.includes("jjworkbench")) this.headingText = "JJ Workbench";
      else if (this.router.url.includes("ticket")) this.headingText = "Staff Workbench";
      else if (this.router.url.includes("unauthorized")) this.headingText = "Unauthorized";
      else if (!this.isLoggedIn) this.headingText = "Please sign in";

    }
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

  async login() {
    await this.keycloak.login({
      redirectUri: window.location.toString()
    });
  }

  logout() {
    this.keycloak.logout();
    this.isLoggedIn = false;
  }
}
