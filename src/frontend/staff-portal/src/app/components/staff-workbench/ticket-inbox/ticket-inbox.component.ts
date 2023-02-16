import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DisputeService, Dispute } from 'app/services/dispute.service';
import { DisputeRequestCourtAppearanceYn, DisputeDisputantDetectedOcrIssues, DisputeStatus, OcrViolationTicket, Field } from 'app/api';
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
    '__SystemDetectedOcrIssues',
    'userAssignedTo',
  ];
  disputes: Dispute[] = [];
  public userProfile: KeycloakProfile = {};
  public RequestCourtAppearance = DisputeRequestCourtAppearanceYn;
  public DisputantDetectedOcrIssues = DisputeDisputantDetectedOcrIssues;

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
            __SystemDetectedOcrIssues: this.getSystemDetectedOcrIssues(d.violationTicket.ocrViolationTicket),
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

  // data returns Notice of Dispute
  // which has a property ViolationTicket
  // which has a property ocrViolationTicket
  // which is the JSON string for the Azure OCR'd version of a paper ticket
  // __SystemDetectedOcrIssues should be set to true if any OCR'd field has less than 80% confidence
  // so this routine will exit with true at the first field of the fields collection that has an OCR error
  getSystemDetectedOcrIssues(ocrViolationTicket?: OcrViolationTicket): boolean {
    try {
      let fields = ocrViolationTicket?.fields;
      if (fields && fields !== undefined) {

        if (this.getOcrViolationErrors(fields.violationTicketTitle)) { return true; }
        if (this.getOcrViolationErrors(fields.ticket_number)) { return true; }
        if (this.getOcrViolationErrors(fields.disputant_surname)) { return true; }
        if (this.getOcrViolationErrors(fields.disputant_given_names)) { return true; }
        if (this.getOcrViolationErrors(fields.drivers_licence_province)) { return true; }
        if (this.getOcrViolationErrors(fields.drivers_licence_number)) { return true; }
        if (this.getOcrViolationErrors(fields.violation_time)) { return true; }
        if (this.getOcrViolationErrors(fields.violation_date)) { return true; }

        // seems like a goofy way to process these but this is how the JSON parse returns it
        // count 1
        if (this.getOcrViolationErrors(fields["counts.count_no_1.description"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_1.act_or_regulation_name_code"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_1.is_act"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_1.is_regulation"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_1.section"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_1.ticketed_amount"])) { return true; }

        // count 2
        if (this.getOcrViolationErrors(fields["counts.count_no_2.description"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_2.act_or_regulation_name_code"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_2.is_act"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_2.is_regulation"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_2.section"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_2.ticketed_amount"])) { return true; }

        // count 3
        if (this.getOcrViolationErrors(fields["counts.count_no_3.description"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_3.act_or_regulation_name_code"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_3.is_act"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_3.is_regulation"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_3.section"])) { return true; }
        if (this.getOcrViolationErrors(fields["counts.count_no_3.ticketed_amount"])) { return true; }

        if (this.getOcrViolationErrors(fields.court_location)) { return true; }
        if (this.getOcrViolationErrors(fields.detachment_location)) { return true; }
        return false;
      }
      else return null;
    } catch (e) {
      return null;
    }
  }

  // return number of validation errors
  getOcrViolationErrors(field?: Field): boolean {
    if (field == undefined || field == null) return false;
    if (field?.fieldConfidence != null && field.fieldConfidence < 0.8) {
      return true;
    } else return false;
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
