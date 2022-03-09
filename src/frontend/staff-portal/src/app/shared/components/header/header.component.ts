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
import { User } from '@shared/models/user.model';
import { AppConfigService } from 'app/services/app-config.service';
import { AuthService } from 'app/services/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
})
export class HeaderComponent implements OnInit {
  public fullName: string;
  @Input() public isMobile: boolean;
  @Input() public hasMobileSidemenu: boolean;
  @Output() public toggle: EventEmitter<void>;

  public languageCode: string;
  public languageDesc: string;

  public environment: string;
  public version: string;

  constructor(
    protected authService: AuthService,
    protected logger: LoggerService,
    private appConfigService: AppConfigService,
    private translateService: TranslateService
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
    const authenticated = await this.authService.isLoggedIn();
    if (authenticated) {
      this.authService.getUser$().subscribe((user: User) => {
        this.fullName = `${user?.firstName} ${user?.lastName}`;
      });
    }
  }

  public toggleSidenav(): void {
    this.toggle.emit();
  }

  public onLogout(): Promise<void> {
    this.authService.logout(
      `${window.location.protocol}//${window.location.host}`
    );
    return Promise.resolve();
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
