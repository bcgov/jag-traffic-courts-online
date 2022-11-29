import { Component, OnInit, ViewChild} from '@angular/core';
import { Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';
import { JJDisputeService, JJDispute } from 'app/services/jj-dispute.service';
import { MatTab } from '@angular/material/tabs';

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
  public userProfile: KeycloakProfile = {};
  public jjDisputeInfo: JJDispute;
  @ViewChild("DCF") private dcfTab: MatTab;
  public isInfoEditable;

  constructor(
    private authService: AuthService,
    private jjDisputeService: JJDisputeService
  ) {
  }

  public async ngOnInit() {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.userProfile = userProfile;
        this.jjAdminRole = this.authService.checkRole("admin-judicial-justice");
      }
    })
  }

  changeJJDispute(jjDispute: JJDispute) {
    this.isInfoEditable = !this.dcfTab.isActive;
    this.jjDisputeInfo = jjDispute;
    this.showDispute = true;
  }

  backInbox() {
    this.showDispute = false;
    this.jjDisputeService.refreshDisputes.emit();
  }
}
