import { Component, OnInit, ViewChild } from '@angular/core';
import { filter, Subscription, Observable } from 'rxjs';
import { FormControl } from '@angular/forms';
import { Dispute } from 'app/services/dispute.service';
import { MatTab } from '@angular/material/tabs';
import { JJDispute } from 'app/services/jj-dispute.service';
import { DisputeDecisionInboxComponent } from '../dispute-decision-inbox/dispute-decision-inbox.component';
import { TicketInboxComponent } from '../ticket-inbox/ticket-inbox.component';
import { DisputeService } from 'app/services/dispute.service';
import { UpdateRequestInboxComponent } from '../update-request-inbox/update-request-inbox.component';
import { select, Store } from '@ngrx/store';
import { AppState } from 'app/store';


@Component({
  selector: 'app-staff-workbench-dashboard',
  templateUrl: './staff-workbench-dashboard.component.html',
  styleUrls: ['./staff-workbench-dashboard.component.scss'],
})
export class StaffWorkbenchDashboardComponent implements OnInit {
  @ViewChild("DCF") private dcfTab: MatTab;
  busy: Subscription;
  public tabSelected = new FormControl(0);
  public showTicket: boolean = false;
  public decidePopup = '';
  public disputeInfo: Dispute;
  data$: Observable<JJDispute[]>;
  public jjDisputeInfo: JJDispute;

  @ViewChild(DisputeDecisionInboxComponent) public disputeChild: DisputeDecisionInboxComponent;
  @ViewChild(TicketInboxComponent) public ticketChild: TicketInboxComponent;
  @ViewChild(UpdateRequestInboxComponent) public updateRequestChild: UpdateRequestInboxComponent;

  constructor(
    private disputeService: DisputeService,
    private store: Store<AppState>
  ) {

  }

  ngOnInit() {
    this.data$ = this.store.pipe(select(state => state.jjDispute.data), filter(i => !!i));
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
