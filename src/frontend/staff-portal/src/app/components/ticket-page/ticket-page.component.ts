import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DisputesService, DisputeView } from 'app/services/disputes.service';
import { DisputeStatus } from 'app/api/model/disputeStatus.model';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { throwToolbarMixedModesError } from '@angular/material/toolbar';

@Component({
  selector: 'app-ticket-page',
  templateUrl: './ticket-page.component.html',
  styleUrls: ['./ticket-page.component.scss', '../../app.component.scss'],
})
export class TicketPageComponent implements OnInit, AfterViewInit {
  dataSource = new MatTableDataSource();
  public decidePopup = '';
  public disputeInfo: DisputeView;
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
  disputes: DisputeView[] = [];
  
  @ViewChild('tickTbSort') tickTbSort = new MatSort();
  public showTicket = false
  constructor(
    public disputesService: DisputesService,
    private logger: LoggerService,
  ) { }

  ngOnInit(): void {


    // when authentication token available, get data
    this.getAllDisputes();
  }

  isNew(d: DisputeView): boolean {
    return d.status == DisputeStatus.New;
  }

  getAllDisputes(): void {
    this.logger.log('TicketPageComponent::getAllDisputes');

    // for now this will show dummy data plus new records
    // FIXME: this should be the other way around - pull all Dispute records from staff-api and then maybe append mock data
    //    Currently, the dashboard data are not staff-api Dispute objects, but custom mock objects.  The mock objects should
    //    be instances of a Dispute (as defined by staff-api openapi spec).
    // this.disputesService.getDisputes().subscribe(next => this.dataSource.data = next);
    
    // concatenate all dummy data to this.disputes
    this.disputes = [];
    // this.remoteDummyData.forEach(d => {
    //   this.disputes = this.disputes.concat(d);
    // });

    this.dataSource.data = this.disputes;

    // initially sort data by Date Submitted
    this.dataSource.data = this.dataSource.data.sort((a: DisputeView, b: DisputeView) => { if (a.__DateSubmitted > b.__DateSubmitted) { return -1; } else { return 1 } });
    
    // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
    this.dataSource.filterPredicate = function (record: DisputeView, filter) {
      return record.ticketNumber.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
    }

    this.busy = this.disputesService.getDisputes().subscribe((response) => {
      this.logger.info(
        'TicketPageComponent::getAllDisputes response',
        response
      );

      this.disputesService.disputes$.next(response);
      response.forEach(d => {
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
      });
      this.dataSource.data = this.disputes;

      // initially sort data by Date Submitted
      this.dataSource.data = this.dataSource.data.sort((a: DisputeView, b: DisputeView) => { if (a.submittedDate > b.submittedDate) { return -1; } else { return 1 } });

      // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
      this.dataSource.filterPredicate = function (record: DisputeView, filter) {
        return record.ticketNumber.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
      }
    });
  }

  // data returns Notice of Dispute 
  // which has a property ViolationTicket
  // which has a property ocrViolationTicket
  // which is the JSON string for the Azure OCR'd version of a paper ticket
  // systemDetectedOcrIssues should be set to true if any OCR'd field has any validation errors returned
  // so this routine will exit with true at the first field of the fields collection that has an OCR error
  getSystemDetectedOcrIssues(ocrViolationTicket?: string): boolean {
    var objOcrViolationTicket = JSON.parse(ocrViolationTicket)

    let fields = objOcrViolationTicket?.fields;
    if (fields) {

      if (this.getOcrViolationErrors(fields.violationTicketTitle) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.ticket_number) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.surname) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.given_names) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.drivers_licence_province) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.drivers_licence_number) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.violation_time) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.violation_date) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.is_mva_offence) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.is_mca_offence) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.is_cta_offence) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.is_wla_offence) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.is_faa_offence) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.is_lca_offense) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.is_tcr_offence) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.is_other_offence) > 0) { return true; }
    
      // seems like a goofy way to process these but this is how the JSON parse returns it
      // count 1
      if (this.getOcrViolationErrors(fields["counts.count_1.description"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_1.act_or_regulation"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_1.is_act"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_1.is_regulation"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_1.section"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_1.ticketed_amount"]) > 0) { return true; }
  
      // count 2
      if (this.getOcrViolationErrors(fields["counts.count_2.description"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_2.act_or_regulation"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_2.is_act"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_2.is_regulation"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_2.section"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_2.ticketed_amount"]) > 0) { return true; }

      // count 3
      if (this.getOcrViolationErrors(fields["counts.count_3.description"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_3.act_or_regulation"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_3.is_act"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_3.is_regulation"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_3.section"]) > 0) { return true; }
      if (this.getOcrViolationErrors(fields["counts.count_3.ticketed_amount"]) > 0) { return true; }

      if (this.getOcrViolationErrors(fields.provincial_court_hearing_location) > 0) { return true; }
      if (this.getOcrViolationErrors(fields.organization_location) > 0) { return true; }
    }

    // step through fields in deserialized object and look for validation errors
    return false;
  }

  // return number of validation errors
  getOcrViolationErrors(field?: RecognizedField): number {
    if (field == undefined || field == null) return 0;
    if (field.validationErrors && field.validationErrors.length > 0) {
      return field.validationErrors.length;
    } else return 0;
  }

  countNewTickets(): number {
    if (this.dataSource.data.filter((x: DisputeView) => x.status == DisputeStatus.New))
      return this.dataSource.data.filter((x: DisputeView) => x.status == DisputeStatus.New).length;
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
  value?: any;
  fieldConfidence?: number;
  validationErrors?: string[];
  type?: string;
  boundingBoxes?: Point[];
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