import { Component, OnInit} from '@angular/core';
import { Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';
import { JJDisputeService, JJDisputeView } from 'app/services/jj-dispute.service';

@Component({
  selector: 'app-jj-workbench-dashboard',
  templateUrl: './jj-workbench-dashboard.component.html',
  styleUrls: ['./jj-workbench-dashboard.component.scss'],
})
export class JjWorkbenchDashboardComponent implements OnInit {
  busy: Subscription;
  showDispute: boolean = false;
  tabSelected = new FormControl(0);
  public jjAdminRole: boolean = false;
  jjPage: string = "WR Assignments";
  public fullName: string = "Loading...";
  public jjIDIR: string = "";
  public userProfile: KeycloakProfile = {};
  public jjDisputeInfo: JJDisputeView;


  constructor(
    private authService: AuthService,
    private jjDisputeService: JJDisputeService
  ) {
  }

  public async ngOnInit() {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.userProfile = userProfile;
        this.fullName = this.authService.userFullName;
        this.jjAdminRole = this.authService.checkRole("admin-judicial-justice");
        this.jjIDIR = this.authService.userIDIRLogin;
      }
    })
  }

  changeJJDispute(jjDispute: JJDisputeView) {
    this.jjDisputeInfo = jjDispute;
    this.showDispute = true;
  }

  backInbox() {
    this.showDispute = false;
    this.jjDisputeService.refreshDisputes.emit();
  }
}
