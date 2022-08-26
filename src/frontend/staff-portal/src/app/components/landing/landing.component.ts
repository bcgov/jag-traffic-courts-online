import { OnInit, Component, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'app/services/auth.service';
import { KeycloakProfile } from 'keycloak-js';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss', '../../app.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class LandingComponent implements OnInit {
  public isLoggedIn = false;
  public userProfile: KeycloakProfile = {};

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {
  }

  public async ngOnInit() {
    this.authService.isLoggedIn$.subscribe(isLoggedIn => {
      this.isLoggedIn = isLoggedIn;
    })

    this.authService.checkAuth().subscribe(() => {
      this.authService.isLoggedIn$.subscribe(isLoggedIn => {
        this.isLoggedIn = isLoggedIn;
        if (this.isLoggedIn) {
          this.authService.userProfile$.subscribe(userProfile => {
            this.userProfile = userProfile;
            this.router.navigate([this.authService.getRedirectUrl()]);
          })
        } else {
          this.router.navigate(["/"]);
        }
      })
    })
  }

  public async login() {
    await this.authService.login();
  }
}
