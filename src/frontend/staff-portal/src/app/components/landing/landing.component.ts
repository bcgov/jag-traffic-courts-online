import { OnInit, Component, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { AppRoutes } from 'app/app.routes';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss', '../../app.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class LandingComponent implements OnInit {
  public isLoggedIn = false;

  constructor(
    private oidcSecurityService: OidcSecurityService,
    private router: Router,
  ) {
  }

  ngOnInit() {
    this.oidcSecurityService.checkAuth().subscribe(({ isAuthenticated }) => {
      if (isAuthenticated) {
        this.isLoggedIn = isAuthenticated;
        this.router.navigate([AppRoutes.TICKET]);
      }
    })
  }

  login() {
    this.oidcSecurityService.authorize();
  }
}
