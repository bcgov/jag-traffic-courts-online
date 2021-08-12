import { Component, Input, OnInit } from '@angular/core';
import { Offence } from '@shared/models/offence.model';

@Component({
  selector: 'app-count-item-dispute-summary',
  templateUrl: './count-item-dispute-summary.component.html',
  styleUrls: ['./count-item-dispute-summary.component.scss'],
})
export class CountItemDisputeSummaryComponent implements OnInit {
  @Input() public count: Offence;

  constructor() {}

  ngOnInit(): void {}
}
