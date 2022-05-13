import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DisputesService, DisputeView, MoreDisputeStatus } from 'app/services/disputes.service';
import { DisputeStatus } from 'app/api/model/disputeStatus.model';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';

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
  newDispute: DisputeView = {
    DateSubmitted: undefined,
    ticketNumber: undefined,
    surname: undefined,
    givenNames: undefined,
    moreDisputeStatus: undefined,
    FilingDate: undefined,
    CourtHearing: undefined,
    disputantDetectedOcrIssues: undefined,
    systemDetectedOcrIssues: undefined,
    AssignedTs: undefined
  };
  displayedColumns: string[] = [
    'RedGreenAlert',
    'DateSubmitted',
    'ticketNumber',
    'surname',
    'givenNames',
    'moreDisputeStatus',
    'FilingDate',
    'CourtHearing',
    'disputantDetectedOcrIssues',
    'systemDetectedOcrIssues',
    'assignedTo',
  ];
  disputes: DisputeView[] = [];
  remoteDummyData: DisputeView[] = [
    {
      DateSubmitted: new Date('2022/02/08'),
      ticketNumber: 'AJ00214578',
      surname: 'McGibbons',
      givenNames: 'Julius Montgommery',
      moreDisputeStatus: MoreDisputeStatus.New,
      FilingDate: undefined,
      CourtHearing: true,
      disputantDetectedOcrIssues: true,
      systemDetectedOcrIssues: true,
      assignedTo: undefined,
    },
    {
      DateSubmitted: new Date('2022/02/08'),
      ticketNumber: 'EZ02000460',
      surname: 'Smithe',
      givenNames: 'Jaxon',
      moreDisputeStatus: MoreDisputeStatus.New,
      FilingDate: undefined,
      CourtHearing: false,
      disputantDetectedOcrIssues: false,
      systemDetectedOcrIssues: true,
      assignedTo: undefined,
    },
    {
      DateSubmitted: new Date('2022/02/08'),
      ticketNumber: 'AJ00214578',
      surname: 'Jacklin',
      givenNames: 'Susanne',
      moreDisputeStatus: MoreDisputeStatus.New,
      FilingDate: undefined,
      CourtHearing: false,
      disputantDetectedOcrIssues: false,
      systemDetectedOcrIssues: false,
      assignedTo: undefined,
    },
    {
      DateSubmitted: new Date('2022/02/08'),
      ticketNumber: 'AJ00214578',
      surname: 'Morris',
      givenNames: 'Mark',
      moreDisputeStatus: MoreDisputeStatus.New,
      FilingDate: undefined,
      CourtHearing: true,
      disputantDetectedOcrIssues: true,
      systemDetectedOcrIssues: false,
      assignedTo: undefined,
    },
    {
      DateSubmitted: new Date('2022/02/08'),
      ticketNumber: 'AJ00214578',
      surname: 'Korrin',
      givenNames: 'Karen',
      moreDisputeStatus: MoreDisputeStatus.New,
      FilingDate: undefined,
      CourtHearing: false,
      disputantDetectedOcrIssues: false,
      systemDetectedOcrIssues: false,
      assignedTo: undefined,
    },
    {
      DateSubmitted: new Date('2022/02/06'),
      ticketNumber: 'AJ00214578',
      surname: 'Aster',
      givenNames: 'Jack',
      moreDisputeStatus: MoreDisputeStatus.CheckedOut,
      FilingDate: undefined,
      CourtHearing: true,
      disputantDetectedOcrIssues: true,
      systemDetectedOcrIssues: false,
      assignedTo: 'Barry Mann',
    },
    {
      DateSubmitted: new Date('2022/02/06'),
      ticketNumber: 'AJ00214578',
      surname: 'Smith',
      givenNames: 'Portia',
      moreDisputeStatus: MoreDisputeStatus.Processing,
      FilingDate: new Date('2022/02/07'),
      CourtHearing: true,
      disputantDetectedOcrIssues: true,
      systemDetectedOcrIssues: false,
      assignedTo: undefined,
    },
    {
      DateSubmitted: new Date('2022/02/06'),
      ticketNumber: 'AJ00214578',
      surname: 'Brown',
      givenNames: 'Will',
      status: DisputeStatus.Cancelled,
      moreDisputeStatus: MoreDisputeStatus.Alert,
      FilingDate: new Date('2022/02/07'),
      CourtHearing: false,
      disputantDetectedOcrIssues: false,
      systemDetectedOcrIssues: false,
      assignedTo: 'Barry Mann',
    },
    {
      DateSubmitted: new Date('2022/02/05'),
      ticketNumber: 'AJ00214578',
      surname: 'Jones',
      givenNames: 'Sharron',
      moreDisputeStatus: MoreDisputeStatus.Processing,
      FilingDate: new Date('2022/02/06'),
      CourtHearing: true,
      disputantDetectedOcrIssues: false,
      systemDetectedOcrIssues: false,
      assignedTo: null,
    },
    {
      DateSubmitted: new Date('2022/02/04'),
      ticketNumber: 'AJ00214578',
      surname: 'Price',
      givenNames: 'Simone',
      moreDisputeStatus: MoreDisputeStatus.Processing,
      FilingDate: new Date('2022/02/06'),
      CourtHearing: false,
      disputantDetectedOcrIssues: false,
      systemDetectedOcrIssues: false,
      assignedTo: null,
    },
  ];

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
    return d.moreDisputeStatus == MoreDisputeStatus.New;
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
    this.remoteDummyData.forEach(d => {
      this.disputes = this.disputes.concat(d);
    });

    this.dataSource.data = this.disputes;

    // initially sort data by Date Submitted
    this.dataSource.data = this.dataSource.data.sort((a: DisputeView, b: DisputeView) => { if (a.DateSubmitted > b.DateSubmitted) { return -1; } else { return 1 } });
    
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
        this.newDispute.id = d.id
        this.newDispute.ticketNumber = d.ticketNumber;
        this.newDispute.assignedTo = d.assignedTo;
        this.newDispute.disputantDetectedOcrIssues = d.disputantDetectedOcrIssues;
        this.newDispute.surname = d.surname;
        this.newDispute.givenNames = d.givenNames;
        this.newDispute.jjAssigned = d.jjAssigned;

        // set court hearing to true if its true for any one of the three possible counts
        // otherwise false
        this.newDispute.CourtHearing = false;
        if (d.disputedCounts) d.disputedCounts.forEach(c => {
          if (c.appearInCourt == true) {
            this.newDispute.CourtHearing = true;
          }
        });

        this.newDispute.DateSubmitted = new Date(d.submittedDate);
        if(d.filingDate != null) {
          this.newDispute.FilingDate = new Date(d.filingDate);
        }
        this.newDispute.AssignedTs = new Date(d.assignedTs);

        switch (d.status) {
          case DisputeStatus.Cancelled:
            this.newDispute.moreDisputeStatus = MoreDisputeStatus.Cancelled;
            break;
          case DisputeStatus.Processing:
            this.newDispute.moreDisputeStatus = MoreDisputeStatus.Cancelled;
            break;
          case DisputeStatus.Rejected:
            this.newDispute.moreDisputeStatus = MoreDisputeStatus.Rejected;
            break;
          default:
            this.newDispute.moreDisputeStatus = MoreDisputeStatus.New;
            break;
        };
        this.newDispute.RedGreenAlert = this.newDispute.moreDisputeStatus == MoreDisputeStatus.New ? 'Green' : (this.newDispute.moreDisputeStatus == MoreDisputeStatus.Alert ? 'Red' : '');
        this.newDispute.provincialCourtHearingLocation = d.provincialCourtHearingLocation;
        this.disputes = this.disputes.concat(this.newDispute);
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

    if (objOcrViolationTicket.fields) {
      var fields = objOcrViolationTicket.fields;

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
    if (this.dataSource.data.filter((x: DisputeView) => x.moreDisputeStatus == MoreDisputeStatus.New))
      return this.dataSource.data.filter((x: DisputeView) => x.moreDisputeStatus == MoreDisputeStatus.New).length;
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
  }
  backTicketpage() {
    this.showTicket = !this.showTicket;
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