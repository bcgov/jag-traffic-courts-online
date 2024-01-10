import { Component, OnInit, ViewChild } from '@angular/core';
import { filter, Observable, Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';
import { AuthService } from 'app/services/auth.service';
import { JJDisputeService, JJDispute } from 'app/services/jj-dispute.service';
import { MatLegacyTab as MatTab } from '@angular/material/legacy-tabs';
import { AppState } from 'app/store';
import { Store } from '@ngrx/store';
import * as JJDisputeStore from "app/store/jj-dispute";
import { BusyService } from '@core/services/busy.service';

@Component({
  selector: 'app-jj-workbench-dashboard',
  templateUrl: './jj-workbench-dashboard.component.html',
  styleUrls: ['./jj-workbench-dashboard.component.scss'],
})
export class JjWorkbenchDashboardComponent implements OnInit {
  @ViewChild("DCF") dcfTab: MatTab;
  busy: Subscription;

  data$: Observable<JJDispute[]>;
  showDispute: boolean = false;
  tabSelected = new FormControl(0);
  jjPage: string = "WR Assignments";
  jjDisputeInfo: JJDispute;
  isInfoEditable: boolean = false;

  hasAssignmentsPermission: boolean = false;
  hasWRInboxPermission: boolean = false;
  hasHearingInboxPermission: boolean = false;
  hasDCFPermission: boolean = false;

  constructor(
    private authService: AuthService,
    private busyService: BusyService,
    private jjDisputeService: JJDisputeService,
    private store: Store<AppState>
  ) {
  }

  ngOnInit() {
    this.busyService.busy$.subscribe(i => this.busy = i);
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        
        // TCVP-1981 - only show tabs to users with permissions
        this.hasAssignmentsPermission = this.authService.checkRoles(["admin-judicial-justice", "support-staff"]);
        this.hasWRInboxPermission = this.authService.checkRoles(["admin-judicial-justice", "judicial-justice", "support-staff"]);
        this.hasHearingInboxPermission = this.authService.checkRoles(["admin-judicial-justice", "judicial-justice", "support-staff"]);
        this.hasDCFPermission = this.authService.checkRoles(["admin-judicial-justice", "judicial-justice", "support-staff"]);
      }
    })
    this.data$ = this.store.select(state => state.jjDispute.data).pipe(filter(i => !!i));
  }

  changeJJDispute(jjDispute: JJDispute) {
    this.isInfoEditable = !this.dcfTab.isActive && this.jjDisputeService.jjDisputeStatusEditable.indexOf(jjDispute.status) > -1;
    this.jjDisputeInfo = jjDispute;
    this.showDispute = true;
  }

  backInbox() {
    this.showDispute = false;
    this.store.dispatch(JJDisputeStore.Actions.Get());
  }
}
