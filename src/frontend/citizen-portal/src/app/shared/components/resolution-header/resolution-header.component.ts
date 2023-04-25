import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { QueryParamsForSearch } from '@shared/models/query-params-for-search.model';
import { ViolationTicket } from 'app/api';

@Component({
  selector: 'app-resolution-header',
  templateUrl: './resolution-header.component.html',
  styleUrls: ['./resolution-header.component.scss'],
})
export class ResolutionHeaderComponent implements OnInit {
  @Input() ticket: ViolationTicket;
  params: QueryParamsForSearch;

  ticketNumber: string;
  time: string;
  date: string;

  constructor(
    private route: ActivatedRoute
  ) {
  }

  // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
  ngOnInit(): void {
    this.params = this.route.snapshot.queryParams as QueryParamsForSearch;
    if (this.ticket) {
      this.ticketNumber = this.ticket.ticket_number;
      this.time =this.ticket.issued_date.substring(11,16); // hh:mm
      this.date = this.ticket.issued_date.substring(0,10); // yyyy-mm-dd
    } else if (this.params) {
      this.ticketNumber = this.params?.ticketNumber;
      this.time = this.params?.time;
    }
  }
}
