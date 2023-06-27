import { Component, ChangeDetectionStrategy, Output, EventEmitter, Input, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { AppConfigService } from 'app/services/app-config.service';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';
import { LookupsService } from 'app/services/lookups.service';
import { AppState, JJDisputeStore } from 'app/store';
import { KeycloakService } from 'keycloak-angular';
import { filter, forkJoin } from 'rxjs';

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
  isInit = true;
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
    private lookupsService: LookupsService,
    private logger: LoggerService,
    private store: Store<AppState>,
    private keycloak: KeycloakService,
  ) {
    this.toggle = new EventEmitter<void>();

    this.languageCode = this.translateService.getDefaultLang();
    this.onLanguage();

    this.environment = this.appConfigService.environment;
    this.version = this.appConfigService.version;
  }

  ngOnInit() {
    this.headingText = this.activatedRoute.snapshot?.data?.title ? this.activatedRoute.snapshot?.data?.title : "Authenticating...";
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.headingText = this.activatedRoute.snapshot?.data?.title ? this.activatedRoute.snapshot?.data?.title : "Authenticating...";
      });

    this.authService.checkAuth().subscribe(() => {
      this.authService.isLoggedIn$.subscribe(isLoggedIn => {
        this.isLoggedIn = isLoggedIn;
        this.fullName = this.authService.userProfile?.fullName;

        if (this.isLoggedIn) {
          this.authService.userProfile$.subscribe(() => {
            if (this.isInit) {
              this.isInit = false;
              let observables = [
                this.authService.loadUsersLists(),
                this.lookupsService.init()
              ];

              forkJoin(observables).subscribe({
                next: results => {
                  this.store.dispatch(JJDisputeStore.Actions.Get());
                },
                error: err => {
                  this.logger.error("Landing Page Init: Initial data loading failed");
                }
              });
            }
          })
        }
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
