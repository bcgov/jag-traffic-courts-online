import { Component, OnInit, ViewChild } from '@angular/core';
import { filter, Subscription, Observable } from 'rxjs';
import { FormControl } from '@angular/forms';
import { Dispute } from 'app/services/dispute.service';
import { MatLegacyTab as MatTab } from '@angular/material/legacy-tabs';
import { JJDispute } from 'app/services/jj-dispute.service';
import { DisputeDecisionInboxComponent } from '../dispute-decision-inbox/dispute-decision-inbox.component';
import { TicketInboxComponent } from '../ticket-inbox/ticket-inbox.component';
import { DisputeService } from 'app/services/dispute.service';
import { UpdateRequestInboxComponent } from '../update-request-inbox/update-request-inbox.component';
import { Store } from '@ngrx/store';
import { JJDisputeStore } from 'app/store';
import { AuthService } from 'app/services/auth.service';
import { UserGroup } from '@shared/enums/user-group.enum';

@Component({
  selector: 'app-staff-workbench-dashboard',
  templateUrl: './staff-workbench-dashboard.component.html',
  styleUrls: ['./staff-workbench-dashboard.component.scss'],
})
export class StaffWorkbenchDashboardComponent implements OnInit {
  @ViewChild("DCF") dcfTab: MatTab;

  busy: Subscription;
  tabSelected = new FormControl(0);
  showTicket: boolean = false;
  decidePopup = '';
  disputeInfo: Dispute;
  data$: Observable<JJDispute[]>;
  jjDisputeInfo: JJDispute;

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
    private store: Store
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
    this.data$ = this.store.select(JJDisputeStore.Selectors.JJDisputes).pipe(filter(i => !!i));
  }

  changeDispute(dispute: Dispute) {
    this.disputeInfo = dispute;
    if (dispute.ticketNumber[0] == 'A') {
      this.decidePopup = 'E'
    } else {
      this.decidePopup = "A"
    }
    this.showTicket = true;
  }

  changeJJDispute(jjDispute: JJDispute) {
    this.jjDisputeInfo = jjDispute;
    this.showTicket = true;
  }

  backInbox() {
    this.showTicket = false;
    this.disputeService.refreshDisputes.emit();
  }
}
