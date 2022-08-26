import { OnInit, Component, ViewEncapsulation } from '@angular/core';
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
  ) {
  }

  public async ngOnInit() {
    this.authService.isLoggedIn$.subscribe(isLoggedIn => {
      this.isLoggedIn = isLoggedIn;
    })
  }

  public async login() {
    await this.authService.login();
  }
}
