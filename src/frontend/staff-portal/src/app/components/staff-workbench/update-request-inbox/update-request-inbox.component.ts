import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DisputeService, Dispute } from 'app/services/dispute.service';
import { DisputeCountRequestCourtAppearance, DisputeStatus, OcrViolationTicket, Field } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';

@Component({
  selector: 'app-update-request-inbox',
  templateUrl: './update-request-inbox.component.html',
  styleUrls: ['../../../app.component.scss', './update-request-inbox.component.scss'],
})
export class UpdateRequestInboxComponent implements OnInit, AfterViewInit {
  @Output() public disputeInfo: EventEmitter<Dispute> = new EventEmitter();

  dataSource = new MatTableDataSource();
  busy: Subscription;
  displayedColumns: string[] = [
    '__RedGreenAlert',
    '__DateSubmitted',
    'ticketNumber',
    'disputantSurname',
    'disputantGivenNames',
    'status',
    '__FilingDate',
    '__CourtHearing',
     'userAssignedTo',
  ];
  disputes: Dispute[] = [];
  public userProfile: KeycloakProfile = {};
  public RequestCourtAppearance = DisputeCountRequestCourtAppearance;

  @ViewChild('tickTbSort') tickTbSort = new MatSort();
  public showTicket = false

  constructor(
    public disputeService: DisputeService,
    private logger: LoggerService,
    private authService: AuthService,
  ) {
    this.disputeService.refreshDisputes.subscribe(x => {this.getAllDisputesWithPendingUpdates();})
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

  isNew(d: Dispute): boolean {
    return d.status == DisputeStatus.New && (d.emailAddressVerified === true || !d.emailAddress);
  }

  getAllDisputesWithPendingUpdates(): void {
    this.logger.log('UpdateRequestInboxComponent::getAllDisputesWithPendingUpdates');

    this.disputes = [];

    this.dataSource.data = this.disputes;

    // initially sort data by Date Submitted
    this.dataSource.data = this.dataSource.data.sort((a: Dispute, b: Dispute) => { if (a.__DateSubmitted > b.__DateSubmitted) { return -1; } else { return 1 } });

    // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
    this.dataSource.filterPredicate = function (record: Dispute, filter) {
      return record.ticketNumber.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
    }

    this.busy = this.disputeService.getDisputesWithPendingUpdates().subscribe((response) => {
      this.logger.info(
        'UpdateRequestInboxComponent::getAllDisputesWithPendingUpdates response',
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
            emailAddressVerified: d.emailAddressVerified,
            emailAddress: d.emailAddress,
            __CourtHearing: false,
            __DateSubmitted: new Date(d.submittedTs),
            __FilingDate: d.filingDate != null ? new Date(d.filingDate) : null,
            __UserAssignedTs: d.userAssignedTs != null ? new Date(d.userAssignedTs) : null,
            additionalProperties: d.additionalProperties,
            status: d.status,
            __RedGreenAlert: d.status == DisputeStatus.New ? 'Green' : '',
            userAssignedTs: d.userAssignedTs
          }

          // set court hearing to true if its true for any one of the three possible counts
          // otherwise false
          if (d.disputeCounts) d.disputeCounts.forEach(c => {
            if (c.requestCourtAppearance === this.RequestCourtAppearance.Y) {
              newDispute.__CourtHearing = true;
            }
          });

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
    });
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.tickTbSort;
  }

  // called on keyup in filter field
  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  backWorkbench(element) {
    console.log("backworkbench");
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
