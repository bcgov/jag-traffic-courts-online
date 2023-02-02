import { Component, Input, OnInit } from '@angular/core';
import { ViolationTicketCount } from 'app/api';

@Component({
  selector: 'app-count-item-summary',
  templateUrl: './count-item-summary.component.html',
  styleUrls: ['./count-item-summary.component.scss'],
})
export class CountItemSummaryComponent implements OnInit {
  @Input() count: ViolationTicketCount;
  @Input() selectView = false;

  selectCount: boolean;

  constructor() { }

  // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
  ngOnInit(): void { }

  selectBox(toggle: boolean): void {
    this.selectCount = toggle;
  }

  onSelectCountChanged(value: boolean): void {
    this.selectCount = value;
  }

  get isSelected(): any {
    let selected = false;

    if (this.showCheckbox) {
      selected = this.selectCount;
    }

    return {
      offenceNumber: this.count.count_no,
      amount: this.count.ticketed_amount,
      selected,
    };
  }

  get showCheckbox(): boolean {
    return this.selectView && this.count.ticketed_amount > 0;
  }
}
