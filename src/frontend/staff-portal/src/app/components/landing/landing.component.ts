import { OnInit, Component, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { AppRoutes } from 'app/app.routes';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss', '../../app.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class LandingComponent implements OnInit {
  public isLoggedIn = false;
  public jjRole: boolean = false;
  public vtcRole: boolean = false;

  constructor(
    private oidcSecurityService: OidcSecurityService,
    private router: Router,
    public jwtHelper: JwtHelperService
  ) {
  }

  ngOnInit() {
    this.oidcSecurityService.checkAuth().subscribe(({ isAuthenticated }) => {
      if (isAuthenticated) {
        this.isLoggedIn = isAuthenticated;

        // decode the token to get its payload
        const tokenPayload = this.jwtHelper.decodeToken(this.oidcSecurityService.getAccessToken());
        if (tokenPayload) { 
          let resource_access = tokenPayload.resource_access["tco-staff-portal"];
          if (resource_access) {
            let roles = resource_access.roles;
            if (roles) roles.forEach(role => {
              if (role == "vtc-user") { // TODO USE role name for JJ
                this.jjRole = true;
              } 
              if (role == "vtc-user") {
                this.vtcRole = true;
              }
            });
          }
        }

        // navigate to Ticket Resolution Management or JJ Workbench or Unauthorized based on role
        if (this.jjRole) this.router.navigate([AppRoutes.JJWORKBENCH]);
        else if (this.vtcRole) this.router.navigate([AppRoutes.TICKET]);
        if (!this.jjRole && !this.vtcRole) this.router.navigate([AppRoutes.UNAUTHORIZED]);
      }
    })
  }

  login() {
    this.oidcSecurityService.authorize();
  }
}
