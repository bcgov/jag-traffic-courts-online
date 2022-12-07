import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DisputeService } from 'app/services/dispute.service';

@Component({
  selector: 'app-email-verification',
  templateUrl: './email-verification.component.html',
  styleUrls: ['../ticket-page/ticket-page.component.scss']
})
export class EmailVerificationComponent {
  private token: string;
  verified: boolean = false;
  checking: boolean = true;

  constructor(
    private route: ActivatedRoute,
    private disputeService: DisputeService,
  ) {
    this.route.queryParams.subscribe((params) => {
      this.token = params.token;
      this.disputeService.verifyEmail(this.token).subscribe(() => {
        this.checking = false;
        this.verified = true;
      }, error => { 
        this.checking = false; 
      });
    });
  }
}
