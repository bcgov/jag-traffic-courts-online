import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';
import { Sort } from '@angular/material/sort';
import { DisputeService, Dispute } from 'app/services/dispute.service';
import { DisputeRequestCourtAppearanceYn, DisputeDisputantDetectedOcrIssues, DisputeStatus, DisputeSystemDetectedOcrIssues, PagedDisputeListItemCollection, SortDirection, DisputeInterpreterRequired } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';
import { TableFilter, TableFilterKeys, TableFilterStatus, TableFilterStatusOptions, TableFilterStatusDefault } from '@shared/models/table-filter-options.model';
import { TableFilterService } from 'app/services/table-filter.service';

@Component({
  selector: 'app-ticket-inbox',
  templateUrl: './ticket-inbox.component.html',
  styleUrls: ['./ticket-inbox.component.scss'],
})
export class TicketInboxComponent implements OnInit {
  @Input() tabIndex: number;
  @Output() disputeInfo: EventEmitter<Dispute> = new EventEmitter();

  disputes: Dispute[] = [];
  disputesCollection: PagedDisputeListItemCollection = {};
  dataSource = new MatTableDataSource(this.disputes);

  tableFilterKeys: TableFilterKeys[] = ["dateSubmittedFrom", "dateSubmittedTo", "disputantSurname", "status", "ticketNumber"];
  statusFilterOptions = TableFilterStatusOptions;
  statusFilterDefault = TableFilterStatusDefault;

  displayedColumns: string[] = [
    '__RedGreenAlert',
    'submittedTs',
    'ticketNumber',
    'disputantSurname',
    'disputantGivenName1',
    'status',
    'requestCourtAppearanceYn',
    'disputantDetectedOcrIssues',
    'interpreterRequired',
    'userAssignedTo',
  ];
  userProfile: KeycloakProfile = {};
  RequestCourtAppearance = DisputeRequestCourtAppearanceYn;
  DisputantDetectedOcrIssues = DisputeDisputantDetectedOcrIssues;
  SystemDetectedOcrIssues = DisputeSystemDetectedOcrIssues;
  DisputeInterpreterRequired = DisputeInterpreterRequired;

  showTicket = false;
  currentPage: number = 1;
  totalPages: number = 1;
  sortBy: Array<string> = ["submittedTs"];
  sortDirection: Array<SortDirection> = [SortDirection.Desc];
  newCount: number = 0;
  newCountShow: boolean = false;
  filters: TableFilter = new TableFilter();
  previousFilters: TableFilter = new TableFilter();

  constructor(
    private disputeService: DisputeService,
    private logger: LoggerService,
    private authService: AuthService,
    private tableFilterService: TableFilterService,
  ) {
    this.disputeService.refreshDisputes.subscribe(x => { 
      this.getAllDisputes(); 
      this.countNewTickets(); 
    });
  }

  ngOnInit() {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.userProfile = userProfile;
      }
    })

    // when authentication token available, get data
    let dataFilter: TableFilter = this.tableFilterService.tableFilters[this.tabIndex];
    //dataFilter.status = dataFilter.status ?? [];
    this.filters = dataFilter;
    this.previousFilters = { ...dataFilter };
    this.currentPage = this.tableFilterService.currentPage[this.tabIndex];
    this.getAllDisputes();
    this.countNewTickets();
  }

  isNew(d: Dispute): boolean {
    return d.status == DisputeStatus.New && (d.emailAddressVerified === true || !d.emailAddress);
  }

  getAllDisputes(): void {
    this.logger.log('TicketInboxComponent::getAllDisputes');    

    this.disputeService.getDisputes(this.sortBy, this.sortDirection, this.currentPage != 0 ? this.currentPage : 1, 
      this.filters).subscribe((response) => {
      this.disputes = [];
      this.logger.info(
        'TicketInboxComponent::getAllDisputes response',
        response
      );

      this.disputesCollection = response;
      this.currentPage = response.pageNumber;
      this.totalPages = response.pageCount;
      if(!this.totalPages){
        this.currentPage = 0;
      }
      response.items.forEach((dispute: Dispute) => {
        dispute.__RedGreenAlert = dispute.status == DisputeStatus.New ? 'Green' : '',
          this.disputes.push(dispute);
      });      
      this.dataSource.data = this.disputes;
    });
  }

  countNewTickets() {
    this.disputeService.getDisputeStatusCount(DisputeStatus.New).subscribe((response) => {
      this.logger.info(
        'TicketInboxComponent::getDisputeStatusCount response',
        response
      );
      if (response.count) {
        this.newCount = response.count;
      }
    });
  }

  // called on keyup in filter field
  onApplyFilter(dataFilters: TableFilter) {
    if (JSON.stringify(this.previousFilters) !== JSON.stringify(dataFilters)) { // Add this line
      this.currentPage = 1;
      this.tableFilterService.currentPage[this.tabIndex] = 1;
    }
    this.filters = dataFilters;
    this.previousFilters = { ...dataFilters };
    this.getAllDisputes();

    this.newCountShow = (this.filters && this.filters.status) ? this.filters.status.mapping.includes(DisputeStatus.New) : false;
  }

  backWorkbench(element) {
    this.disputeInfo.emit(element);
  }

  sortData(sort: Sort){
    this.sortBy = [sort.active];
    this.sortDirection = [sort.direction ? sort.direction as SortDirection : SortDirection.Desc];
    this.currentPage = 1;
    this.tableFilterService.currentPage[this.tabIndex] = 1;
    this.getAllDisputes();
  }

  onPageChange(event: number) {
    this.currentPage = event;
    this.tableFilterService.currentPage[this.tabIndex] = event;
    this.getAllDisputes();
  }
}