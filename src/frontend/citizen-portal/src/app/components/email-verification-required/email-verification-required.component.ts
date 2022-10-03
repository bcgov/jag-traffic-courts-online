import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ticketTypes } from '@shared/enums/ticket-type.enum';
import { DisputeRepresentedByLawyer, NoticeOfDispute } from 'app/api';
import { NoticeOfDisputeService } from 'app/services/notice-of-dispute.service';
import { ViolationTicketService } from 'app/services/violation-ticket.service';

@Component({
  selector: 'app-email-verification-required',
  templateUrl: './email-verification-required.component.html',
  styleUrls: ['./email-verification-required.component.scss'],
})
export class EmailVerificationRequiredComponent implements OnInit {
  public email: string;
  private token: string;
  public ticketType;
  public ticketTypes = ticketTypes;
  public countsActions: any;
  public RepresentedByLawyer = DisputeRepresentedByLawyer;

  public noticeOfDispute: NoticeOfDispute;

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

  public ngOnInit() {
    this.ticketType = this.violationTicketService.ticketType;
    this.countsActions = this.noticeOfDisputeService.getCountsActions(this.noticeOfDispute.dispute_counts);
  }

  resendEmail() {
    this.noticeOfDisputeService.resendVerificationEmail(this.token).subscribe(() => {

    });
  }

  public onPrint(): void {
    window.print();
  }
}
