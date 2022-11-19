import { Component, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';
import { DisputeExtended } from 'app/services/dispute.service';
import { JJDisputeView } from 'app/services/jj-dispute.service';
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
  public fullName: string = "Loading...";
  public staffIDIR: string = "";
  public userProfile: KeycloakProfile = {};
  public showTicket: boolean = false;
  public decidePopup = '';
  public disputeInfo: DisputeExtended;
  public jjDisputeInfo: JJDisputeView;

  @ViewChild(DisputeDecisionInboxComponent) public disputeChild: DisputeDecisionInboxComponent;
  @ViewChild(TicketInboxComponent) public ticketChild: TicketInboxComponent;

  constructor(
    private authService: AuthService,
    private disputeService: DisputeService
  ) {

  }

  public async ngOnInit() {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.userProfile = userProfile;
        this.fullName = this.userProfile?.firstName + " " + this.userProfile?.lastName;
        this.staffIDIR = this.authService.userIDIR;
      }
    })
  }

  changeDispute(dispute: DisputeExtended) {
    this.disputeInfo = dispute;
    if (dispute.ticketNumber[0] == 'A') {
      this.decidePopup = 'E'
    } else {
      this.decidePopup = "A"
    }
    this.showTicket = true;
  }

  changeJJDispute(jjDispute: JJDisputeView) {
    this.jjDisputeInfo = jjDispute;
    this.showTicket = true;
  }

  backInbox() {
    this.showTicket = false;
    this.disputeService.refreshDisputes.emit();
  }
}
