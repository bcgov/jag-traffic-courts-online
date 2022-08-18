import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';
import { KeycloakProfile } from 'keycloak-js';
import { AuthService } from 'app/services/auth.service';

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
    private authService: AuthService
  ) {
  }

  public async ngOnInit() {
    this.authService.isLoggedIn$.subscribe(isLoggedIn => {
      this.isLoggedIn = isLoggedIn;
      this.authService.userProfile$.subscribe(userProfile => {
        this.userProfile = userProfile;
        this.fullName = this.userProfile?.firstName + " " + this.userProfile?.lastName;
        this.jjAdminRole = this.authService.checkRole("admin-judicial-justice");
        this.jjIDIR = this.authService.userIDIR;
      })
    })
  }

  changeHeading(heading: any) {
    if (heading == "WR Inbox") this.tabSelected.setValue(1);
    else if (heading == "WR Assignments") this.tabSelected.setValue(0);
    this.jjPage = heading;
  }

}
