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
import { AppConfigService } from 'app/services/app-config.service';
import { LogInOutService } from 'app/services/log-in-out.service';

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
  public btnLabel: string ='IDIR Sign in';
  public btnIcon: string = 'login'; 

  public environment: string;
  public version: string;

  constructor(
    protected logger: LoggerService,
    private appConfigService: AppConfigService,
    private translateService: TranslateService,
    public logInOutService: LogInOutService
  ) {
    this.hasMobileSidemenu = false;
    this.toggle = new EventEmitter<void>();

    this.languageCode = this.translateService.getDefaultLang();
    this.onLanguage();

    this.environment = this.appConfigService.environment;
    this.version = this.appConfigService.version;
  }

  public async ngOnInit() {
    this.logInOutService.getCurrentStatus.subscribe((data) => {
      if (data !== null || data !== undefined)
      {
        if(data === true){

          this.btnLabel = 'Sign out';
          this.btnIcon = 'logout';
        }
        else
        {
          this.btnLabel = 'IDIR Sign in';
          this.btnIcon = 'login';
        }
      }
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

  public onClickBtn()
  {
    this.logInOutService.logoutUser(this.btnLabel);
    if (this.btnLabel === 'IDIR Sign in')
    {
      this.btnLabel = 'Sign out';
      this.btnIcon = 'logout';
    }
    else
    {
      this.btnLabel = 'IDIR Sign in';
      this.btnIcon = 'login';
    }
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
