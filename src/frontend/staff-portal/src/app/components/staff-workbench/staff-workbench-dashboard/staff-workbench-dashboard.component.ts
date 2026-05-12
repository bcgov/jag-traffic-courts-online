import { Component, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';
import { Dispute } from 'app/services/dispute.service';
import { MatTab } from '@angular/material/tabs';
import { DisputeDecisionInboxComponent } from '../dispute-decision-inbox/dispute-decision-inbox.component';
import { TicketInboxComponent } from '../ticket-inbox/ticket-inbox.component';
import { DisputeService } from 'app/services/dispute.service';
import { UpdateRequestInboxComponent } from '../update-request-inbox/update-request-inbox.component';
import { AuthService } from 'app/services/auth.service';
import { UserGroup } from '@shared/enums/user-group.enum';
import { TabType } from '@shared/enums/tab-type.enum';
import { DisputeCaseFileSummary } from 'app/api';

@Component({
  selector: 'app-staff-workbench-dashboard',
  templateUrl: './staff-workbench-dashboard.component.html',
  styleUrls: ['./staff-workbench-dashboard.component.scss'],
  standalone: false,
})
export class StaffWorkbenchDashboardComponent implements OnInit {
  @ViewChild("DCF") dcfTab: MatTab;

  tabSelected = new FormControl(0);
  showTicket: boolean = false;
  showManualEntry: boolean = false;
  decidePopup = '';
  disputeInfo: Dispute;
  tcoDisputeInfo: DisputeCaseFileSummary;
  tabTypes = TabType;
  tabTypeSelected: TabType;

  hasTicketValidationPermission: boolean = false;
  hasDecisionValidationPermission: boolean = false;
  hasUpdateRequestsPermission: boolean = false;
  hasDCFPermission: boolean = false;

  @ViewChild(DisputeDecisionInboxComponent) disputeChild: DisputeDecisionInboxComponent;
  @ViewChild(TicketInboxComponent) ticketChild: TicketInboxComponent;
  @ViewChild(UpdateRequestInboxComponent) updateRequestChild: UpdateRequestInboxComponent;

  constructor(
    private authService: AuthService,
    private disputeService: DisputeService,
  ) {
  }
  
  ngOnInit(): void {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {        
        this.hasTicketValidationPermission = this.authService.checkRoles([UserGroup.VTC_STAFF, UserGroup.SUPPORT_STAFF]);
        this.hasDecisionValidationPermission = this.authService.checkRoles([UserGroup.VTC_STAFF, UserGroup.SUPPORT_STAFF]);
        this.hasUpdateRequestsPermission = this.authService.checkRoles([UserGroup.VTC_STAFF, UserGroup.SUPPORT_STAFF]);
        this.hasDCFPermission = this.authService.checkRoles([UserGroup.VTC_STAFF, UserGroup.SUPPORT_STAFF]);
      }
    });
  }

  changeDispute(dispute: Dispute, type: TabType) {
    if ((dispute as any)?.action === 'createNew') {
      this.showManualEntry = true;
      this.showTicket = false;
      return;
    }

    this.disputeInfo = dispute;
    if (dispute.createdBy !== 'OCCAMORDS') {
      this.decidePopup = 'A';
    } else {
      if (dispute.ticketNumber[0] === 'A') {
        this.decidePopup = 'E';
      } else {
        this.decidePopup = 'A';
      }
    }
    this.tabTypeSelected = type;
    this.showTicket = true;
  }

  changeTCODispute(tcoDispute: DisputeCaseFileSummary, type: TabType) {
    this.tcoDisputeInfo = tcoDispute;
    this.tabTypeSelected = type;
    this.showTicket = true;
  }

  backInbox() {
    this.showTicket = false;
    this.showManualEntry = false;
    this.disputeService.refreshDisputes.emit();
  }
}
