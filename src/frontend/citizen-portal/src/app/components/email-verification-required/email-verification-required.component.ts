import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DisputeFormMode } from '@shared/enums/dispute-form-mode';
import { TicketTypes } from '@shared/enums/ticket-type.enum';
import { DisputeRepresentedByLawyer, DisputeRequestCourtAppearance } from 'app/api';
import { NoticeOfDisputeService, NoticeOfDispute } from 'app/services/notice-of-dispute.service';
import { ViolationTicketService } from 'app/services/violation-ticket.service';

@Component({
  selector: 'app-email-verification-required',
  templateUrl: './email-verification-required.component.html',
  styleUrls: ['./email-verification-required.component.scss'],
})
export class EmailVerificationRequiredComponent implements OnInit {
  private token: string;
  private mode: DisputeFormMode;

  email: string;
  ticketType: string;
  ticketTypes = TicketTypes;
  countsActions: any;
  RepresentedByLawyer = DisputeRepresentedByLawyer;
  RequestCourtAppearance = DisputeRequestCourtAppearance;

  dispute: NoticeOfDispute;

  constructor(
    private route: ActivatedRoute,
    private noticeOfDisputeService: NoticeOfDisputeService,
    private violationTicketService: ViolationTicketService,
  ) {
    let params = this.route.snapshot.queryParams;
    this.email = params?.email;
    this.token = params?.token;
    this.mode = params?.mode;
  }

  ngOnInit() {
    if (this.mode === DisputeFormMode.CREATE) {
      this.dispute = this.noticeOfDisputeService.noticeOfDispute;
      this.ticketType = this.violationTicketService.ticketType;
      this.countsActions = this.noticeOfDisputeService.getCountsActions(this.dispute.dispute_counts);
    }
  }

  get isCreate(): boolean {
    return this.mode === DisputeFormMode.CREATE
  }

  resendEmail() {
    this.noticeOfDisputeService.resendVerificationEmail(this.token).subscribe(() => { });
  }

  onPrint(): void {
    window.print();
  }
}
