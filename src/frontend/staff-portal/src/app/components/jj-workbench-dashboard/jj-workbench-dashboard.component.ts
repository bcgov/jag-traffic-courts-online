import { Component  } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { JwtHelperService } from '@auth0/angular-jwt';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { JJDispute } from '../../api/model/jJDispute.model';
import { Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-jj-workbench-dashboard',
  templateUrl: './jj-workbench-dashboard.component.html',
  styleUrls: ['./jj-workbench-dashboard.component.scss'],
})
export class JjWorkbenchDashboardComponent {
  busy: Subscription;
  jjDisputeInfo: JJDispute;
  tabSelected = new FormControl(0);
  public isLoggedIn = false;
  public jjAdminRole: boolean = false;
  jjPage: string = "Assignments";
  public fullName: string = "Loading...";
  public jjIDIR: string = "";

  constructor(
    public jjDisputeService: JJDisputeService,
    private oidcSecurityService: OidcSecurityService,
    public jwtHelper: JwtHelperService

  ) {

    // check for JJ Admin role
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

              if (role === "vtc-user") { // TODO USE role name for jj Admin
                this.jjAdminRole = true;
              }
            });
          }
        }

        this.fullName = this.oidcSecurityService.getUserData()?.name;
        this.jjIDIR = this.oidcSecurityService.getUserData()?.preferred_username;
      }
    });
  }

  changeHeading(heading: any) {
    if (heading == "Inbox") this.tabSelected.setValue(1);
    else if (heading == "Assignments") this.tabSelected.setValue(0);
    this.jjPage = heading;
  }

}
