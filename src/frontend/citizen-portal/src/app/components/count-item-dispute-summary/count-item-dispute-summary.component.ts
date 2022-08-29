import { Component, Input, OnInit } from '@angular/core';
import { DisputeCountPleaCode, ViolationTicketCount, DisputeCountRequestCourtAppearance, DisputeCount, DisputeCountRequestReduction, DisputeCountRequestTimeToPay } from 'app/api';

@Component({
  selector: 'app-count-item-dispute-summary',
  templateUrl: './count-item-dispute-summary.component.html',
  styleUrls: ['./count-item-dispute-summary.component.scss'],
})
export class CountItemDisputeSummaryComponent implements OnInit {
  @Input() public count: any;
  public Plea = DisputeCountPleaCode;
  public RequestCourtAppearance = DisputeCountRequestCourtAppearance;
  public RequestReduction = DisputeCountRequestReduction;
  public RequestTimeToPay = DisputeCountRequestTimeToPay;

  constructor() { // do nothing.
  }

  // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
  ngOnInit(): void {
  }
}
