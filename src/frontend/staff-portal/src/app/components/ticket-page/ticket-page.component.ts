import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import {MatSort, Sort} from '@angular/material/sort';

@Component({
  selector: 'app-ticket-page',
  templateUrl: './ticket-page.component.html',
  styleUrls: ['./ticket-page.component.scss'],
})
export class TicketPageComponent implements OnInit, AfterViewInit {
  dataSource = new MatTableDataSource();
  displayedColumns: string[] = [
    'DateSubmitted',
    'Ticket',
    'Surname',
    'GivenName',
    'RequestType',
    'Status',
    'FilingDate',
    'CourtHearing',
    'CitizenFlag',
    'SystemFlag',
    'AssignedTo',
  ];
  remoteDummyData = [
    {
      RedGreenAlert: 'Green',
      DateSubmitted: new Date('2022/02/08'),
      Ticket: 'AJ00214578',
      Surname: 'McGibbons',
      GivenName: 'Julius Montgommery',
      RequestType: 'Dispute',
      Status: 'New',
      FilingDate: undefined,
      CourtHearing: 'Y',
      CitizenFlag: 'Y',
      SystemFlag: 'Y',
      AssignedTo: undefined,
    },
    {
      RedGreenAlert: 'Green',
      DateSubmitted: new Date('2022/02/08'),
      Ticket: 'EZ02000460',
      Surname: 'Smithe',
      GivenName: 'Jaxon',
      RequestType: 'Dispute',
      Status: 'New',
      FilingDate: undefined,
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'Y',
      AssignedTo: undefined,
    },
    {
      RedGreenAlert: 'Green',
      DateSubmitted: new Date('2022/02/08'),
      Ticket: 'AJ00214578',
      Surname: 'Jacklin',
      GivenName: 'Susanne',
      RequestType: 'Admin',
      Status: 'New',
      FilingDate: undefined,
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: undefined,
    },
    {
      RedGreenAlert: 'Green',
      DateSubmitted: new Date('2022/02/08'),
      Ticket: 'AJ00214578',
      Surname: 'Morris',
      GivenName: 'Mark',
      RequestType: 'Dispute',
      Status: 'New',
      FilingDate: undefined,
      CourtHearing: 'Y',
      CitizenFlag: 'Y',
      SystemFlag: 'N',
      AssignedTo: undefined,
    },
    {
      RedGreenAlert: 'Green',
      DateSubmitted: new Date('2022/02/08'),
      Ticket: 'AJ00214578',
      Surname: 'Korrin',
      GivenName: 'Karen',
      RequestType: 'Dispute',
      Status: 'New',
      FilingDate: undefined,
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: undefined,
    },
    {
      RedGreenAlert: null,
      DateSubmitted: new Date('2022/02/06'),
      Ticket: 'AJ00214578',
      Surname: 'Aster',
      GivenName: 'Jack',
      RequestType: 'Dispute',
      Status: 'Checked out',
      FilingDate: undefined,
      CourtHearing: 'Y',
      CitizenFlag: 'Y',
      SystemFlag: 'N',
      AssignedTo: 'Barry Mann',
    },
    {
      RedGreenAlert: null,
      DateSubmitted: new Date('2022/02/06'),
      Ticket: 'AJ00214578',
      Surname: 'Smith',
      GivenName: 'Portia',
      RequestType: 'Admin',
      Status: 'Processing',
      FilingDate: new Date('2022/02/07'),
      CourtHearing: 'Y',
      CitizenFlag: 'Y',
      SystemFlag: 'N',
      AssignedTo: undefined,
    },
    {
      RedGreenAlert: 'Red',
      DateSubmitted: new Date('2022/02/06'),
      Ticket: 'AJ00214578',
      Surname: 'Brown',
      GivenName: 'Will',
      RequestType: 'Admin',
      Status: 'Alert',
      FilingDate: new Date('2022/02/07'),
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: 'Barry Mann',
    },
    {
      RedGreenAlert: null,
      DateSubmitted: new Date('2022/02/05'),
      Ticket: 'AJ00214578',
      Surname: 'Jones',
      GivenName: 'Sharron',
      RequestType: 'Dispute',
      Status: 'Processing',
      FilingDate: new Date('2022/02/06'),
      CourtHearing: 'Y',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: null,
    },
    {
      RedGreenAlert: null,
      DateSubmitted: new Date('2022/02/04'),
      Ticket: 'AJ00214578',
      Surname: 'Price',
      GivenName: 'Simone',
      RequestType: 'Admin',
      Status: 'Processing',
      FilingDate: new Date('2022/02/06'),
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: null,
    },
  ];

  RegionName: string = "";

  @ViewChild('tickTbSort') tickTbSort = new MatSort();

  constructor() {}

  ngOnInit(): void {
    this.dataSource.data = this.remoteDummyData;
    this.RegionName = "Fraser Valley Region";

    // initially sort data by Date Submitted
    this.dataSource.data = this.dataSource.data.sort((a,b)=> { if (a.DateSubmitted > b.DateSubmitted) { return 1; } else { return -1 } } );

    // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
    this.dataSource.filterPredicate = function (record,filter) {
      return record.Ticket.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
    }
  }

  countNewTickets(): number {
    if (this.dataSource.data.filter(x => x.Status == 'New'))
     return this.dataSource.data.filter(x => x.Status == 'New').length;
    else return 0;
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.tickTbSort;
  }

  // called on keyup in filter field
  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
}
