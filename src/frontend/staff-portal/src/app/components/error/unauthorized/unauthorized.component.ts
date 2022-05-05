import { Component, OnInit } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { LogInOutService } from 'app/services/log-in-out.service';

@Component({
  selector: 'app-unauthorized',
  templateUrl: './unauthorized.component.html',
  styleUrls: ['./unauthorized.component.scss']
})
export class UnauthorizedComponent implements OnInit {

  constructor(public logInOutService: LogInOutService, public oidcSecurityService: OidcSecurityService) { }

  ngOnInit(): void {
    this.logInOutService.getLogoutStatus.subscribe((data) => {
      if (data !== null || data !== '')
      {
        if(data === 'IDIR Sign in'){
          this.login();
        }
        else
          if(data === 'Sign out'){
            this.logout();
          }
      }
    })
  }

  login() {
    this.oidcSecurityService.authorize();
  }

  logout() {
    this.oidcSecurityService.logoffAndRevokeTokens();
  }

}
