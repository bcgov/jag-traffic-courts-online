import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NoticeOfDisputeService } from 'app/services/notice-of-dispute.service';

@Component({
  selector: 'app-email-verification',
  templateUrl: './email-verification.component.html',
})
export class EmailVerificationComponent {
  private token: string;

  constructor(
    private route: ActivatedRoute,
    private noticeOfDisputeService: NoticeOfDisputeService,
  ) {
    this.route.queryParams.subscribe((params) => {
      this.token = params.token;
      this.noticeOfDisputeService.verifyEmail(this.token).subscribe(() => {});
    });
  }
}
