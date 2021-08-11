import { Component, Input, OnInit } from '@angular/core';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { TicketDispute } from '@shared/models/ticketDispute.model';

@Component({
  selector: 'app-count-summary',
  templateUrl: './count-summary.component.html',
  styleUrls: ['./count-summary.component.scss'],
})
export class CountSummaryComponent implements OnInit {
  @Input() public ticket: TicketDispute;
  @Input() public payView = false;

  public defaultLanguage: string;

  constructor() {}

  ngOnInit(): void {}

  public get within30Days(): boolean {
    let isWithin = false;

    if (this.ticket?.discountDueDate) {
      const today = new Date();
      const discountDate = this.ticket?.discountDueDate;

      const diff = Math.floor(
        (Date.parse(discountDate) - Date.parse(today.toDateString())) / 86400000
      );

      isWithin = diff >= 0 && diff <= 30;
    }

    return isWithin;
  }

  public onSelectAllChange(event: MatCheckboxChange): void {
    console.log('checked: ' + event.checked);
  }
}
