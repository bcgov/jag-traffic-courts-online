import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DisputeService, Dispute } from 'app/services/dispute.service';
import { DisputeRequestCourtAppearanceYn, DisputeDisputantDetectedOcrIssues, DisputeStatus, OcrViolationTicket, Field, DisputeSystemDetectedOcrIssues, DisputeListItem } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';

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
    "dateFrom": "",
    "dateTo": "",
    "ticketNumber": "",
    "disputantSurname": "",
    "status": ""
  };
  statusFilterOptions = [DisputeStatus.New, DisputeStatus.Processing, DisputeStatus.Validated, DisputeStatus.Rejected, DisputeStatus.Cancelled, DisputeStatus.Concluded];
  tableHeight: number = window.innerHeight - 425; // less size of other fixed elements
  displayedColumns: string[] = [
    '__RedGreenAlert',
    '__DateSubmitted',
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

  private disputeStatusesExcluded = [DisputeStatus.Cancelled, DisputeStatus.Processing, DisputeStatus.Rejected];

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

  isNew(d: Dispute): boolean {
    return d.status == DisputeStatus.New && (d.emailAddressVerified === true || !d.emailAddress);
  }

  calcTableHeight(heightOther) {
    return Math.min(window.innerHeight - heightOther, (this.dataSource.filteredData.length + 1) * 80)
  }

  getAllDisputes(): void {
    this.logger.log('TicketInboxComponent::getAllDisputes');

    this.disputes = [];

    this.dataSource.data = this.disputes;

    // initially sort data by Date Submitted
    this.dataSource.data = this.dataSource.data.sort((a: Dispute, b: Dispute) => { if (a.__DateSubmitted > b.__DateSubmitted) { return -1; } else { return 1 } });

    // this section allows filtering by ticket number or partial ticket number by setting the filter predicate
    this.dataSource.filterPredicate = this.searchFilter;

    this.disputeService.getDisputes().subscribe((response) => {
      this.logger.info(
        'TicketInboxComponent::getAllDisputes response',
        response
      );

      response.forEach(d => {
        if (!this.disputeStatusesExcluded.includes(d.status)) {
          var newDispute: Dispute = {
            ticketNumber: d.ticketNumber,
            disputantSurname: d.disputantSurname,
            disputantGivenNames: d.disputantGivenNames,
            disputeId: d.disputeId,
            userAssignedTo: d.userAssignedTo,
            disputantDetectedOcrIssues: d.disputantDetectedOcrIssues,
            emailAddressVerified: d.emailAddressVerified,
            emailAddress: d.emailAddress,
            systemDetectedOcrIssues: d.systemDetectedOcrIssues,
            __DateSubmitted: new Date(d.submittedTs),
            __UserAssignedTs: d.userAssignedTs != null ? new Date(d.userAssignedTs) : null,
            additionalProperties: d.additionalProperties,
            status: d.status,
            __RedGreenAlert: d.status == DisputeStatus.New ? 'Green' : '',
            userAssignedTs: d.userAssignedTs,
            requestCourtAppearanceYn: d.requestCourtAppearanceYn
          }

          this.disputes = this.disputes.concat(newDispute);
        }
      });
      this.dataSource.data = this.disputes;

      // initially sort data by Date Submitted
      this.dataSource.data = this.dataSource.data.sort((a: Dispute, b: Dispute) => { if (a.submittedTs > b.submittedTs) { return -1; } else { return 1 } });

      // this section allows filtering by ticket number or partial ticket number by setting the filter predicate
      this.dataSource.filterPredicate = this.searchFilter;

      this.tableHeight = this.calcTableHeight(425);
    });
  }

  searchFilter = function (record: Dispute, filter: string) {
    let searchTerms = JSON.parse(filter);
    return Object.entries(searchTerms).every(([field, value]: [string, string]) => {
      if ("dateFrom" === field) {
        if (!isNaN(Date.parse(value))) {
          let ds = new Date(record.__DateSubmitted?.getFullYear(), record.__DateSubmitted?.getMonth(), record.__DateSubmitted?.getDate());
          return ds >= new Date(value);
        }
        else {
          return true;
        }
      }
      else if ("dateTo" === field) {
        if (!isNaN(Date.parse(value))) {
          let ds = new Date(record.__DateSubmitted?.getFullYear(), record.__DateSubmitted?.getMonth(), record.__DateSubmitted?.getDate());
          return ds <= new Date(value);
        }
        else {
          return true;
        }
      }
      else {
        return record[field].toLocaleLowerCase().indexOf(value.trim().toLocaleLowerCase()) != -1;
      }
    });
  };

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
      // FIXME: This static table height has got to go. The panel should vertically extend to the footer (100%) not some arbitrary pixel height that is not resized when the window is resized.
      this.tableHeight = this.calcTableHeight(400);
    }, 100);
  }

  countNewTickets(): number {
    if (this.dataSource.data?.filter((x: Dispute) => x.status == DisputeStatus.New))
      return this.dataSource.data?.filter((x: Dispute) => x.status == DisputeStatus.New).length;
    else return 0;
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.tickTbSort;
  }

  // called on keyup in filter field
  onApplyFilter(filterName: string, value: string) {
    const filterValue = value;
    this.dataFilters[filterName] = filterValue;
    this.dataSource.filter = JSON.stringify(this.dataFilters);
    this.tableHeight = this.calcTableHeight(425);
  }

  backWorkbench(element) {
    this.disputeInfo.emit(element);
  }

}

export interface Point {
  x?: number;
  y?: number;
}

export interface OcrCount {
  description?: Field;
  act_or_regulation_name_code?: Field;
  is_act?: Field;
  is_regulation?: Field;
  section?: Field;
  ticketed_amount?: Field;
}
