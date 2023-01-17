import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DisputeService, DisputeWithUpdates } from 'app/services/dispute.service';
import { DisputeCountRequestCourtAppearance, DisputeStatus, OcrViolationTicket, Field, Dispute } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';

@Component({
  selector: 'app-update-request-inbox',
  templateUrl: './update-request-inbox.component.html',
  styleUrls: ['../../../app.component.scss', './update-request-inbox.component.scss'],
})
export class UpdateRequestInboxComponent implements OnInit, AfterViewInit {
  @Output() public disputeInfo: EventEmitter<Dispute> = new EventEmitter();

  dataSource = new MatTableDataSource();
  displayedColumns: string[] = [
    '__DateSubmitted',
    'ticketNumber',
    'disputantSurname',
    'disputantGivenNames',
     'hearingDate',
     'changeOfPlea',
     'adjournmentDocument',
     'status',
     'userAssignedTo'
  ];
  disputesWithUpdates: DisputeWithUpdates[] = [];
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

  getAllDisputesWithPendingUpdates(): void {
    this.logger.log('UpdateRequestInboxComponent::getAllDisputesWithPendingUpdates');

    this.disputesWithUpdates = [];

    this.dataSource.data = this.disputesWithUpdates;

    this.disputeService.getDisputesWithPendingUpdates().subscribe((response) => {
      this.logger.info(
        'UpdateRequestInboxComponent::getAllDisputesWithPendingUpdates response',
        response
      );

      response.forEach((d:DisputeWithUpdates) => {
        var newDispute: DisputeWithUpdates = {
          ticketNumber: d.ticketNumber,
          disputantSurname: d.disputantSurname,
          disputantGivenNames: `${d.disputantGivenName1} ${d.disputantGivenName2} ${d.disputantGivenName3}`,
          disputeId: d.disputeId,
          userAssignedTo: d.userAssignedTo,
          emailAddressVerified: d.emailAddressVerified,
          emailAddress: d.emailAddress,
          __DateSubmitted: new Date(d.submittedTs),
          __UserAssignedTs: d.userAssignedTs != null ? new Date(d.userAssignedTs) : null,
          status: d.status,
          userAssignedTs: d.userAssignedTs,
          hearingDate: d.hearingDate,
          changeOfPlea: d.changeOfPlea,
          adjournmentDocument: d.adjournmentDocument
        }

        this.disputesWithUpdates = this.disputesWithUpdates.concat(newDispute);
      });
      this.dataSource.data = this.disputesWithUpdates;

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
    this.disputeInfo.emit(element);
  }

}
