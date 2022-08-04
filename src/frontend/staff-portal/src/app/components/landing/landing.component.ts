import { OnInit, Component, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { AppRoutes } from 'app/app.routes';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';

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
  public userProfile: KeycloakProfile = {};

  constructor(
    private router: Router,
    public keycloak: KeycloakService,
  ) {
  }

  public async ngOnInit() {

    this.isLoggedIn = await this.keycloak.isLoggedIn();

    if (this.isLoggedIn) {
      this.userProfile = await this.keycloak.loadUserProfile();

      this.jjRole = this.keycloak.isUserInRole("judicial-justice", "tco-staff-portal");
      this.vtcRole = this.keycloak.isUserInRole("vtc-staff", "tco-staff-portal");

      if (this.jjRole) this.router.navigate([AppRoutes.JJWORKBENCH]);
      else if (this.vtcRole) this.router.navigate([AppRoutes.TICKET]);
      if (!this.jjRole && !this.vtcRole) this.router.navigate([AppRoutes.UNAUTHORIZED]);
  }
  }

  public async login() {
    await this.keycloak.login({
      redirectUri: window.location.toString()
    });
  }
}
