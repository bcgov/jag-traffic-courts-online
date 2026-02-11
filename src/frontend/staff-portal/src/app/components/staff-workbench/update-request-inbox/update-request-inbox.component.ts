import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DisputeService, DisputeWithUpdates, Dispute } from 'app/services/dispute.service';
import { DisputeStatus, PagedDisputeListItemCollection, SortDirection } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';
import { DateUtil } from '@shared/utils/date-util';
import { TableFilter, TableFilterKeys, TableFilterStatus, TableFilterStatusOptions, UpdateRequestTableStatusDefault } from '@shared/models/table-filter-options.model';
import { TableFilterService } from 'app/services/table-filter.service';
import { Sort } from '@angular/material/sort';

@Component({
  selector: 'app-update-request-inbox',
  templateUrl: './update-request-inbox.component.html',
  styleUrls: ['./update-request-inbox.component.scss'],
})
export class UpdateRequestInboxComponent implements OnInit, AfterViewInit {
  @Input() tabIndex: number;
  @Output() public disputeInfo: EventEmitter<Dispute> = new EventEmitter();

  disputes: Dispute[] = [];
  disputesCollection: PagedDisputeListItemCollection = {};
  dataSource = new MatTableDataSource(this.disputes);

  tableFilterKeys: TableFilterKeys[] = ["dateSubmittedFrom", "dateSubmittedTo", "disputantSurname", "status", "ticketNumber" /*, "courthouseLocation"*/ ]; // TCVP-3258 - temporarily hiding 'courthouseLocation'
  statusFilterOptions = TableFilterStatusOptions;
  defaultStatusFilter = UpdateRequestTableStatusDefault;

  displayedColumns: string[] = [
    '__RedGreenAlert',
    'updateRequest_OldestDate',
    'ticketNumber',
    'disputantSurname',
    'disputantGivenName1',
    // TCVP-3258 - temporarily hiding 'courthouseLocation'
    'hearingDate',
    'changeOfPlea',
    'adjournmentDocument',
    'status',
    'userAssignedTo'
  ];
  filters: TableFilter = new TableFilter();
  previousFilters: TableFilter = new TableFilter();

  currentPage: number = 1;
  totalPages: number = 1;
  sortBy: Array<string> = ["updateRequest_OldestDate"];
  sortDirection: Array<SortDirection> = [SortDirection.Desc];

  public userProfile: KeycloakProfile = {};

  @ViewChild('tickTbSort') tickTbSort = new MatSort();
  public showTicket = false

  constructor(
    public disputeService: DisputeService,
    private logger: LoggerService,
    private authService: AuthService,
    private tableFilterService: TableFilterService,
    
  ) {
    this.disputeService.refreshDisputes.subscribe(x => { 
      this.getAllDisputesWithPendingUpdates(); 
    })
  }

  public async ngOnInit() {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.userProfile = userProfile;
      }
    })

    // when authentication token available, get data
    let dataFilter: TableFilter = this.tableFilterService.tableFilters[this.tabIndex];
    this.filters = dataFilter;
    this.previousFilters = { ...dataFilter };
    this.currentPage = this.tableFilterService.currentPage[this.tabIndex];
    this.getAllDisputesWithPendingUpdates();
  }

  getAllDisputesWithPendingUpdates(): void {
    this.logger.log('UpdateRequestInboxComponent::getAllDisputesWithPendingUpdates');
    this.dataSource.data = [];
    this.disputeService
      .getDisputesWithPendingUpdates(this.sortBy, this.sortDirection, this.currentPage != 0 ? this.currentPage : 1, this.filters)
      .subscribe((response) => {
        this.disputes = [];
        this.logger.info(
          'UpdateRequestInboxComponent::getAllDisputesWithPendingUpdates response',
          response
        );

        this.disputesCollection = response;
        this.currentPage = response.pageNumber;
        this.totalPages = response.totalPages;
        if(!this.totalPages){
          this.currentPage = 0;
        }

        response.items.forEach((dispute: Dispute) => {
          dispute.__RedGreenAlert = dispute.status == DisputeStatus.New ? 'Green' : '',
            this.disputes.push(dispute);
        });      
        this.dataSource.data = this.disputes;
      }
    );
  }

    sortData(sort: Sort){
      this.sortBy = [sort.active];
      this.sortDirection = [sort.direction ? sort.direction as SortDirection : SortDirection.Desc];
      this.currentPage = 1;
      this.tableFilterService.currentPage[this.tabIndex] = 1;
      this.getAllDisputesWithPendingUpdates();
    }

  ngAfterViewInit() {
    this.dataSource.sort = this.tickTbSort;
  }


  onApplyFilter(dataFilters: TableFilter) {
    if (JSON.stringify(this.previousFilters) !== JSON.stringify(dataFilters)) { // Add this line
      this.currentPage = 1;
      this.tableFilterService.currentPage[this.tabIndex] = 1;
    }
    this.filters = dataFilters;
    this.previousFilters = { ...dataFilters };
    this.getAllDisputesWithPendingUpdates();
  }

  onPageChange(event: number) {
    this.currentPage = event;
    this.tableFilterService.currentPage[this.tabIndex] = event;
    this.getAllDisputesWithPendingUpdates();
  }

  backWorkbench(element) {
    this.disputeInfo.emit(element);
  }
}
