import {
  Component,
  Input,
  OnInit,
  QueryList,
  ViewChildren,
} from '@angular/core';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { CountItemSummaryComponent } from '@components/initiate-resolution/count-item-summary/count-item-summary.component';
import { ViolationTicket } from 'app/api';
import { ViolationTicketService } from 'app/services/violation-ticket.service';

@Component({
  selector: 'app-count-summary',
  templateUrl: './count-summary.component.html',
  styleUrls: ['./count-summary.component.scss'],
})
export class CountSummaryComponent implements OnInit {
  @ViewChildren('countitem')
  countComponents: QueryList<CountItemSummaryComponent>;
  @Input() ticket: ViolationTicket;
  @Input() selectView = false;

  defaultLanguage: string;

  constructor(public violationTicketService: ViolationTicketService) { }

  // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
  ngOnInit(): void {
  }

  onSelectAllChange(event: MatCheckboxChange): void {
    this.countComponents.forEach((child) => {
      child.selectBox(event.checked);
    });
  }
}
