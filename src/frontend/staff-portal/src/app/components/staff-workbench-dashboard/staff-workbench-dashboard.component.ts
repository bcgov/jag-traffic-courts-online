import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';

@Component({
  selector: 'app-staff-workbench-dashboard',
  templateUrl: './staff-workbench-dashboard.component.html',
  styleUrls: ['./staff-workbench-dashboard.component.scss'],
})
export class StaffWorkbenchDashboardComponent implements OnInit {
  busy: Subscription;
  tabSelected = new FormControl(0);
  public isLoggedIn = false;
  public fullName: string = "Loading...";
  public staffIDIR: string = "";
  public userProfile: KeycloakProfile = {};
  public staffPage: string = "";

  constructor(
    public keycloak: KeycloakService
  ) {

  }

  public async ngOnInit() {

    this.isLoggedIn = await this.keycloak.isLoggedIn();

    if (this.isLoggedIn) {
      this.userProfile = await this.keycloak.loadUserProfile();
      this.fullName = this.userProfile.firstName + " " + this.userProfile.lastName;
      this.staffIDIR = this.keycloak.getUsername();
    }

  }

  changeHeading(heading: any) {
    if (heading == "Ticket Validation") this.tabSelected.setValue(1);
    else if (heading == "Decision Validation") this.tabSelected.setValue(0);
    this.staffPage = heading;
  }

}
