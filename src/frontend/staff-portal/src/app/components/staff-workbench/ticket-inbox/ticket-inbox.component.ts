import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DisputeService, Dispute } from 'app/services/dispute.service';
import { DisputeRequestCourtAppearanceYn, DisputeDisputantDetectedOcrIssues, DisputeStatus, OcrViolationTicket, Field, DisputeSystemDetectedOcrIssues, DisputeListItem } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';

@Component({
  selector: 'app-ticket-inbox',
  templateUrl: './ticket-inbox.component.html',
  styleUrls: ['../../../app.component.scss', './ticket-inbox.component.scss'],
})
export class TicketInboxComponent implements OnInit, AfterViewInit {
  @Output() public disputeInfo: EventEmitter<Dispute> = new EventEmitter();

  dataSource = new MatTableDataSource();
  tableHeight: number = window.innerHeight - 425; // less size of other fixed elements
  busy: Subscription;
  displayedColumns: string[] = [
    '__RedGreenAlert',
    '__DateSubmitted',
    'ticketNumber',
    'disputantSurname',
    'disputantGivenNames',
    'status',
    '__FilingDate',
    'requestCourtAppearanceYn',
    'disputantDetectedOcrIssues',
    'systemDetectedOcrIssues',
    'userAssignedTo',
  ];
  disputes: Dispute[] = [];
  public userProfile: KeycloakProfile = {};
  public RequestCourtAppearance = DisputeRequestCourtAppearanceYn;
  public DisputantDetectedOcrIssues = DisputeDisputantDetectedOcrIssues;
  public SystemDetectedOcrIssues = DisputeSystemDetectedOcrIssues;

  @ViewChild('tickTbSort') tickTbSort = new MatSort();
  public showTicket = false

  constructor(
    public disputeService: DisputeService,
    private logger: LoggerService,
    private authService: AuthService,
  ) {
    this.disputeService.refreshDisputes.subscribe(x => {this.getAllDisputes();})
  }

  public async ngOnInit() {
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
    return Math.min(window.innerHeight - heightOther, (this.dataSource.filteredData.length + 1)*80)
  }

  getAllDisputes(): void {
    this.logger.log('TicketInboxComponent::getAllDisputes');

    this.disputes = [];

    this.dataSource.data = this.disputes;

    // initially sort data by Date Submitted
    this.dataSource.data = this.dataSource.data.sort((a: Dispute, b: Dispute) => { if (a.__DateSubmitted > b.__DateSubmitted) { return -1; } else { return 1 } });

    // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
    this.dataSource.filterPredicate = function (record: Dispute, filter) {
      return record.ticketNumber.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
    }

    this.busy = this.disputeService.getDisputes().subscribe((response) => {
      this.logger.info(
        'TicketInboxComponent::getAllDisputes response',
        response
      );

      response.forEach(d => {
        if (d.status != "CANCELLED") { // do not show cancelled
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
            __FilingDate: d.filingDate != null ? new Date(d.filingDate) : null,
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

      // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
      this.dataSource.filterPredicate = function (record: Dispute, filter) {
        return record.ticketNumber.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
      }

      this.tableHeight = this.calcTableHeight(425);
    });
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
  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue?.trim().toLowerCase();
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
