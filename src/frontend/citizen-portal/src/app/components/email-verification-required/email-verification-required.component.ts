import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NoticeOfDisputeService } from 'app/services/notice-of-dispute.service';

@Component({
  selector: 'app-email-verification-required',
  templateUrl: './email-verification-required.component.html',
  styleUrls: ['./email-verification-required.component.scss'],
})
export class EmailVerificationRequiredComponent {
  public email: string;
  private token: string;

  constructor(
    private route: ActivatedRoute,
    private noticeOfDisputeService: NoticeOfDisputeService,
  ) {
    this.route.queryParams.subscribe((params) => {
      this.email = params.email;
      this.token = params.token;
    });
  }

  resendEmail() {
    this.noticeOfDisputeService.resendVerificationEmail(this.token).subscribe(() => {
      
    });
  }
}
