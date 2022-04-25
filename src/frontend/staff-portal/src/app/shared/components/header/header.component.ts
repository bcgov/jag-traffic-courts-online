import {
  Component,
  ChangeDetectionStrategy,
  Output,
  EventEmitter,
  Input,
  OnInit,
} from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { AppConfigService } from 'app/services/app-config.service';
import { LogInOutService } from 'app/services/log-in-out.service';
import { OidcSecurityService } from 'angular-auth-oidc-client';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
})
export class HeaderComponent implements OnInit {
  public fullName: string;
  public isLoggedIn = false;
  @Input() public isMobile: boolean;
  @Input() public hasMobileSidemenu: boolean;
  @Output() public toggle: EventEmitter<void>;

  public languageCode: string;
  public languageDesc: string;

  public environment: string;
  public version: string;

  constructor(
    public oidcSecurityService : OidcSecurityService,
    private logInOutService : LogInOutService,
    protected logger: LoggerService,
    private appConfigService: AppConfigService,
    private translateService: TranslateService,
    private router: Router
  ) {
    this.hasMobileSidemenu = false;
    this.toggle = new EventEmitter<void>();

    this.languageCode = this.translateService.getDefaultLang();
    this.onLanguage();

    this.environment = this.appConfigService.environment;
    this.version = this.appConfigService.version;
  }

  // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
  public async ngOnInit() {
    this.logInOutService.getLogoutStatus.subscribe((data) => {
      if (data !== null || data !== '')
      {
        if(data === 'BCeID Login'){
          this.login();
        }
        else
          if(data === 'Logout'){
            this.logout();
          }
      }
    })

    this.oidcSecurityService.checkAuth().subscribe(
      ({ isAuthenticated, userData }) => {
        console.log(isAuthenticated, userData);
        this.fullName = userData.firstName + ' ' + userData.lastName;
        if (isAuthenticated === true)
        {
          this.router.navigate(['/ticketpage']);
        }
        else
        {
          this.router.navigate(['/']);
        }

        this.logInOutService.currentUser(isAuthenticated);
    })
  }

  public login() {
    this.oidcSecurityService.authorize();
  }

  public logout() {
    this.oidcSecurityService.logoffAndRevokeTokens();
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
}
