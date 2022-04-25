import { Component, OnInit, ViewChild, AfterViewInit} from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { OidcSecurityService } from "angular-auth-oidc-client";
import { LogInOutService } from 'app/services/log-in-out.service';
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
  disputes: Dispute[] = [];
  remoteDummyData: disputeData[] = [
    {
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
  todayDate: Date = new Date();
  fullName: string = "Loading...";
  isLoggedIn: boolean = false;
  accessToken: string = "";

  @ViewChild('tickTbSort') tickTbSort = new MatSort();
  public showTicket = false
  constructor(
    public oidcSecurityService : OidcSecurityService, 
    private logInOutService : LogInOutService,
    public disputesService: DisputesService,  
    private logger: LoggerService,
    private router: Router,
    ) {  }

  ngOnInit(): void {

    this.logInOutService.getLogoutStatus.subscribe((data) => {
      if (data !== null || data !== '')
      {
        if(data === 'BCeID Login'){
          this.login();
        }
        else
          if(data === 'Logout'){
            this.logout();
          }
      }
    })

    this.oidcSecurityService.checkAuth().subscribe(
      ({ isAuthenticated, userData, accessToken }) => {
        if (isAuthenticated !== true)
        {
          this.router.navigate(['error/401']);
        }
        this.accessToken = accessToken;
        this.logInOutService.currentUser(isAuthenticated);
    });


    this.dataSource.data = this.remoteDummyData as disputeData[];
    this.RegionName = "Fraser Valley Region";

    // initially sort data by Date Submitted
    this.dataSource.data = this.dataSource.data.sort((a:disputeData,b:disputeData)=> { if (a.DateSubmitted > b.DateSubmitted) { return -1; } else { return 1 } } );

    // set red green alert
    this.remoteDummyData.forEach(x => {x.RedGreenAlert = x.Status == 'New' ? 'Green' : (x.Status == 'Alert' ? 'Red' : '' )});

    // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
    this.dataSource.filterPredicate = function (record:disputeData ,filter) {
      return record.Ticket.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
    }

    // when authentication token available, get data
    // this.getAllDisputes();
  }

  login() {
    this.oidcSecurityService.authorize();
  }

  logout() {
    this.oidcSecurityService.logoffAndRevokeTokens();
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
export interface disputeData {
  RedGreenAlert?: string,
  DateSubmitted: Date,
  Ticket: string,
  Surname: string,
  GivenName: string,
  RequestType: string,
  Status: string,
  FilingDate?: Date,
  CourtHearing: string,
  CitizenFlag: string,
  SystemFlag: string,
  AssignedTo: string,
}
