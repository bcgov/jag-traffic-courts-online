import { Component, OnInit, ViewChild, AfterViewInit} from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DisputesService } from 'app/services/disputes.service';
import { Dispute } from 'app/api';
import { DisputeStatus } from 'app/api/model/disputeStatus.model';
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
  newDispute: disputeData = {
    DateSubmitted: undefined,
    ticketNumber: undefined,
    disputantSurname: undefined,
    givenNames: undefined,
    moreDisputeStatus: undefined,
    FilingDate: undefined,
    CourtHearing: undefined,
    CitizenFlag: undefined,
    SystemFlag: undefined,
    AssignedTo: undefined};
  public ticketInfo:any;
  displayedColumns: string[] = [
    'RedGreenAlert',
    'DateSubmitted',
    'ticketNumber',
    'disputantSurname',
    'givenNames',
    'moreDisputeStatus',
    'FilingDate',
    'CourtHearing',
    'CitizenFlag',
    'SystemFlag',
    'AssignedTo',
  ];
  disputes: disputeData[] = [];
  remoteDummyData: disputeData[] = [
    {
      DateSubmitted: new Date('2022/02/08'),
      ticketNumber: 'AJ00214578',
      disputantSurname: 'McGibbons',
      givenNames: 'Julius Montgommery',
      moreDisputeStatus: MoreDisputeStatus.New,
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
      moreDisputeStatus: MoreDisputeStatus.New,
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
      moreDisputeStatus: MoreDisputeStatus.New,
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
      moreDisputeStatus: MoreDisputeStatus.New,
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
      moreDisputeStatus: MoreDisputeStatus.New,
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
      moreDisputeStatus: MoreDisputeStatus.CheckedOut,
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
      moreDisputeStatus: MoreDisputeStatus.Processing,
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
      status: DisputeStatus.Cancelled,
      moreDisputeStatus: MoreDisputeStatus.Alert,
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
      moreDisputeStatus: MoreDisputeStatus.Processing,
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
      moreDisputeStatus: MoreDisputeStatus.Processing,
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

    // set red green alert and status
    this.remoteDummyData.forEach(x => {
      x.RedGreenAlert = x.moreDisputeStatus == MoreDisputeStatus.New ? 'Green' : (x.moreDisputeStatus == MoreDisputeStatus.Alert ? 'Red' : '' );
    });

    this.RegionName = "Fraser Valley Region";

    // when authentication token available, get data
    this.getAllDisputes();
  }

  isNew(d: disputeData): boolean {
    return d.moreDisputeStatus == MoreDisputeStatus.New;
  }

  getAllDisputes(): void {
      this.logger.log('TicketPageComponent::getAllDisputes');

      // concatenate all dummy data to this.disputes
      this.disputes = [];
      this.remoteDummyData.forEach(d => {
        this.disputes = this.disputes.concat(d);
      });

      this.dataSource.data = this.disputes;

      // initially sort data by Date Submitted
      this.dataSource.data = this.dataSource.data.sort((a:disputeData,b:disputeData)=> { if (a.DateSubmitted > b.DateSubmitted) { return -1; } else { return 1 } } );

      // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
      this.dataSource.filterPredicate = function (record:disputeData ,filter) {
        return record.ticketNumber.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
      }

  
      this.busy = this.disputesService.getDisputes().subscribe((response) => {
        this.logger.info(
          'TicketPageComponent::getAllDisputes response',
          response
        );

        this.disputesService.disputes$.next(response);
        response.forEach(d => {
          this.newDispute.AssignedTo = '';
          this.newDispute.CitizenFlag = 'N';
          this.newDispute.CourtHearing = 'N';
          this.newDispute.DateSubmitted = new Date(d.violationDate);
          this.newDispute.FilingDate = undefined;
          switch (d.status) 
          { 
            case DisputeStatus.Cancelled: 
              this.newDispute.moreDisputeStatus = MoreDisputeStatus.Cancelled;
              break; 
            case DisputeStatus.Processing:
              this.newDispute.moreDisputeStatus = MoreDisputeStatus.Cancelled;
              break;
            case DisputeStatus.Rejected:
              this.newDispute.moreDisputeStatus = MoreDisputeStatus.Rejected;
              break;
            default:
              this.newDispute.moreDisputeStatus = MoreDisputeStatus.New;
              break;
          };
          this.newDispute.RedGreenAlert = this.newDispute.moreDisputeStatus == MoreDisputeStatus.New ? 'Green' : (this.newDispute.moreDisputeStatus == MoreDisputeStatus.Alert ? 'Red' : '' );
          this.newDispute.SystemFlag = 'N';
          this.newDispute.additionalProperties = d.additionalProperties;
          this.newDispute.courtLocation = d.courtLocation;
          this.disputes = this.disputes.concat(this.newDispute);
        })
        this.dataSource.data = this.disputes;

        // initially sort data by Date Submitted
        this.dataSource.data = this.dataSource.data.sort((a:disputeData,b:disputeData)=> { if (a.DateSubmitted > b.DateSubmitted) { return -1; } else { return 1 } } );

        // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
        this.dataSource.filterPredicate = function (record:disputeData ,filter) {
          return record.ticketNumber.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
       }


      });
  }

  countNewTickets(): number {
    if (this.dataSource.data.filter((x:disputeData) => x.moreDisputeStatus == MoreDisputeStatus.New))
     return this.dataSource.data.filter((x:disputeData) => x.moreDisputeStatus == MoreDisputeStatus.New).length;
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

type MoreDisputeStatus = 'New' | 'Processing' | 'Rejected' | 'Cancelled' | 'Alert' | 'Checked Out';
const MoreDisputeStatus = {
  New: 'New' as MoreDisputeStatus,
  Processing: 'Processing' as MoreDisputeStatus,
  Rejected: 'Rejected' as MoreDisputeStatus,
  Cancelled: 'Cancelled' as MoreDisputeStatus,
  Alert: 'Alert' as MoreDisputeStatus,
  CheckedOut: 'Checked Out' as MoreDisputeStatus
}
export interface disputeData extends Dispute {
  DateSubmitted?: Date, 
  RedGreenAlert?: string,
  moreDisputeStatus : MoreDisputeStatus;
  FilingDate?: Date, // extends citizen portal, set in staff portal, initially undefined
  CourtHearing: string, // if at least one count requests court hearing
  CitizenFlag: string, // comes from citizen portal, citizen has noticed OCR differences
  SystemFlag: string, // comes from citizen portal, system finds OCR discrepancies
  AssignedTo: string, // extends citizen portal, set in staff portal, undefined at first
}