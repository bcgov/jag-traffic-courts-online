import { Component, Input, OnInit } from '@angular/core';
import { Offence } from '@shared/models/offence.model';

@Component({
  selector: 'app-count-item-summary',
  templateUrl: './count-item-summary.component.html',
  styleUrls: ['./count-item-summary.component.scss'],
})
export class CountItemSummaryComponent implements OnInit {
  @Input() public count: Offence;
  @Input() public selectView = false;

  public selectCount: boolean;

  constructor() {}

  ngOnInit(): void {}

  public selectBox(toggle: boolean): void {
    this.selectCount = toggle;
  }

  public onSelectCountChanged(value: boolean): void {
    this.selectCount = value;
  }

  public get isSelected(): any {
    let selected = false;

    if (this.showCheckbox) {
      selected = this.selectCount;
    }

    return {
      offenceNumber: this.count.offenceNumber,
      selected: selected,
    };
  }

  public get showCheckbox(): boolean {
    return this.selectView && this.count._amountDue > 0;
  }
}
