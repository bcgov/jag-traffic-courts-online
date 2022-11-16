import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DisputeService, DisputeExtended } from 'app/services/dispute.service';
import { DisputeCountRequestCourtAppearance, DisputeDisputantDetectedOcrIssues, DisputeStatus, DisputeSystemDetectedOcrIssues } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { KeycloakProfile } from 'keycloak-js';
import { AuthService } from 'app/services/auth.service';

@Component({
  selector: 'app-ticket-inbox',
  templateUrl: './ticket-inbox.component.html',
  styleUrls: ['../../../app.component.scss', './ticket-inbox.component.scss'],
})
export class TicketInboxComponent implements OnInit, AfterViewInit {
  @Input() public IDIR: string;
  @Output() public disputeInfo: EventEmitter<DisputeExtended> = new EventEmitter();

  dataSource = new MatTableDataSource();
  public IDIRLogin: string = "";
  // public decidePopup = '';
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
    'disputantDetectedOcrIssues',
    'systemDetectedOcrIssues',
    'userAssignedTo',
  ];
  disputes: DisputeExtended[] = [];
  public userProfile: KeycloakProfile = {};
  public RequestCourtAppearance = DisputeCountRequestCourtAppearance;
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
        this.IDIRLogin = this.authService.userIDIRLogin;
      }
    })

    // when authentication token available, get data
    this.getAllDisputes();
  }

  isNew(d: DisputeExtended): boolean {
    return d.status == DisputeStatus.New && (d.emailAddressVerified === true || !d.emailAddress);
  }

  getAllDisputes(): void {
    this.logger.log('TicketInboxComponent::getAllDisputes');

    this.disputes = [];

    this.dataSource.data = this.disputes;

    // initially sort data by Date Submitted
    this.dataSource.data = this.dataSource.data.sort((a: DisputeExtended, b: DisputeExtended) => { if (a.__DateSubmitted > b.__DateSubmitted) { return -1; } else { return 1 } });

    // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
    this.dataSource.filterPredicate = function (record: DisputeExtended, filter) {
      return record.ticketNumber.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
    }

    this.busy = this.disputeService.getDisputes().subscribe((response) => {
      this.logger.info(
        'TicketInboxComponent::getAllDisputes response',
        response
      );

      response.forEach(d => {
        if (d.status != "CANCELLED") { // do not show cancelled
          var newDispute: DisputeExtended = {
            ticketNumber: d.ticketNumber,
            disputantSurname: d.disputantSurname,
            disputantGivenNames: d.disputantGivenNames,
            disputeId: d.disputeId,
            userAssignedTo: d.userAssignedTo,
            disputantDetectedOcrIssues: d.disputantDetectedOcrIssues,
            emailAddressVerified: d.emailAddressVerified,
            emailAddress: d.emailAddress,
            systemDetectedOcrIssues: this.getSystemDetectedOcrIssues(d.ocrViolationTicket),
            __CourtHearing: false,
            __DateSubmitted: new Date(d.submittedTs),
            __FilingDate: d.filingDate != null ? new Date(d.filingDate) : null,
            __UserAssignedTs: d.userAssignedTs != null ? new Date(d.userAssignedTs) : null,
            additionalProperties: d.additionalProperties,
            courtLocation: d.courtLocation,
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
      this.dataSource.data = this.dataSource.data.sort((a: DisputeExtended, b: DisputeExtended) => { if (a.submittedTs > b.submittedTs) { return -1; } else { return 1 } });

      // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
      this.dataSource.filterPredicate = function (record: DisputeExtended, filter) {
        return record.ticketNumber.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
      }
    });
  }

  // data returns Notice of Dispute
  // which has a property ViolationTicket
  // which has a property ocrViolationTicket
  // which is the JSON string for the Azure OCR'd version of a paper ticket
  // systemDetectedOcrIssues should be set to Y if any OCR'd field has less than 80% confidence
  // so this routine will exit with true at the first field of the fields collection that has an OCR error
  getSystemDetectedOcrIssues(ocrViolationTicket?: string): DisputeSystemDetectedOcrIssues {
    try {
      var objOcrViolationTicket = JSON.parse(ocrViolationTicket, );
      let fields = objOcrViolationTicket?.Fields;
      if (fields && fields !== undefined) {

        if (this.getOcrViolationErrors(fields.violationTicketTitle)) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields.ticket_number)) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields.disputant_surname)) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields.disputant_given_names)) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields.drivers_licence_province)) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields.drivers_licence_number)) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields.violation_time)) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields.violation_date)) { return this.SystemDetectedOcrIssues.Y; }

        // seems like a goofy way to process these but this is how the JSON parse returns it
        // count 1
        if (this.getOcrViolationErrors(fields["counts.count_no_1.description"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_1.act_or_regulation_name_code"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_1.is_act"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_1.is_regulation"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_1.section"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_1.ticketed_amount"])) { return this.SystemDetectedOcrIssues.Y; }

        // count 2
        if (this.getOcrViolationErrors(fields["counts.count_no_2.description"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_2.act_or_regulation_name_code"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_2.is_act"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_2.is_regulation"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_2.section"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_2.ticketed_amount"])) { return this.SystemDetectedOcrIssues.Y; }

        // count 3
        if (this.getOcrViolationErrors(fields["counts.count_no_3.description"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_3.act_or_regulation_name_code"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_3.is_act"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_3.is_regulation"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_3.section"])) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields["counts.count_no_3.ticketed_amount"])) { return this.SystemDetectedOcrIssues.Y; }

        if (this.getOcrViolationErrors(fields.court_location)) { return this.SystemDetectedOcrIssues.Y; }
        if (this.getOcrViolationErrors(fields.detachment_location)) { return this.SystemDetectedOcrIssues.Y; }
      }
    } catch (e) {
      return this.SystemDetectedOcrIssues.Y;
    }

    // step through fields in deserialized object and look for validation errors
    return this.SystemDetectedOcrIssues.N;
  }

  // return number of validation errors
  getOcrViolationErrors(field?: RecognizedField): DisputeSystemDetectedOcrIssues {
    if (field == undefined || field == null) return this.SystemDetectedOcrIssues.N;
    if (field.FieldConfidence && field.FieldConfidence < 0.8) {
      return this.SystemDetectedOcrIssues.Y;
    } else return this.SystemDetectedOcrIssues.N;
  }

  countNewTickets(): number {
    if (this.dataSource.data.filter((x: DisputeExtended) => x.status == DisputeStatus.New))
      return this.dataSource.data.filter((x: DisputeExtended) => x.status == DisputeStatus.New).length;
    else return 0;
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
export interface RecognizedField {
  Value?: any;
  FieldConfidence?: number;
  ValidationErrors?: string[];
  Type?: string;
  BoundingBoxes?: Point[];
}

export interface Point {
  x?: number;
  y?: number;
}

export interface OcrCount {
  description?: RecognizedField;
  act_or_regulation_name_code?: RecognizedField;
  is_act?: RecognizedField;
  is_regulation?: RecognizedField;
  section?: RecognizedField;
  ticketed_amount?: RecognizedField;
}
