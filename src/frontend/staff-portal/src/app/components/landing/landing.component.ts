import { OnInit, Component, ViewEncapsulation, Inject } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { LogInOutService } from 'app/services/log-in-out.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class LandingComponent implements OnInit {
  public isLoggedIn = false;

  constructor(
    public oidcSecurityService : OidcSecurityService,
    private logInOutService : LogInOutService,
    @Inject(Router) private router,

  ) {   }

  public async ngOnInit() {

      this.logInOutService.getLogoutStatus.subscribe((data) => {
        if (data !== null || data !== '')
        {
          if(data === 'IDIR Login'){
            this.login();
          }
          else
            if(data === 'Logout'){
              this.logout();
            }
        }
      })

      this.oidcSecurityService.checkAuth().subscribe(
        ({ isAuthenticated}) => {
          console.log("landing page", isAuthenticated);
          if (isAuthenticated === true)
          {
            this.router.navigate(['/ticket']);
          }
          else
          {
            this.login();
          }

          this.logInOutService.currentUser(isAuthenticated);
      });

  }

  login() {
    this.oidcSecurityService.authorize();
  }

  logout() {
    this.oidcSecurityService.logoffAndRevokeTokens();
  }

}
