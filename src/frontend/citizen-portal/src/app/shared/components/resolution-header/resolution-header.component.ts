import { Component, Input, OnInit } from '@angular/core';
import { ViolationTicket } from 'app/api';

@Component({
  selector: 'app-resolution-header',
  templateUrl: './resolution-header.component.html',
  styleUrls: ['./resolution-header.component.scss'],
})
export class ResolutionHeaderComponent implements OnInit {
  @Input() public ticket: ViolationTicket;

  constructor() { }

  // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
  ngOnInit(): void { }
}
