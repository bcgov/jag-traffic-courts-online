import { Component, Input, OnInit } from '@angular/core';
import { Plea } from 'app/api';

@Component({
  selector: 'app-count-item-dispute-summary',
  templateUrl: './count-item-dispute-summary.component.html',
  styleUrls: ['./count-item-dispute-summary.component.scss'],
})
export class CountItemDisputeSummaryComponent implements OnInit {
  @Input() public count: any;
  public Plea = Plea;

  constructor() { // do nothing.
  }

  // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
  ngOnInit(): void {
  }
}
