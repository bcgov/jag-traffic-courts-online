import {
  Component,
  Input,
  OnInit,
  QueryList,
  ViewChildren,
} from '@angular/core';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { CountItemSummaryComponent } from '@components/count-item-summary/count-item-summary.component';
import { ViolationTicket } from 'app/api';

@Component({
  selector: 'app-count-summary',
  templateUrl: './count-summary.component.html',
  styleUrls: ['./count-summary.component.scss'],
})
export class CountSummaryComponent implements OnInit {
  @ViewChildren('countitem')
  countComponents: QueryList<CountItemSummaryComponent>;
  @Input() public ticket: ViolationTicket;
  @Input() public selectView = false;

  public defaultLanguage: string;

  constructor() { }

  // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
  ngOnInit(): void {
  }

  public onSelectAllChange(event: MatCheckboxChange): void {
    this.countComponents.forEach((child) => {
      child.selectBox(event.checked);
    });
  }
}
