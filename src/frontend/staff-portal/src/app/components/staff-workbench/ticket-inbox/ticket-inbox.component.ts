import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DisputeService, Dispute } from 'app/services/dispute.service';
import { DisputeRequestCourtAppearanceYn, DisputeDisputantDetectedOcrIssues, DisputeStatus, DisputeSystemDetectedOcrIssues } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';
import { DateUtil } from '@shared/utils/date-util';

@Component({
  selector: 'app-ticket-inbox',
  templateUrl: './ticket-inbox.component.html',
  styleUrls: ['../../../app.component.scss', './ticket-inbox.component.scss'],
})
export class TicketInboxComponent implements OnInit, AfterViewInit {
  @Output() disputeInfo: EventEmitter<Dispute> = new EventEmitter();

  disputes: Dispute[] = [];
  dataSource = new MatTableDataSource(this.disputes);
  dataFilters = {
    "dateFrom": null,
    "dateTo": null,
    "ticketNumber": null,
    "disputantSurname": null,
    "status": null
  };
  _dataFilters = { ...this.dataFilters };
  statusFilterOptions = [DisputeStatus.New, DisputeStatus.Processing, DisputeStatus.Validated, DisputeStatus.Rejected, DisputeStatus.Cancelled, DisputeStatus.Concluded];
  tableHeight: number = window.innerHeight - 425; // less size of other fixed elements
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

  @ViewChild('tickTbSort') tickTbSort = new MatSort();
  showTicket = false;

  constructor(
    private disputeService: DisputeService,
    private logger: LoggerService,
    private authService: AuthService,
  ) {
    this.disputeService.refreshDisputes.subscribe(x => { this.getAllDisputes(); })
  }

  ngOnInit() {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.userProfile = userProfile;
      }
    })

    // when authentication token available, get data
    this.getAllDisputes();
  }

  ngAfterViewInit() {
    // load dataFilters (Ticket Validation tab) in session for page reload
    let __dataFilter = sessionStorage.getItem("dataFilters-TV");
    if (__dataFilter) {
      this.dataFilters = JSON.parse(__dataFilter);
    }
    
    this.dataSource.sort = this.tickTbSort;
  }

  isNew(d: Dispute): boolean {
    return d.status == DisputeStatus.New && (d.emailAddressVerified === true || !d.emailAddress);
  }

  calcTableHeight(heightOther: number) {
    return Math.min(window.innerHeight - heightOther, (this.dataSource.filteredData.length + 1) * 80)
  }

  getAllDisputes(): void {
    this.logger.log('TicketInboxComponent::getAllDisputes');

    this.disputes = [];
    this.dataSource.data = this.disputes;

    this.disputeService.getDisputes().subscribe((response) => {
      this.logger.info(
        'TicketInboxComponent::getAllDisputes response',
        response
      );

      response.forEach((dispute: Dispute) => {
        dispute.__RedGreenAlert = dispute.status == DisputeStatus.New ? 'Green' : '',
          this.disputes.push(dispute);
      });
      // initially sort data by Date Submitted
      this.dataSource.data = this.disputes.sort((a: Dispute, b: Dispute) => { if (new Date(a.submittedTs) > new Date(b.submittedTs)) { return -1; } else { return 1 } });

      // this section allows filtering by ticket number or partial ticket number by setting the filter predicate
      this.dataSource.filterPredicate = this.searchFilter;
      this.onApplyFilter(null, null);
    });
  }

  searchFilter = function (record: Dispute, filter: string) {
    let searchTerms = JSON.parse(filter);
    var excludingStatuses = !searchTerms?.status ? [DisputeStatus.Cancelled, DisputeStatus.Processing, DisputeStatus.Rejected] : [];
    if (excludingStatuses.includes(record?.status)) {
      return false;
    }
    return Object.entries(searchTerms).every(([field, value]: [string, string]) => {
      if ("dateFrom" === field) {
        return !DateUtil.isValid(value) || DateUtil.isDateOnOrAfter(record.submittedTs, value);
      }
      else if ("dateTo" === field) {
        return !DateUtil.isValid(value) || DateUtil.isDateOnOrBefore(record.submittedTs, value);
      }
      else {
        return record[field].toLocaleLowerCase().indexOf(value?.trim().toLocaleLowerCase() ?? "") != -1;
      }
    });
  };

  resetSearchFilters() {
    // Will update search filters in UI
    this.dataFilters = { ...this._dataFilters };
    // Will re-execute the filter function, but will block UI rendering
    // Put this call in a Timeout to keep UI responsive.
    setTimeout(() => {
      this.dataSource.filter = "{}";
      // FIXME: This static table height has got to go. The panel should vertically extend to the footer (100%) not some arbitrary pixel height that is not resized when the window is resized.
      this.tableHeight = this.calcTableHeight(351);
    }, 100);
  }

  countNewTickets(): number {
    if (this.dataSource.data?.filter((x: Dispute) => x.status == DisputeStatus.New))
      return this.dataSource.data?.filter((x: Dispute) => x.status == DisputeStatus.New).length;
    else return 0;
  }

  // called on keyup in filter field
  onApplyFilter(filterName: string, value: string) {
    if (filterName) {
      const filterValue = value;
      this.dataFilters[filterName] = filterValue;
    }
    this.dataSource.filter = JSON.stringify(this.dataFilters);
    
    // cache dataFilters (for Ticket Validation tab) in session for page reload
    sessionStorage.setItem("dataFilters-TV", this.dataSource.filter);

    this.tableHeight = this.calcTableHeight(351);
  }

  backWorkbench(element: Dispute) {
    this.disputeInfo.emit(element);
  }
}