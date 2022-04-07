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
      DateSubmitted: '2022/02/08',
      Ticket: 'AJ00214578',
      Surname: 'McGibbons',
      GivenName: 'Julius Montgommery',
      RequestType: 'Dispute',
      Status: 'New',
      FilingDate: '(null)',
      CourtHearing: 'Y',
      CitizenFlag: 'Y',
      SystemFlag: 'Y',
      AssignedTo: 'Unassigned',
    },
    {
      RedGreenAlert: 'Green',
      DateSubmitted: '2022/02/08',
      Ticket: 'EZ02000460',
      Surname: 'Smithe',
      GivenName: 'Jaxon',
      RequestType: 'Dispute',
      Status: 'New',
      FilingDate: '(null)',
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'Y',
      AssignedTo: 'Unassigned',
    },
    {
      RedGreenAlert: 'Green',
      DateSubmitted: '2022/02/08',
      Ticket: 'AJ00214578',
      Surname: 'Jacklin',
      GivenName: 'Susanne',
      RequestType: 'Admin',
      Status: 'New',
      FilingDate: '(null)',
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: 'Unassigned',
    },
    {
      RedGreenAlert: 'Green',
      DateSubmitted: '2022/02/08',
      Ticket: 'AJ00214578',
      Surname: 'Morris',
      GivenName: 'Mark',
      RequestType: 'Dispute',
      Status: 'New',
      FilingDate: '(null)',
      CourtHearing: 'Y',
      CitizenFlag: 'Y',
      SystemFlag: 'N',
      AssignedTo: 'Unassigned',
    },
    {
      RedGreenAlert: 'Green',
      DateSubmitted: '2022/02/08',
      Ticket: 'AJ00214578',
      Surname: 'Korrin',
      GivenName: 'Karen',
      RequestType: 'Dispute',
      Status: 'New',
      FilingDate: '(null)',
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: 'Unassigned',
    },
    {
      RedGreenAlert: null,
      DateSubmitted: '2022/02/06',
      Ticket: 'AJ00214578',
      Surname: 'Aster',
      GivenName: 'Jack',
      RequestType: 'Dispute',
      Status: 'Checked out',
      FilingDate: '(null)',
      CourtHearing: 'Y',
      CitizenFlag: 'Y',
      SystemFlag: 'N',
      AssignedTo: 'Barry Mann',
    },
    {
      RedGreenAlert: null,
      DateSubmitted: '2022/02/06',
      Ticket: 'AJ00214578',
      Surname: 'Smith',
      GivenName: 'Portia',
      RequestType: 'Admin',
      Status: 'Processing',
      FilingDate: '2022/02/07',
      CourtHearing: 'Y',
      CitizenFlag: 'Y',
      SystemFlag: 'N',
      AssignedTo: 'Unassigned',
    },
    {
      RedGreenAlert: 'Red',
      DateSubmitted: '2022/02/06',
      Ticket: 'AJ00214578',
      Surname: 'Brown',
      GivenName: 'Will',
      RequestType: 'Admin',
      Status: 'Alert',
      FilingDate: '2022/02/07',
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: 'Barry Mann',
    },
    {
      RedGreenAlert: null,
      DateSubmitted: '2022/02/05',
      Ticket: 'AJ00214578',
      Surname: 'Jones',
      GivenName: 'Sharron',
      RequestType: 'Dispute',
      Status: 'Processing',
      FilingDate: '2022/02/06',
      CourtHearing: 'Y',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: 'Unassigned',
    },
    {
      RedGreenAlert: null,
      DateSubmitted: '2022/02/04',
      Ticket: 'AJ00214578',
      Surname: 'Price',
      GivenName: 'Simone',
      RequestType: 'Admin',
      Status: 'Processing',
      FilingDate: '2022/02/06',
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: 'Unassigned',
    },
  ];

  @ViewChild('tickTbSort') tickTbSort = new MatSort();

  constructor() {}

  ngOnInit(): void {
    this.dataSource.data = this.remoteDummyData;
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.tickTbSort;
  }

  public sort(_sort: any, key: any) {
    debugger;
    if (['DateSubmitted', 'FilingDate'].includes(key)) {
      this.remoteDummyData.sort(function (a: any, b: any) {
        if (_sort === 'asc') {
          return new Date(a[key]).valueOf() - new Date(b[key]).valueOf();
        }
        if (_sort === 'desc') {
          return new Date(b[key]).valueOf() - new Date(a[key]).valueOf();
        }
      });
    }

    if (
      [
        'Ticket',
        'Surname',
        'GivenName',
        'Status',
        'RequestType',
        'AssignedTo',
      ].includes(key)
    ) {
      this.remoteDummyData.sort((a: any, b: any) => {
        var nameA = a[key].toLowerCase(),
          nameB = b[key].toLowerCase();
        if (_sort === 'asc') {
          if (nameA < nameB)
            return -1;
          if (nameB > nameA) return 1;
          return 0;
        }
        if (_sort === 'desc') {
          if (nameA > nameB)
          return -1;
        if (nameB < nameA) return 1;
        return 0;
        }
      });
    }

    this.dataSource.data = this.remoteDummyData;
  }
}
