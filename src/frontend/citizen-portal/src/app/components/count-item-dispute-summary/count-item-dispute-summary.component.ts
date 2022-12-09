import { Component, Input, OnInit } from '@angular/core';
import { DisputeCountPleaCode, DisputeCountRequestCourtAppearance, DisputeCountRequestReduction, DisputeCountRequestTimeToPay } from 'app/api';

@Component({
  selector: 'app-count-item-dispute-summary',
  templateUrl: './count-item-dispute-summary.component.html',
  styleUrls: ['./count-item-dispute-summary.component.scss'],
})
export class CountItemDisputeSummaryComponent implements OnInit {
  @Input() count: any;
  Plea = DisputeCountPleaCode;
  RequestCourtAppearance = DisputeCountRequestCourtAppearance;
  RequestReduction = DisputeCountRequestReduction;
  RequestTimeToPay = DisputeCountRequestTimeToPay;

  constructor() { // do nothing.
  }

  // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
  ngOnInit(): void {
  }
}
