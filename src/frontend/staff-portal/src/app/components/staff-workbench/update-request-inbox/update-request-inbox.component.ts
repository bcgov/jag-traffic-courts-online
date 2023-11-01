import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DisputeService, DisputeWithUpdates } from 'app/services/dispute.service';
import { Dispute, DisputeStatus } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';
import { DateUtil } from '@shared/utils/date-util';

@Component({
  selector: 'app-update-request-inbox',
  templateUrl: './update-request-inbox.component.html',
  styleUrls: ['../../../app.component.scss', './update-request-inbox.component.scss'],
})
export class UpdateRequestInboxComponent implements OnInit, AfterViewInit {
  @Output() public disputeInfo: EventEmitter<Dispute> = new EventEmitter();

  dataSource = new MatTableDataSource();
  dataFilters = {
    "dateFrom": "",
    "dateTo": "",
    "ticketNumber": "",
    "disputantSurname": "",
    "status": ""
  };
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
  tableHeight: number = window.innerHeight - 352; // less size of other fixed elements

  @ViewChild('tickTbSort') tickTbSort = new MatSort();
  public showTicket = false

  constructor(
    public disputeService: DisputeService,
    private logger: LoggerService,
    private authService: AuthService,
  ) {
    this.disputeService.refreshDisputes.subscribe(x => { this.getAllDisputesWithPendingUpdates(); })
  }

  // FIXME: This static table height has got to go. The query results panel should vertically extend to the footer (100%) not some arbitrary pixel height that is not resized when the window is resized.
  calcTableHeight(heightOther: number) {
    return Math.min(window.innerHeight - heightOther, (this.dataSource.filteredData.length + 1) * 60);
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

      this.tableHeight = this.calcTableHeight(352);
    });
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.tickTbSort;
  }

  searchFilter = function (record: DisputeWithUpdates, filter: string) {
    let searchTerms = JSON.parse(filter);
    return Object.entries(searchTerms).every(([field, value]: [string, string]) => {
      if ("dateFrom" === field) {
        return !DateUtil.isValid(value) || DateUtil.isDateOnOrAfter(record.submittedTs, value);
      }
      else if ("dateTo" === field) {
        return !DateUtil.isValid(value) || DateUtil.isDateOnOrBefore(record.submittedTs, value);
      }
      else {
        return record[field].toLocaleLowerCase().indexOf(value.trim().toLocaleLowerCase()) != -1;
      }
    });
  };

  onApplyFilter(filterName: string, value: string) {
    const filterValue = value;
    this.dataFilters[filterName] = filterValue;
    this.dataSource.filter = JSON.stringify(this.dataFilters);
    this.tableHeight = this.calcTableHeight(352);
  }

  resetSearchFilters() {
    // Will update search filters in UI
    this.dataFilters = {
      "dateFrom": "",
      "dateTo": "",
      "ticketNumber": "",
      "disputantSurname": "",
      "status": ""
    };

    // Will re-execute the filter function, but will block UI rendering
    // Put this call in a Timeout to keep UI responsive.
    setTimeout(() => {
      this.dataSource.filter = "{}";
      this.tableHeight = this.calcTableHeight(352);
    }, 100);
  }

  backWorkbench(element) {
    this.disputeInfo.emit(element);
  }

}
