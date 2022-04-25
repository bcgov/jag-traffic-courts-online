import { Component, OnInit, ViewChild, AfterViewInit} from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DisputesService } from 'app/services/disputes.service';
import { Dispute } from 'app/api/model/dispute.model';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-ticket-page',
  templateUrl: './ticket-page.component.html',
  styleUrls: ['./ticket-page.component.scss', '../../app.component.scss'],
})
export class TicketPageComponent implements OnInit, AfterViewInit {
  dataSource = new MatTableDataSource();
  busy: Subscription;
  public ticketInfo:any;
  displayedColumns: string[] = [
    'RedGreenAlert',
    'DateSubmitted',
    'ticketNumber',
    'disputantSurname',
    'givenNames',
    'Status',
    'FilingDate',
    'CourtHearing',
    'CitizenFlag',
    'SystemFlag',
    'AssignedTo',
  ];
  disputes: Dispute[] = [];
  remoteDummyData: disputeData[] = [
    {
      DateSubmitted: new Date('2022/02/08'),
      ticketNumber: 'AJ00214578',
      disputantSurname: 'McGibbons',
      givenNames: 'Julius Montgommery',
      Status: 'New',
      FilingDate: undefined,
      CourtHearing: 'Y',
      CitizenFlag: 'Y',
      SystemFlag: 'Y',
      AssignedTo: undefined,
    },
    {
      DateSubmitted: new Date('2022/02/08'),
      ticketNumber: 'EZ02000460',
      disputantSurname: 'Smithe',
      givenNames: 'Jaxon',
      Status: 'New',
      FilingDate: undefined,
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'Y',
      AssignedTo: undefined,
    },
    {
      DateSubmitted: new Date('2022/02/08'),
      ticketNumber: 'AJ00214578',
      disputantSurname: 'Jacklin',
      givenNames: 'Susanne',
      Status: 'New',
      FilingDate: undefined,
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: undefined,
    },
    {
      DateSubmitted: new Date('2022/02/08'),
      ticketNumber: 'AJ00214578',
      disputantSurname: 'Morris',
      givenNames: 'Mark',
      Status: 'New',
      FilingDate: undefined,
      CourtHearing: 'Y',
      CitizenFlag: 'Y',
      SystemFlag: 'N',
      AssignedTo: undefined,
    },
    {
      DateSubmitted: new Date('2022/02/08'),
      ticketNumber: 'AJ00214578',
      disputantSurname: 'Korrin',
      givenNames: 'Karen',
      Status: 'New',
      FilingDate: undefined,
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: undefined,
    },
    {
      DateSubmitted: new Date('2022/02/06'),
      ticketNumber: 'AJ00214578',
      disputantSurname: 'Aster',
      givenNames: 'Jack',
      Status: 'Checked out',
      FilingDate: undefined,
      CourtHearing: 'Y',
      CitizenFlag: 'Y',
      SystemFlag: 'N',
      AssignedTo: 'Barry Mann',
    },
    {
      DateSubmitted: new Date('2022/02/06'),
      ticketNumber: 'AJ00214578',
      disputantSurname: 'Smith',
      givenNames: 'Portia',
      Status: 'Processing',
      FilingDate: new Date('2022/02/07'),
      CourtHearing: 'Y',
      CitizenFlag: 'Y',
      SystemFlag: 'N',
      AssignedTo: undefined,
    },
    {
      DateSubmitted: new Date('2022/02/06'),
      ticketNumber: 'AJ00214578',
      disputantSurname: 'Brown',
      givenNames: 'Will',
      Status: 'Alert',
      FilingDate: new Date('2022/02/07'),
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: 'Barry Mann',
    },
    {
      DateSubmitted: new Date('2022/02/05'),
      ticketNumber: 'AJ00214578',
      disputantSurname: 'Jones',
      givenNames: 'Sharron',
      Status: 'Processing',
      FilingDate: new Date('2022/02/06'),
      CourtHearing: 'Y',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: null,
    },
    {
      DateSubmitted: new Date('2022/02/04'),
      ticketNumber: 'AJ00214578',
      disputantSurname: 'Price',
      givenNames: 'Simone',
      Status: 'Processing',
      FilingDate: new Date('2022/02/06'),
      CourtHearing: 'N',
      CitizenFlag: 'N',
      SystemFlag: 'N',
      AssignedTo: null,
    },
  ];

  RegionName: string = "";
  todayDate: Date = new Date();
  fullName: string = "Loading...";
  isLoggedIn: boolean = false;
  accessToken: string = "";

  @ViewChild('tickTbSort') tickTbSort = new MatSort();
  public showTicket = false
  constructor(
    public disputesService: DisputesService,  
    private logger: LoggerService,
    ) {  }

  ngOnInit(): void {

    this.dataSource.data = this.remoteDummyData as disputeData[];
    this.RegionName = "Fraser Valley Region";

    // initially sort data by Date Submitted
    this.dataSource.data = this.dataSource.data.sort((a:disputeData,b:disputeData)=> { if (a.DateSubmitted > b.DateSubmitted) { return -1; } else { return 1 } } );

    // set red green alert
    this.remoteDummyData.forEach(x => {x.RedGreenAlert = x.Status == 'New' ? 'Green' : (x.Status == 'Alert' ? 'Red' : '' )});

    // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
    this.dataSource.filterPredicate = function (record:disputeData ,filter) {
      return record.ticketNumber.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
    }

    // when authentication token available, get data
    this.getAllDisputes();
  }

  getAllDisputes(): void {
      this.logger.log('TicketPageComponent::getAllDisputes');
  
      this.busy = this.disputesService.getDisputes().subscribe((response) => {
        this.logger.info(
          'TicketPageComponent::getAllDisputes response',
          response
        );

        console.log(this.disputes);
   
        this.disputesService.disputes$.next(response);
        this.disputes = response;
      });
  }

  countNewTickets(): number {
    if (this.dataSource.data.filter((x:disputeData) => x.Status == 'New'))
     return this.dataSource.data.filter((x:disputeData) => x.Status == 'New').length;
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

  backTicketList(element){
    this.ticketInfo=element
    this.showTicket = !this.showTicket;
  }
  backTicketpage(){
    this.showTicket = !this.showTicket;
  }
}
export interface disputeData extends Dispute {
  DateSubmitted?: Date, // same as violationDate?
  RedGreenAlert?: string,
  FilingDate?: Date, // extends citizen portal, set in staff portal, initially undefined
  Status: string,  // extends citizen portal set in staff portal, New at first
  CourtHearing: string, // if at least one count requests court hearing
  CitizenFlag: string, // comes from citizen portal, citizen has noticed OCR differences
  SystemFlag: string, // comes from citizen portal, system finds OCR discrepancies
  AssignedTo: string, // extends citizen portal, set in staff portal, undefined at first
}