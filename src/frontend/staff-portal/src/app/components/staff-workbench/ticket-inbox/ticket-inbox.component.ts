import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';
import { MatSort, Sort } from '@angular/material/sort';
import { DisputeService, Dispute } from 'app/services/dispute.service';
import { DisputeRequestCourtAppearanceYn, DisputeDisputantDetectedOcrIssues, DisputeStatus, DisputeSystemDetectedOcrIssues, PagedDisputeListItemCollection, SortDirection } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';
import { DateUtil } from '@shared/utils/date-util';
import { TableFilter, TableFilterKeys } from '@shared/models/table-filter-options.model';
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
  statusFilterOptions = [DisputeStatus.New, DisputeStatus.Processing, DisputeStatus.Validated, DisputeStatus.Rejected, DisputeStatus.Cancelled, DisputeStatus.Concluded];

  displayedColumns: string[] = [
    '__RedGreenAlert',
    'submittedTs',
    'ticketNumber',
    'disputantSurname',
    'disputantGivenNames',
    'status',
    'requestCourtAppearanceYn',
    'disputantDetectedOcrIssues',
    'systemDetectedOcrIssues',
    'userAssignedTo',
  ];
  userProfile: KeycloakProfile = {};
  RequestCourtAppearance = DisputeRequestCourtAppearanceYn;
  DisputantDetectedOcrIssues = DisputeDisputantDetectedOcrIssues;
  SystemDetectedOcrIssues = DisputeSystemDetectedOcrIssues;

  showTicket = false;
  currentPage: number = 1;
  totalPages: number = 1;
  sortBy: Array<string> = ["submittedTs"];
  sortDirection: Array<SortDirection> = [SortDirection.Desc];
  newCount: number = 0;

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
    this.getAllDisputes();
    this.countNewTickets();
  }

  isNew(d: Dispute): boolean {
    return d.status == DisputeStatus.New && (d.emailAddressVerified === true || !d.emailAddress);
  }

  getAllDisputes(): void {
    this.logger.log('TicketInboxComponent::getAllDisputes');

    this.disputes = [];

    this.disputeService.getDisputes(this.sortBy, this.sortDirection, this.currentPage, DisputeStatus.New).subscribe((response) => {
      this.logger.info(
        'TicketInboxComponent::getAllDisputes response',
        response
      );

      this.disputesCollection = response;
      this.currentPage = response.pageNumber;
      this.totalPages = response.pageCount;
      response.items.forEach((dispute: Dispute) => {
        dispute.__RedGreenAlert = dispute.status == DisputeStatus.New ? 'Green' : '',
          this.disputes.push(dispute);
      });      
      this.dataSource.data = this.disputes;

      // this section allows filtering by ticket number or partial ticket number by setting the filter predicate
      // this.dataSource.filterPredicate = this.searchFilter;
      // let dataFilter: TableFilter = this.tableFilterService.tableFilters[this.tabIndex];
      // dataFilter.status = dataFilter.status ?? "";
      // this.onApplyFilter(dataFilter);
    });
  }

  searchFilter = function (record: Dispute, filter: string) {
    let searchTerms = JSON.parse(filter);
    var excludingStatuses = !searchTerms?.status ? [DisputeStatus.Cancelled, DisputeStatus.Processing, DisputeStatus.Rejected, DisputeStatus.Concluded] : [];
    if (excludingStatuses.includes(record?.status)) {
      return false;
    }
    return Object.entries(searchTerms).every(([field, value]: [string, string]) => {
      if ("dateSubmittedFrom" === field) {
        return !DateUtil.isValid(value) || DateUtil.isDateOnOrAfter(record.submittedTs, value);
      }
      else if ("dateSubmittedTo" === field) {
        return !DateUtil.isValid(value) || DateUtil.isDateOnOrBefore(record.submittedTs, value);
      }
      else {
        return record[field].toLocaleLowerCase().indexOf(value?.trim().toLocaleLowerCase() ?? "") != -1;
      }
    });
  };

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
    this.dataSource.filter = JSON.stringify(dataFilters);
  }

  backWorkbench(element) {
    this.disputeInfo.emit(element);
  }

  sortData(sort: Sort){
    this.sortBy = [sort.active];
    this.sortDirection = [sort.direction ? sort.direction as SortDirection : SortDirection.Desc];
    this.getAllDisputes();
  }

  onPageChange(event: number) {
    this.currentPage = event;
    this.getAllDisputes();
  }
}