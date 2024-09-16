import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';
import { MatSort } from '@angular/material/sort';
import { DisputeService, DisputeWithUpdates } from 'app/services/dispute.service';
import { Dispute, DisputeStatus } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';
import { DateUtil } from '@shared/utils/date-util';
import { TableFilter, TableFilterKeys } from '@shared/models/table-filter-options.model';
import { TableFilterService } from 'app/services/table-filter.service';

@Component({
  selector: 'app-update-request-inbox',
  templateUrl: './update-request-inbox.component.html',
  styleUrls: ['./update-request-inbox.component.scss'],
})
export class UpdateRequestInboxComponent implements OnInit, AfterViewInit {
  @Input() tabIndex: number;
  @Output() public disputeInfo: EventEmitter<Dispute> = new EventEmitter();

  dataSource = new MatTableDataSource();
  tableFilterKeys: TableFilterKeys[] = ["dateSubmittedFrom", "dateSubmittedTo", "disputantSurname", "status", "ticketNumber"];
  statusFilterOptions = [DisputeStatus.New, DisputeStatus.Processing, DisputeStatus.Validated, DisputeStatus.Rejected, DisputeStatus.Cancelled, DisputeStatus.Concluded];
  displayedColumns: string[] = [
    'submittedTs',
    'ticketNumber',
    'disputantSurname',
    'disputantGivenNames',
    'hearingDate',
    'changeOfPlea',
    'adjournmentDocument',
    'status',
    'userAssignedTo'
  ];
  public userProfile: KeycloakProfile = {};

  @ViewChild('tickTbSort') tickTbSort = new MatSort();
  public showTicket = false

  constructor(
    public disputeService: DisputeService,
    private logger: LoggerService,
    private authService: AuthService,
    private tableFilterService: TableFilterService,
  ) {
    this.disputeService.refreshDisputes.subscribe(x => { this.getAllDisputesWithPendingUpdates(); })
  }

  public async ngOnInit() {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.userProfile = userProfile;
      }
    })

    // when authentication token available, get data
    this.getAllDisputesWithPendingUpdates();
  }

  getAllDisputesWithPendingUpdates(): void {
    this.logger.log('UpdateRequestInboxComponent::getAllDisputesWithPendingUpdates');

    this.dataSource.data = [];

    this.disputeService.getDisputesWithPendingUpdates().subscribe((response) => {
      this.logger.info(
        'UpdateRequestInboxComponent::getAllDisputesWithPendingUpdates response',
        response
      );

      this.dataSource.data = response as DisputeWithUpdates[];

      // initially sort data by Date Submitted
      this.dataSource.data = this.dataSource.data.sort((a: DisputeWithUpdates, b: DisputeWithUpdates) => { if (a.submittedTs > b.submittedTs) { return -1; } else { return 1 } });

      // this section allows filtering by ticket number or partial ticket number by setting the filter predicate
      this.dataSource.filterPredicate = this.searchFilter;
      this.onApplyFilter(this.tableFilterService.tableFilters[this.tabIndex]);
    });
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.tickTbSort;
  }

  searchFilter = function (record: DisputeWithUpdates, filter: string) {
    let searchTerms = JSON.parse(filter);
    return Object.entries(searchTerms).every(([field, value]: [string, string]) => {
      if ("dateSubmittedFrom" === field) {
        return !DateUtil.isValid(value) || DateUtil.isDateOnOrAfter(record.submittedTs, value);
      }
      else if ("dateSubmittedTo" === field) {
        return !DateUtil.isValid(value) || DateUtil.isDateOnOrBefore(record.submittedTs, value);
      }
      else if (record[field]) {
        return record[field].toLocaleLowerCase().indexOf(value.trim().toLocaleLowerCase()) != -1;
      }
    });
  };

  onApplyFilter(dataFilters: TableFilter) {
    this.dataSource.filter = JSON.stringify(dataFilters);
  }

  backWorkbench(element) {
    this.disputeInfo.emit(element);
  }
}
