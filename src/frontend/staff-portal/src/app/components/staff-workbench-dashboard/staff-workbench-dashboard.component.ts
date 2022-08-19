import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';
import { KeycloakProfile } from 'keycloak-js';
import { AuthService } from 'app/services/auth.service';

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
    private authService: AuthService
  ) {

  }

  public async ngOnInit() {
    this.authService.isLoggedIn$.subscribe(isLoggedIn => {
      this.isLoggedIn = isLoggedIn;
      this.authService.userProfile$.subscribe(userProfile => {
        this.userProfile = userProfile;
        this.fullName = this.userProfile?.firstName + " " + this.userProfile?.lastName;
        this.staffIDIR = this.authService.userIDIR;
      })
    })
  }

  changeHeading(heading: any) {
    if (heading == "Ticket Validation") this.tabSelected.setValue(1);
    else if (heading == "Decision Validation") this.tabSelected.setValue(0);
    this.staffPage = heading;
  }
}
