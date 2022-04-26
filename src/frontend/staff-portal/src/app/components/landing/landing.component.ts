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

    this.oidcSecurityService.checkAuth().subscribe(
      ({ isAuthenticated}) => {
        this.router.navigate(['/ticket']);
        this.logInOutService.currentUser(isAuthenticated);
    });
  }
}
