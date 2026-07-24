import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { AuthService } from 'app/services/auth.service';
import { MatTab } from '@angular/material/tabs';
import { UserGroup } from '@shared/enums/user-group.enum';
import { TabType } from '@shared/enums/tab-type.enum';
import { DisputeStatus } from '@shared/consts/DisputeStatus.model';
import { DisputeCaseFileSummary } from 'app/api';
import { Store } from '@ngrx/store';
import { ReturnedDecisionStore } from 'app/store';
import { LoggerService } from '@core/services/logger.service';
import { ReturnedDecisionSelectors } from 'app/store/returned-decision/returned-decision.selectors';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-jj-workbench-dashboard',
  templateUrl: './jj-workbench-dashboard.component.html',
  styleUrls: ['./jj-workbench-dashboard.component.scss'],
  standalone: false,
})
export class JjWorkbenchDashboardComponent implements OnInit {
  @ViewChild('DCF') dcfTab: MatTab;

  private authService = inject(AuthService);
  private logger = inject(LoggerService);
  private store = inject(Store);

  returnedDecisionCollection = toSignal(
    this.store.select(ReturnedDecisionSelectors.PagedCollection),
  );

  showDispute: boolean = false;
  tabSelected = new FormControl(0);
  jjPage: string = 'WR Assignments';
  tcoDisputeInfo: DisputeCaseFileSummary;
  isInfoEditable: boolean = false;
  tabTypes = TabType;
  tabTypeSelected: TabType;

  hasAssignmentsPermission: boolean = false;
  hasWRInboxPermission: boolean = false;
  hasHearingInboxPermission: boolean = false;
  hasDCFPermission: boolean = false;
  jjIDIR?: string;

  ngOnInit() {
    this.authService.userProfile$.subscribe((userProfile) => {
      if (userProfile) {
        // TCVP-1981 - only show tabs to users with permissions
        this.hasAssignmentsPermission = this.authService.checkRoles([
          UserGroup.ADMIN_JUDICIAL_JUSTICE,
          UserGroup.SUPPORT_STAFF,
        ]);
        this.hasWRInboxPermission = this.authService.checkRoles([
          UserGroup.ADMIN_JUDICIAL_JUSTICE,
          UserGroup.JUDICIAL_JUSTICE,
          UserGroup.SUPPORT_STAFF,
        ]);
        this.hasHearingInboxPermission = this.authService.checkRoles([
          UserGroup.ADMIN_JUDICIAL_JUSTICE,
          UserGroup.JUDICIAL_JUSTICE,
          UserGroup.SUPPORT_STAFF,
        ]);
        this.hasDCFPermission = this.authService.checkRoles([
          UserGroup.ADMIN_JUDICIAL_JUSTICE,
          UserGroup.JUDICIAL_JUSTICE,
          UserGroup.SUPPORT_STAFF,
        ]);

        this.jjIDIR = userProfile.idir;
        this.getReturnedDecisions();
      }
    });
  }

  changeTab(index: number, tab: MatTab) {
    this.logger.info(
      `JjWorkbenchDashboardComponent::changeTab: ${tab.textLabel}`,
    );
    this.tabSelected.setValue(index);
    if (tab.textLabel === 'Returned Decisions') {
      this.getReturnedDecisions();
    }
  }

  changeTCODispute(tcoDispute: DisputeCaseFileSummary, type: TabType) {
    this.isInfoEditable =
      !this.dcfTab.isActive &&
      [
        DisputeStatus.New,
        DisputeStatus.Review,
        DisputeStatus.InProgress,
        DisputeStatus.HearingScheduled,
      ].includes(tcoDispute.disputeStatus.code as DisputeStatus);
    this.tcoDisputeInfo = tcoDispute;
    this.tabTypeSelected = type;
    this.showDispute = true;
  }

  backInbox() {
    this.showDispute = false;
  }

  getReturnedDecisions() {
    if (this.jjIDIR) {
      this.logger.info(
        'JjWorkbenchDashboardComponent::getReturnedDecisions',
      );
      this.store.dispatch(
        ReturnedDecisionStore.Actions.Get({
          assignedTo: this.authService.checkRole(UserGroup.SUPPORT_STAFF) ? undefined : this.jjIDIR,
        }),
      );
    }
  }
}
