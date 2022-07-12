import {
  Component,
  ChangeDetectionStrategy,
  Output,
  EventEmitter,
  Input,
  OnInit,
  OnChanges,
  AfterViewInit,
} from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { TranslateService } from '@ngx-translate/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { AppRoutes } from 'app/app.routes';
import { AppConfigService } from 'app/services/app-config.service';
import { JwtHelperService } from '@auth0/angular-jwt';

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
  @Input() public jjPage: string;

  public languageCode: string;
  public languageDesc: string;
  public btnLabel: string = 'IDIR Sign in';
  public btnIcon: string = 'login';

  public environment: string;
  public version: string;
  public isLoggedIn: Boolean = false;
  public jjRole: boolean = false;
  public vtcRole: boolean = false;
  public headingText: string = "Authenticating...";

  constructor(
    protected logger: LoggerService,
    private appConfigService: AppConfigService,
    private translateService: TranslateService,
    private oidcSecurityService: OidcSecurityService,
    private router: Router,
    public jwtHelper: JwtHelperService
  ) {
    this.hasMobileSidemenu = false;
    this.toggle = new EventEmitter<void>();

    this.languageCode = this.translateService.getDefaultLang();
    this.onLanguage();

    this.environment = this.appConfigService.environment;
    this.version = this.appConfigService.version;
  }

  ngOnChanges() {
    if (this.jjRole && this.isLoggedIn) this.headingText = "JJ Written Reasons - " + this.jjPage ;
  }

  ngOnInit() {
    this.oidcSecurityService.isAuthenticated$.subscribe(({ isAuthenticated }) => {

      // decode the token to get its payload
      const tokenPayload = this.jwtHelper.decodeToken(this.oidcSecurityService.getAccessToken());
      let resource_access = tokenPayload?.resource_access["tco-staff-portal"];
      if (resource_access) {
        this.isLoggedIn = true;
        let roles = resource_access.roles;
        if (roles) roles.forEach(role => {
          if (role == "vtc-user") { // TODO USE role name for JJ
            this.jjRole = true;
          }
          if (role == "vtc-user") {
            this.vtcRole = true;
          }
        });
      } else this.isLoggedIn = false;

      if (this.jjRole && this.isLoggedIn) this.headingText = "JJ Written Reasons - " + this.jjPage ;
      else if (this.vtcRole && this.isLoggedIn) this.headingText = "Ticket Resolution Management ";
      else if (!this.isLoggedIn) this.headingText = "Please sign in"

      this.fullName = this.oidcSecurityService.getUserData()?.name;
    })

    this.oidcSecurityService.userData$.subscribe( (userInfo: any) => {
      if (userInfo && userInfo.userData && userInfo.userData.name) this.fullName = userInfo.userData.name;
    });

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
    this.oidcSecurityService.authorize();
  }

  logout() {
    this.oidcSecurityService.logoffAndRevokeTokens();
    this.isLoggedIn = false;
  }
}
