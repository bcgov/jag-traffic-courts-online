import { AfterViewInit, Component, ViewEncapsulation, Inject } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { LogInOutService } from 'app/services/log-in-out.service';
import { Router } from '@angular/router';
// import { SnowplowService } from '@core/services/snowplow.service';
// import { AppConfigService } from 'app/services/app-config.service';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class LandingComponent implements AfterViewInit {
  public isLoggedIn = false;

  // Urls for the various links
  public understandYourTicketLink: string;
  public paymentOptionsLink: string;
  public resolutionOptionsLink: string;
  public roadSafetyBCVisitUsLink: string;
  public icbcVisitUsLink: string;
  public provincialCourtOfBCVisitUsLink: string;
  public courthouseServicesOfBCVisitUsLink: string;

  constructor(
    public oidcSecurityService : OidcSecurityService,
    private logInOutService : LogInOutService,
    @Inject(Router) private router,

    // private appConfigService: AppConfigService,
    // private snowplow: SnowplowService
  ) {
    // this.understandYourTicketLink =
    //   this.appConfigService.understandYourTicketLink;
    // this.paymentOptionsLink = this.appConfigService.paymentOptionsLink;
    // this.resolutionOptionsLink = this.appConfigService.resolutionOptionsLink;
    // this.roadSafetyBCVisitUsLink =
    //   this.appConfigService.roadSafetyBCVisitUsLink;
    // this.icbcVisitUsLink = this.appConfigService.icbcVisitUsLink;
    // this.provincialCourtOfBCVisitUsLink =
    //   this.appConfigService.provincialCourtOfBCVisitUsLink;
    // this.courthouseServicesOfBCVisitUsLink =
    //   this.appConfigService.courthouseServicesOfBCVisitUsLink;
  }

  public async ngOnInit() {
    // this.logInOutService.getLogoutStatus.subscribe((data) => {
    //   console.log(data);
    //   if (data !== null || data !== '') {
    //     if (data == 'BCeID Login') {
    //       this.login();
    //     } else {
    //       this.logout();
    //     }
    //   }
    // });

    this.oidcSecurityService.checkAuth().subscribe(
      ({ isAuthenticated}) => {
        // if (isAuthenticated === true)
        // {
          this.router.navigate(['/ticket']);
        // }
        // else
        // {
          // this.router.navigate(['/']);
        // }

        this.logInOutService.currentUser(isAuthenticated);
    });
  }

  // login() {
  //   this.oidcSecurityService.authorize();
  // }

  // logout() {
  //   this.oidcSecurityService.logoffAndRevokeTokens();
  // }

  public ngAfterViewInit(): void {
    // refresh link urls now that we set the links
    // this.snowplow.refreshLinkClickTracking();
  }
}
