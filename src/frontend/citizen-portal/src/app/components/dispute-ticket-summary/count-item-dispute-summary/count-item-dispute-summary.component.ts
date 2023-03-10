import { Component, Input, OnInit } from '@angular/core';
import { DisputeCountPleaCode, DisputeRequestCourtAppearanceYn, DisputeCountRequestReduction, DisputeCountRequestTimeToPay, ViolationTicketCount } from 'app/api';
import { DisputeCount } from 'app/services/notice-of-dispute.service';

@Component({
  selector: 'app-count-item-dispute-summary',
  templateUrl: './count-item-dispute-summary.component.html',
  styleUrls: ['./count-item-dispute-summary.component.scss'],
})
export class CountItemDisputeSummaryComponent implements OnInit {
  @Input() ticketCount: ViolationTicketCount;
  @Input() disputeCount: DisputeCount;
  Plea = DisputeCountPleaCode;
  RequestCourtAppearance = DisputeRequestCourtAppearanceYn;
  RequestReduction = DisputeCountRequestReduction;
  RequestTimeToPay = DisputeCountRequestTimeToPay;

  constructor() { // do nothing.
  }

  // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
  ngOnInit(): void {
  }
}
