import { Component, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';
import { AuthService } from 'app/services/auth.service';
import { MatTab } from '@angular/material/tabs';
import { BusyService } from '@core/services/busy.service';
import { UserGroup } from '@shared/enums/user-group.enum';
import { TabType } from '@shared/enums/tab-type.enum';
import { DisputeStatus } from '@shared/consts/DisputeStatus.model';
import { DisputeCaseFileSummary } from 'app/api';

@Component({
  selector: 'app-jj-workbench-dashboard',
  templateUrl: './jj-workbench-dashboard.component.html',
  styleUrls: ['./jj-workbench-dashboard.component.scss'],
  standalone: false,
})
export class JjWorkbenchDashboardComponent implements OnInit {
  @ViewChild("DCF") dcfTab: MatTab;
  busy: Subscription;
  showDispute: boolean = false;
  tabSelected = new FormControl(0);
  jjPage: string = "WR Assignments";
  tcoDisputeInfo: DisputeCaseFileSummary;
  isInfoEditable: boolean = false;
  tabTypes = TabType;
  tabTypeSelected: TabType;

  hasAssignmentsPermission: boolean = false;
  hasWRInboxPermission: boolean = false;
  hasHearingInboxPermission: boolean = false;
  hasDCFPermission: boolean = false;

  constructor(
    private authService: AuthService,
    private busyService: BusyService
  ) {
  }

  ngOnInit() {
    this.busyService.busy$.subscribe(i => this.busy = i);
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        
        // TCVP-1981 - only show tabs to users with permissions
        this.hasAssignmentsPermission = this.authService.checkRoles([UserGroup.ADMIN_JUDICIAL_JUSTICE, UserGroup.SUPPORT_STAFF]);
        this.hasWRInboxPermission = this.authService.checkRoles([UserGroup.ADMIN_JUDICIAL_JUSTICE, UserGroup.JUDICIAL_JUSTICE, UserGroup.SUPPORT_STAFF]);
        this.hasHearingInboxPermission = this.authService.checkRoles([UserGroup.ADMIN_JUDICIAL_JUSTICE, UserGroup.JUDICIAL_JUSTICE, UserGroup.SUPPORT_STAFF]);
        this.hasDCFPermission = this.authService.checkRoles([UserGroup.ADMIN_JUDICIAL_JUSTICE, UserGroup.JUDICIAL_JUSTICE, UserGroup.SUPPORT_STAFF]);
      }
    })
  }

  changeTCODispute(tcoDispute: DisputeCaseFileSummary, type: TabType) {
    this.isInfoEditable = !this.dcfTab.isActive && [DisputeStatus.New, DisputeStatus.Review, DisputeStatus.InProgress, 
      DisputeStatus.HearingScheduled].includes(tcoDispute.disputeStatus.code as DisputeStatus);
    this.tcoDisputeInfo = tcoDispute;
    this.tabTypeSelected = type;
    this.showDispute = true;
  }

  backInbox() {
    this.showDispute = false;
  }
}
