import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NoticeOfDisputeService } from 'app/services/notice-of-dispute.service';
import { AppRoutes } from 'app/app.routes';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-email-verification',
  templateUrl: './email-verification.component.html',
  styleUrls: ['../ticket-page/ticket-page.component.scss']
})
export class EmailVerificationComponent {
  private token: string;
  public verified: boolean = false;
  public checking: boolean = true;

  constructor(
    private route: ActivatedRoute,
    private noticeOfDisputeService: NoticeOfDisputeService,
  ) {
    this.route.queryParams.subscribe((params) => {
      this.token = params.uuid;
      this.noticeOfDisputeService.verifyEmail(this.token).subscribe(() => {
        this.checking = false;
        this.verified = true;
      },
      error => {this.checking = false;});
    });
  }
}
