import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ticketTypes } from '@shared/enums/ticket-type.enum';
import { DisputeRepresentedByLawyer } from 'app/api';
import { DisputeService, NoticeOfDispute } from 'app/services/dispute.service';
import { ViolationTicketService } from 'app/services/violation-ticket.service';

@Component({
  selector: 'app-email-verification-required',
  templateUrl: './email-verification-required.component.html',
  styleUrls: ['./email-verification-required.component.scss'],
})
export class EmailVerificationRequiredComponent implements OnInit {
  private token: string;
  
  email: string;
  ticketType;
  ticketTypes = ticketTypes;
  countsActions: any;
  RepresentedByLawyer = DisputeRepresentedByLawyer;

  noticeOfDispute: NoticeOfDispute;

  constructor(
    private route: ActivatedRoute,
    private disputeService: DisputeService,
    private violationTicketService: ViolationTicketService,
  ) {
    this.route.queryParams.subscribe((params) => {
      this.email = params.email;
      this.token = params.token;
    });
    this.noticeOfDispute = disputeService.noticeOfDispute;
  }

  ngOnInit() {
    this.ticketType = this.violationTicketService.ticketType;
    this.countsActions = this.disputeService.getCountsActions(this.noticeOfDispute.dispute_counts);
  }

  resendEmail() {
    this.disputeService.resendVerificationEmail(this.token).subscribe(() => { });
  }

  onPrint(): void {
    window.print();
  }
}
