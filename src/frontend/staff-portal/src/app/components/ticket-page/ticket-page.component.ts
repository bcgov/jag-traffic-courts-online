import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DisputeService, Dispute } from 'app/services/dispute.service';
import { DisputeStatus } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { OidcSecurityService } from 'angular-auth-oidc-client';
@Component({
  selector: 'app-ticket-page',
  templateUrl: './ticket-page.component.html',
  styleUrls: ['./ticket-page.component.scss', '../../app.component.scss'],
})
export class TicketPageComponent implements OnInit, AfterViewInit {
  dataSource = new MatTableDataSource();
  public IDIRLogin: string = "";
  public decidePopup = '';
  public disputeInfo: Dispute;
  busy: Subscription;
  displayedColumns: string[] = [
    '__RedGreenAlert',
    '__DateSubmitted',
    'ticketNumber',
    'surname',
    'givenNames',
    'status',
    '__FilingDate',
    '__CourtHearing',
    'disputantDetectedOcrIssues',
    'systemDetectedOcrIssues',
    'assignedTo',
  ];
  disputes: Dispute[] = [];

  @ViewChild('tickTbSort') tickTbSort = new MatSort();
  public showTicket = false
  constructor(
    public disputeService: DisputeService,
    private logger: LoggerService,
    private oidcSecurityService: OidcSecurityService
  ) { 
    this.oidcSecurityService.checkAuth().subscribe(({ isAuthenticated }) => {
      this.IDIRLogin = this.oidcSecurityService.getUserData()?.preferred_username?.split("@")[0]; // split at @ sign and take first part
    });
  }

  ngOnInit(): void {


    // when authentication token available, get data
    this.getAllDisputes();
  }

  isNew(d: Dispute): boolean {
    return d.status == DisputeStatus.New;
  }

  getAllDisputes(): void {
    this.logger.log('TicketPageComponent::getAllDisputes');

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
        'TicketPageComponent::getAllDisputes response',
        response
      );

      response.forEach(d => {
        if (d.status != "CANCELLED") { // do not show cancelled
          var newDispute = {
            ticketNumber: d.ticketNumber,
            surname: d.surname,
            givenNames: d.givenNames,
            jjAssigned: d.jjAssigned,
            id: d.id,
            assignedTo: d.assignedTo,
            disputantDetectedOcrIssues: d.disputantDetectedOcrIssues,
            systemDetectedOcrIssues: this.getSystemDetectedOcrIssues(d.ocrViolationTicket),
            __CourtHearing: false,
            __DateSubmitted: new Date(d.submittedDate),
            __FilingDate: d.filingDate != null ? new Date(d.filingDate) : null,
            __AssignedTs: d.assignedTs != null ? new Date(d.assignedTs) : null,
            additionalProperties: d.additionalProperties,
            provincialCourtHearingLocation: d.provincialCourtHearingLocation,
            status: d.status,
            __RedGreenAlert: d.status == DisputeStatus.New ? 'Green' : '',
            assignedTs: d.assignedTs
          }

          // set court hearing to true if its true for any one of the three possible counts
          // otherwise false
          if (d.disputedCounts) d.disputedCounts.forEach(c => {
            if (c.appearInCourt == true) {
              newDispute.__CourtHearing = true;
            }
          });

          this.disputes = this.disputes.concat(newDispute);
        }
      });
      this.dataSource.data = this.disputes;

      // initially sort data by Date Submitted
      this.dataSource.data = this.dataSource.data.sort((a: Dispute, b: Dispute) => { if (a.submittedDate > b.submittedDate) { return -1; } else { return 1 } });

      // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
      this.dataSource.filterPredicate = function (record: Dispute, filter) {
        return record.ticketNumber.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
      }
    });
  }

  // data returns Notice of Dispute 
  // which has a property ViolationTicket
  // which has a property ocrViolationTicket
  // which is the JSON string for the Azure OCR'd version of a paper ticket
  // systemDetectedOcrIssues should be set to true if any OCR'd field has less than 80% confidence
  // so this routine will exit with true at the first field of the fields collection that has an OCR error
  getSystemDetectedOcrIssues(ocrViolationTicket?: string): boolean {
    var objOcrViolationTicket = JSON.parse(ocrViolationTicket)

    let fields = objOcrViolationTicket?.Fields;
    if (fields) {

      if (this.getOcrViolationErrors(fields.violationTicketTitle)) { return true; }
      if (this.getOcrViolationErrors(fields.ticket_number)) { return true; }
      if (this.getOcrViolationErrors(fields.surname)) { return true; }
      if (this.getOcrViolationErrors(fields.given_names)) { return true; }
      if (this.getOcrViolationErrors(fields.drivers_licence_province)) { return true; }
      if (this.getOcrViolationErrors(fields.drivers_licence_number)) { return true; }
      if (this.getOcrViolationErrors(fields.violation_time)) { return true; }
      if (this.getOcrViolationErrors(fields.violation_date)) { return true; }
      if (this.getOcrViolationErrors(fields.is_mva_offence)) { return true; }
      if (this.getOcrViolationErrors(fields.is_mca_offence)) { return true; }
      if (this.getOcrViolationErrors(fields.is_cta_offence)) { return true; }
      if (this.getOcrViolationErrors(fields.is_wla_offence)) { return true; }
      if (this.getOcrViolationErrors(fields.is_faa_offence)) { return true; }
      if (this.getOcrViolationErrors(fields.is_lca_offense)) { return true; }
      if (this.getOcrViolationErrors(fields.is_tcr_offence)) { return true; }
      if (this.getOcrViolationErrors(fields.is_other_offence)) { return true; }

      // seems like a goofy way to process these but this is how the JSON parse returns it
      // count 1
      if (this.getOcrViolationErrors(fields["counts.count_1.description"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_1.act_or_regulation"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_1.is_act"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_1.is_regulation"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_1.section"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_1.ticketed_amount"])) { return true; }

      // count 2
      if (this.getOcrViolationErrors(fields["counts.count_2.description"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_2.act_or_regulation"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_2.is_act"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_2.is_regulation"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_2.section"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_2.ticketed_amount"])) { return true; }

      // count 3
      if (this.getOcrViolationErrors(fields["counts.count_3.description"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_3.act_or_regulation"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_3.is_act"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_3.is_regulation"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_3.section"])) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_3.ticketed_amount"])) { return true; }

      if (this.getOcrViolationErrors(fields.provincial_court_hearing_location)) { return true; }
      if (this.getOcrViolationErrors(fields.organization_location)) { return true; }
    }

    // step through fields in deserialized object and look for validation errors
    return false;
  }

  // return number of validation errors
  getOcrViolationErrors(field?: RecognizedField): boolean {
    if (field == undefined || field == null) return false;
    if (field.FieldConfidence && field.FieldConfidence < 0.8) {
      return true;
    } else return false;
  }

  countNewTickets(): number {
    if (this.dataSource.data.filter((x: Dispute) => x.status == DisputeStatus.New))
      return this.dataSource.data.filter((x: Dispute) => x.status == DisputeStatus.New).length;
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

  backTicketList(element) {
    this.disputeInfo = element;
    if (element.ticketNumber[0] == 'A') {
      this.decidePopup = 'E'
    } else {
      this.decidePopup = "A"
    }
    this.showTicket = !this.showTicket;
    if (!this.showTicket) this.getAllDisputes();  // refresh list
  }

  backTicketpage() {
    this.showTicket = !this.showTicket;
    if (!this.showTicket) this.getAllDisputes(); // refresh list
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
  act_or_regulation?: RecognizedField;
  is_act?: RecognizedField;
  is_regulation?: RecognizedField;
  section?: RecognizedField;
  ticketed_amount?: RecognizedField;
}