import { Component, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';
import { Dispute } from 'app/services/dispute.service';
import { JJDispute } from 'app/services/jj-dispute.service';
import { DisputeDecisionInboxComponent } from '../dispute-decision-inbox/dispute-decision-inbox.component';
import { TicketInboxComponent } from '../ticket-inbox/ticket-inbox.component';
import { DisputeService } from 'app/services/dispute.service';

@Component({
  selector: 'app-staff-workbench-dashboard',
  templateUrl: './staff-workbench-dashboard.component.html',
  styleUrls: ['./staff-workbench-dashboard.component.scss'],
})
export class StaffWorkbenchDashboardComponent implements OnInit {
  busy: Subscription;
  public tabSelected = new FormControl(0);
  public showTicket: boolean = false;
  public decidePopup = '';
  public disputeInfo: Dispute;
  public jjDisputeInfo: JJDispute;

  @ViewChild(DisputeDecisionInboxComponent) public disputeChild: DisputeDecisionInboxComponent;
  @ViewChild(TicketInboxComponent) public ticketChild: TicketInboxComponent;

  constructor(
    private disputeService: DisputeService
  ) {

  }

  public async ngOnInit() {
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
