import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DisputeFormMode } from '@shared/enums/dispute-form-mode';
import { DisputeRepresentedByLawyer, DisputeRequestCourtAppearanceYn } from 'app/api';
import { NoticeOfDisputeService, NoticeOfDispute } from 'app/services/notice-of-dispute.service';

@Component({
  selector: 'app-email-verification-required',
  templateUrl: './email-verification-required.component.html',
  styleUrls: ['./email-verification-required.component.scss'],
})
export class EmailVerificationRequiredComponent implements OnInit {
  private token: string;
  private mode: DisputeFormMode;

  email: string;
  disputeFormMode = DisputeFormMode;
  RepresentedByLawyer = DisputeRepresentedByLawyer;
  RequestCourtAppearance = DisputeRequestCourtAppearanceYn;

  dispute: NoticeOfDispute;

  constructor(
    private route: ActivatedRoute,
    private noticeOfDisputeService: NoticeOfDisputeService,
  ) {
    let params = this.route.snapshot.queryParams;
    this.email = params?.email;
    this.token = params?.token;
    this.mode = parseInt(params?.mode);
  }

  ngOnInit() {
    if (this.mode === DisputeFormMode.CREATE) {
      this.dispute = this.noticeOfDisputeService.noticeOfDispute;
    }
  }

  get isCreate(): boolean {
    return this.mode === DisputeFormMode.CREATE;
  }

  resendEmail() {
    this.noticeOfDisputeService.resendVerificationEmail(this.token).subscribe(() => { });
  }

  onPrint(): void {
    window.print();
  }
}
