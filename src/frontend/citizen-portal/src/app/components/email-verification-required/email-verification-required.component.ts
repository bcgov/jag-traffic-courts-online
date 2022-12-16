import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TicketTypes } from '@shared/enums/ticket-type.enum';
import { DisputeRepresentedByLawyer } from 'app/api';
import { NoticeOfDisputeService, NoticeOfDispute } from 'app/services/notice-of-dispute.service';
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
  ticketTypes = TicketTypes ;
  countsActions: any;
  RepresentedByLawyer = DisputeRepresentedByLawyer;

  noticeOfDispute: NoticeOfDispute;

  constructor(
    private route: ActivatedRoute,
    private noticeOfDisputeService: NoticeOfDisputeService,
    private violationTicketService: ViolationTicketService,
  ) {
    this.route.queryParams.subscribe((params) => {
      this.email = params.email;
      this.token = params.token;
    });
    this.noticeOfDispute = noticeOfDisputeService.noticeOfDispute;
  }

  ngOnInit() {
    this.ticketType = this.violationTicketService.ticketType;
    this.countsActions = this.noticeOfDisputeService.getCountsActions(this.noticeOfDispute.dispute_counts);
  }

  resendEmail() {
    this.noticeOfDisputeService.resendVerificationEmail(this.token).subscribe(() => { });
  }

  onPrint(): void {
    window.print();
  }
}
