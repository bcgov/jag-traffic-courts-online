import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';

@Component({
  selector: 'app-jj-workbench-dashboard',
  templateUrl: './jj-workbench-dashboard.component.html',
  styleUrls: ['./jj-workbench-dashboard.component.scss'],
})
export class JjWorkbenchDashboardComponent implements OnInit {
  busy: Subscription;
  tabSelected = new FormControl(0);
  public isLoggedIn = false;
  public jjAdminRole: boolean = false;
  jjPage: string = "WR Assignments";
  public fullName: string = "Loading...";
  public jjIDIR: string = "";
  public userProfile: KeycloakProfile = {};

  constructor(
    public keycloak: KeycloakService
  ) {

  }

  public async ngOnInit() {

    this.isLoggedIn = await this.keycloak.isLoggedIn();

    if (this.isLoggedIn) {
      this.userProfile = await this.keycloak.loadUserProfile();
      this.fullName = this.userProfile.firstName + " " + this.userProfile.lastName;
      this.jjAdminRole = this.keycloak.isUserInRole("admin-judicial-justice", "staff-api");
      this.jjIDIR = this.keycloak.getUsername();
    }


  }
  changeHeading(heading: any) {
    if (heading == "WR Inbox") this.tabSelected.setValue(1);
    else if (heading == "WR Assignments") this.tabSelected.setValue(0);
    this.jjPage = heading;
  }

}
