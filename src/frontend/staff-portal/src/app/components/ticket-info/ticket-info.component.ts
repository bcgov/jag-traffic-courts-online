import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { DatePipe } from '@angular/common';
import { DisputeView, DisputesService } from '../../services/disputes.service';
import { Subscription } from 'rxjs';
import { ProvinceConfig, Config } from '@config/config.model';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { Dispute, DisputedCountPlea, ViolationTicket, ViolationTicketCount } from 'app/api';
import { LookupsService, StatuteView } from 'app/services/lookups.service';
import { DateRange } from '@angular/material/datepicker';
@Component({
  selector: 'app-ticket-info',
  templateUrl: './ticket-info.component.html',
  styleUrls: ['./ticket-info.component.scss'],
})
export class TicketInfoComponent implements OnInit {
  @Input() public disputeInfo: DisputeView;
  @Output() public backTicketList: EventEmitter<MatStepper> = new EventEmitter();
  public isMobile: boolean;
  public previousButtonIcon = 'keyboard_arrow_left';
  public previousButtonKey = 'stepper.backReview';
  public saveButtonKey = 'stepper.next';
  public busy: Subscription;
  public lastUpdatedDispute: Dispute;
  public form: FormGroup;
  public flagsForm: FormGroup;
  public tempViolationTicketCount: ViolationTicketCount;
  public todayDate: Date = new Date();
  public provinces: ProvinceConfig[];
  public states: ProvinceConfig[];
  public initialDisputeValues: Dispute;
  public courtLocations: Config<string>[];
  public imageToShow: any;

  /**
   * @description
   * Whether to show the address line fields.
   */
  public filteredCount1Statutes: StatuteView[];
  public filteredCount2Statutes: StatuteView[];
  public filteredCount3Statutes: StatuteView[];
  public collapseObj: any = {
    ticketInformation: true,
    contactInformation: true,
    imageInformation: true
  }
  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    private utilsService: UtilsService,
    private datePipe: DatePipe,
    private logger: LoggerService,
    private disputesService: DisputesService,
    public mockConfigService: MockConfigService,
    public lookupsService: LookupsService
  ) {
    const today = new Date();
    this.isMobile = this.utilsService.isMobile();
    if (this.mockConfigService.provinces) {
      this.provinces = this.mockConfigService.provinces.filter(x => x.countryCode == 'CA');
      this.states = this.mockConfigService.provinces.filter(x => x.countryCode == 'US');
    }
    if (this.mockConfigService.courtLocations) {
      this.courtLocations = this.mockConfigService.courtLocations.sort((a, b) => { if (a.name < b.name) return 1; });
    }

    this.busy = this.lookupsService.getStatutes().subscribe((response: StatuteView[]) => {
      this.lookupsService.statutes$.next(response);
    });
  }

  public ngOnInit() {
    this.flagsForm = this.formBuilder.group({
      disputantOcrIssuesDescription: [null],
      disputantDetectedOcrIssues: [null],
    });
    this.form = this.formBuilder.group({
      ticketNumber: [null, [Validators.required]],
      homePhoneNumber: [null, Validators.required],
      emailAddress: [null, [Validators.email, Validators.required]],
      surname: [null, [Validators.required]],
      givenNames: [null, [Validators.required]],
      country: ["Canada", [Validators.required]], // hard coded this is not returned from API, assumed always Canada
      birthdate: [null, [Validators.required]], // Optional
      address: [null, [Validators.required]],
      city: [null, [Validators.required]],
      province: [null, [Validators.required]],
      postalCode: [null, [Validators.required]], // space needs to be added back to the middle for display
      driversLicenceNumber: [null, [Validators.required, Validators.minLength(7), Validators.maxLength(9), Validators.pattern(/^(\d{7}|\d{8}|\d{9})$/)]],
      driversLicenceProvince: [null, [Validators.required]],
      provincialCourtHearingLocation: [null, [Validators.required]],
      _chargeCount: [1],
      _amountOwing: [null],
      violationTicket: this.formBuilder.group({
        ticketNumber: [null, Validators.required],
        provincialCourtHearingLocation: [null, [Validators.required]],
        surname: [null, Validators.required],
        givenNames: [null, Validators.required],
        driversLicenceNumber: [null, [Validators.required, Validators.minLength(7), Validators.maxLength(9), Validators.pattern(/^(\d{7}|\d{8}|\d{9})$/)]],
        driversLicenceProvince: [null, Validators.required],
        issuedDate: [null, Validators.required],
        violationTicketCount1: this.formBuilder.group({
          description: [null],
          actRegulation: [null],
          fullSection: [null],
          section: [null],
          subsection: [null],
          paragraph: [null],
          fullDescription: [null],
          ticketedAmount: [null, [FormControlValidators.currency]]
        }),
        violationTicketCount2: this.formBuilder.group({
          description: [null],
          actRegulation: [null],
          fullSection: [null],
          section: [null],
          subsection: [null],
          paragraph: [null],
          fullDescription: [null],
          ticketedAmount: [null, [FormControlValidators.currency]]
        }),
        violationTicketCount3: this.formBuilder.group({
          description: [null],
          actRegulation: [null],
          fullSection: [null],
          section: [null],
          subsection: [null],
          paragraph: [null],
          fullDescription: [null],
          ticketedAmount: [null, [FormControlValidators.currency]]
        }),
        violationDate: [null, [Validators.required]],  // api returns issued date, extract date from that
        violationTime: [null, [Validators.required, Validators.pattern(/^(0[0-9]|1[0-9]|2[0-3])[0-5][0-9]$/)]],  // api returns issued date, extract time from that  
      })
    });
    // retreive fresh copy from db
    this.getDispute();
  }

  onFullDescription1Keyup() {
    this.filteredCount1Statutes = this.filterStatutes(this.form.get('violationTicket').get('violationTicketCount1').get('fullDescription').value);
  }

  onFullDescription2Keyup() {
    this.filteredCount2Statutes = this.filterStatutes(this.form.get('violationTicket').get('violationTicketCount1').get('fullDescription').value);
  }

  onFullDescription3Keyup() {
    this.filteredCount3Statutes = this.filterStatutes(this.form.get('violationTicket').get('violationTicketCount1').get('fullDescription').value);
  }

  // return a filtered list of statutes
  public filterStatutes(val: string): StatuteView[] {
    if (!this.lookupsService.statutes || this.lookupsService.statutes.length == 0) return [];
    return this.lookupsService.statutes.filter(option => option.statuteString.indexOf(val) >= 0);
  }

  public onBack() {
    this.backTicketList.emit();
  }

  // change validators on drivers licence number in violation ticket when changing province / state
  public onViolationTicketDLProvinceChange(province: string) {
    if (province == 'British Columbia') {
      this.form.get('violationTicket').get('driversLicenceNumber').setValidators([Validators.required, Validators.minLength(7), Validators.maxLength(9), Validators.pattern(/^(\d{7}|\d{8}|\d{9})$/)]);
      this.form.get('violationTicket').get('driversLicenceNumber').updateValueAndValidity();
      this.form.updateValueAndValidity();

    } else {
      this.form.get('violationTicket').get('driversLicenceNumber').setValidators(Validators.required);
      this.form.get('violationTicket').get('driversLicenceNumber').updateValueAndValidity();
      this.form.updateValueAndValidity();
    }
  }

  // change validators on drivers licence number in notice of dispute when changing province / state
  public onNoticeOfDisputeDLProvinceChange(province: string) {
    if (province == 'British Columbia') {
      this.form.get('driversLicenceNumber').setValidators([Validators.required, Validators.minLength(7), Validators.maxLength(9), Validators.pattern(/^(\d{7}|\d{8}|\d{9})$/)]);
      this.form.get('driversLicenceNumber').updateValueAndValidity();
      this.form.updateValueAndValidity();

    } else {
      this.form.get('driversLicenceNumber').setValidators(Validators.required);
      this.form.get('driversLicenceNumber').updateValueAndValidity();
      this.form.updateValueAndValidity();
    }
  }
  onKeyPressNumbers(event: any, BCOnly: boolean) {
    var charCode = (event.which) ? event.which : event.keyCode;
    // Only Numbers 0-9
    if ((charCode < 48 || charCode > 57) && BCOnly) {
      event.preventDefault();
      return false;
    } else {
      return true;
    }
  }

  public onSubmitViolationTicket(): void {

    // We are only sending the violation Ticket fields so update a local copy of lastUpdatedDispute
    // with violation Ticket form fields only that were changed
    let putDispute = this.lastUpdatedDispute;

    putDispute.violationTicket.ticketNumber = this.form.get('violationTicket').get('ticketNumber').value;
    putDispute.violationTicket.surname = this.form.get('violationTicket').get('surname').value;
    putDispute.violationTicket.givenNames = this.form.get('violationTicket').get('givenNames').value;
    putDispute.violationTicket.driversLicenceNumber = this.form.get('violationTicket').get('driversLicenceNumber').value;
    putDispute.violationTicket.driversLicenceProvince = this.form.get('violationTicket').get('driversLicenceProvince').value;

    // provincial court hearing is returned by the GET at the level of notice of dispute but is to be saved with
    // violation Ticket fields not notice of dispute fields
    putDispute.provincialCourtHearingLocation = this.form.get('violationTicket').get('provincialCourtHearingLocation').value;

    // reconstruct issued date as string from violation date and violation time format yyyy-mm-ddThh:mm
    putDispute.violationTicket.issuedDate =
      this.form.get('violationTicket').get('violationDate').value +
      "T" +
      this.form.get('violationTicket').get('violationTime').value.substring(0, 2)
      + ":" +
      this.form.get('violationTicket').get('violationTime').value.substring(2, 4);

    // Loop through violation Ticket counts and set fields
    putDispute.violationTicket.violationTicketCounts.forEach(violationTicketCount => {
      if (violationTicketCount.count == 1) {
        violationTicketCount = this.constructViolationTicketCount(this.form.get('violationTicket').get('violationTicketCount1').get('fullDescription').value);
        violationTicketCount.ticketedAmount = this.form.get('violationTicket').get('violationTicketCount1').get('ticketedAmount').value;
      } else if (violationTicketCount.count == 2) {
        violationTicketCount = this.constructViolationTicketCount(this.form.get('violationTicket').get('violationTicketCount2').get('fullDescription').value);
        violationTicketCount.ticketedAmount = this.form.get('violationTicket').get('violationTicketCount2').get('ticketedAmount').value;
      } else if (violationTicketCount.count == 3) {
        violationTicketCount = this.constructViolationTicketCount(this.form.get('violationTicket').get('violationTicketCount3').get('fullDescription').value);
        violationTicketCount.ticketedAmount = this.form.get('violationTicket').get('violationTicketCount3').get('ticketedAmount').value;
      }
    });

    this.putDispute(putDispute);
  }

  public onSubmitNoticeOfDispute(): void {
    // We are only sending the notice of dispute fields so update a local copy of lastUpdatedDispute
    // with notice of dispute form fields only that were changed
    let putDispute = this.lastUpdatedDispute;

    putDispute.surname = this.form.get('surname').value;
    putDispute.givenNames = this.form.get('givenNames').value;
    putDispute.driversLicenceNumber = this.form.get('driversLicenceNumber').value;
    putDispute.driversLicenceProvince = this.form.get('driversLicenceProvince').value;
    putDispute.homePhoneNumber = this.form.get('homePhoneNumber').value;
    putDispute.emailAddress = this.form.get('emailAddress').value;
    putDispute.birthdate = this.form.get('birthdate').value;
    putDispute.address = this.form.get('address').value;
    putDispute.city = this.form.get('city').value;
    putDispute.province = this.form.get('province').value;
    putDispute.postalCode = this.form.get('postalCode').value;

    this.putDispute(putDispute);
  }

  // decompose string into fullsection, section, subsection, paragraph
  public unLegalParagraph(statuteLegalParagraphing: string): { fullSection: string, section: string, subsection: string, paragraph: string } {
    let allParts = statuteLegalParagraphing.split("(");
    let fullSection = "";
    let section = "";
    let subsection = "";
    let paragraph = "";

    // parts are fullSection(section)(subsection)(paragraph) if all are present
    // extract substrings but dont include final ')' of each part
    if (allParts.length > 0) fullSection = allParts[0].substring(0, allParts[0].length);
    if (allParts.length > 1) section = allParts[1].substring(0, allParts[1].length - 1);
    if (allParts.length > 2) subsection = allParts[2].substring(0, allParts[2].length - 1);
    if (allParts.length > 3) paragraph = allParts[3].substring(0, allParts[3].length - 1);

    return { fullSection: fullSection, section: section, subsection: subsection, paragraph: paragraph };
  }

  public constructViolationTicketCount(statuteString: string): ViolationTicketCount {

    this.tempViolationTicketCount = { description: "", actRegulation: "", fullSection: "", section: "", subsection: "", paragraph: "" };

    // look in list of statutes
    let statute = this.lookupsService.statutes.filter(x => x.statuteString == statuteString) as StatuteView[];
    if (statute && statute.length > 0) {
      this.tempViolationTicketCount.description = statute[0].description;
      this.tempViolationTicketCount.actRegulation = statute[0].act; // to do break down legal paragraphing
      let parts = this.unLegalParagraph(statute[0].section);
      this.tempViolationTicketCount.fullSection = parts.fullSection;
      this.tempViolationTicketCount.section = parts.section;
      this.tempViolationTicketCount.subsection = parts.subsection;
      this.tempViolationTicketCount.paragraph = parts.paragraph;
      return this.tempViolationTicketCount;
    }

    // if not found in list of statutes, make an attempt from passed in string
    // first portion before space is act or regulation
    // second portion before next space is legal paragraphing
    // remaining is desciption
    const statuteStringParts = statuteString.split(" ");

    // three parts yay
    if (statuteStringParts.length > 2) {
      this.tempViolationTicketCount.actRegulation = statuteStringParts[0];
      let parts = this.unLegalParagraph(statuteStringParts[1]);
      this.tempViolationTicketCount.fullSection = parts.fullSection;
      this.tempViolationTicketCount.section = parts.section;
      this.tempViolationTicketCount.subsection = parts.subsection;
      this.tempViolationTicketCount.paragraph = parts.paragraph;
      this.tempViolationTicketCount.description = statuteString.substring(
        statuteString.indexOf(statuteStringParts[2])
      );
    }

    // two parts hmm....
    else if (statuteStringParts.length > 1) {
      this.tempViolationTicketCount.actRegulation = "";
      this.tempViolationTicketCount.fullSection = statuteStringParts[0];
      this.tempViolationTicketCount.description = statuteString.substring(statuteString.indexOf(statuteStringParts[1]));

      // yuk shove it in description field
    } else {
      this.tempViolationTicketCount.actRegulation = "";
      this.tempViolationTicketCount.fullSection = "";
      this.tempViolationTicketCount.description = statuteString;
    }

    return this.tempViolationTicketCount;
  }

  public enableNoticeOfDisputeSave(): boolean {

    // check for fields invalid in contact information only
    if (this.form.get('emailAddress').invalid) return false;
    if (this.form.get('homePhoneNumber').invalid) return false;
    if (this.form.get('surname').invalid) return false;
    if (this.form.get('givenNames').invalid) return false;
    if (this.form.get('country').invalid) return false;
    if (this.form.get('birthdate').invalid) return false;
    if (this.form.get('address').invalid) return false;
    if (this.form.get('city').invalid) return false;
    if (this.form.get('province').invalid) return false;
    if (this.form.get('postalCode').invalid) return false;
    if (this.form.get('driversLicenceNumber').invalid) return false;
    if (this.form.get('driversLicenceProvince').invalid) return false;

    // check for touched fields
    if (this.form.get('emailAddress').touched) return true;
    if (this.form.get('homePhoneNumber').touched) return true;
    if (this.form.get('surname').touched) return true;
    if (this.form.get('givenNames').touched) return true;
    if (this.form.get('country').touched) return true;
    if (this.form.get('birthdate').touched) return true;
    if (this.form.get('address').touched) return true;
    if (this.form.get('city').touched) return true;
    if (this.form.get('province').touched) return true;
    if (this.form.get('postalCode').touched) return true;
    if (this.form.get('driversLicenceNumber').touched) return true;
    if (this.form.get('driversLicenceProvince').touched) return true;

    // no contact information touched, all valid
    return false;
  }

  public handleCollapse(name: string) {
    this.collapseObj[name] = !this.collapseObj[name]
  }

  // act fullsection(section)(subsection)(paragraph)
  public getLegalParagraphing(violationTicketCount: ViolationTicketCount): string {
    if (!violationTicketCount || !violationTicketCount.description) return "";
    let ticketDesc = (violationTicketCount.actRegulation ? violationTicketCount.actRegulation : "") + " ";
    if (violationTicketCount.section && violationTicketCount.section.length > 0) ticketDesc = ticketDesc + violationTicketCount.section;
    if (violationTicketCount.subsection && violationTicketCount.subsection.length > 0) ticketDesc = ticketDesc + "(" + violationTicketCount.subsection + ")";
    if (violationTicketCount.paragraph && violationTicketCount.paragraph.length > 0) ticketDesc = ticketDesc + "(" + violationTicketCount.paragraph + ")";
    ticketDesc = ticketDesc + " " + violationTicketCount.description;
    return ticketDesc;
  }

  // get legal paragraphing for a particular count
  public getCountLegalParagraphing(countNumber: number, violationTicket: ViolationTicket): string {
    if (violationTicket.violationTicketCounts.filter(x => x.count == countNumber)) return this.getLegalParagraphing(violationTicket.violationTicketCounts.filter(x => x.count == countNumber)[0]);
    else return "";
  }

  // put dispute by id
  putDispute(dispute: Dispute): void {
    this.logger.log('TicketInfoComponent::putDispute', dispute);

    // no need to pass back byte array with image
    let tempDispute = dispute;
    tempDispute.violationTicket.violationTicketImage = null;

    this.busy = this.disputesService.putDispute(dispute.id, tempDispute).subscribe((response: Dispute) => {
      this.logger.info(
        'TicketInfoComponent::putDispute response',
        response
      );

      // this structure contains last version of what was send to db
      this.lastUpdatedDispute = response;
    });

  }

  setViolationTicketFieldsFromJSON(dispute: Dispute): Dispute {
    var objOcrViolationTicket = JSON.parse(dispute.ocrViolationTicket);

    if (objOcrViolationTicket && objOcrViolationTicket.Fields) {
      var fields = objOcrViolationTicket.Fields;

      if (!dispute.violationTicket.ticketNumber) dispute.violationTicket.ticketNumber = fields.ticket_number.Value;
      if (!dispute.violationTicket.surname) dispute.violationTicket.surname = fields.surname.Value;
      if (!dispute.violationTicket.givenNames) dispute.violationTicket.givenNames = fields.given_names.Value;
      if (!dispute.violationTicket.driversLicenceProvince) dispute.violationTicket.driversLicenceProvince = fields.drivers_licence_province.Value;
      if (!dispute.violationTicket.driversLicenceNumber) dispute.violationTicket.driversLicenceNumber = fields.drivers_licence_number.Value;
      if (!dispute.violationTicket.isMvaOffence) dispute.violationTicket.isMvaOffence = fields.is_mva_offence.Value == "selected" ? true : false;
      if (!dispute.violationTicket.isLcaOffence) dispute.violationTicket.isLcaOffence = fields.is_lca_offence.Value == "selected" ? true : false;;
      if (!dispute.violationTicket.isMcaOffence) dispute.violationTicket.isMcaOffence = fields.is_mca_offence.Value == "selected" ? true : false;;
      if (!dispute.violationTicket.isFaaOffence) dispute.violationTicket.isFaaOffence = fields.is_faa_offence.Value == "selected" ? true : false;;
      if (!dispute.violationTicket.isTcrOffence) dispute.violationTicket.isTcrOffence = fields.is_tcr_offence.Value == "selected" ? true : false;;
      if (!dispute.violationTicket.isCtaOffence) dispute.violationTicket.isCtaOffence = fields.is_cta_offence.Value == "selected" ? true : false;;
      if (!dispute.violationTicket.isWlaOffence) dispute.violationTicket.isWlaOffence = fields.is_wla_offence.Value == "selected" ? true : false;;
      if (!dispute.violationTicket.isOtherOffence) dispute.violationTicket.isOtherOffence = fields.is_other_offence.Value == "selected" ? true : false;;
      if (!dispute.violationTicket.organizationLocation) dispute.violationTicket.organizationLocation = fields.organization_location.Value;
      if (!dispute.provincialCourtHearingLocation) dispute.provincialCourtHearingLocation = fields.provincial_court_hearing_location.Value;

      // set up ticket count 1
      if (fields["counts.count_1.description"]) {
        const foundViolationTicketCount1 = dispute.violationTicket.violationTicketCounts.filter(x => x.count == 1);
        if (foundViolationTicketCount1.length > 0) {
          if (!foundViolationTicketCount1[0].description) foundViolationTicketCount1[0].description = fields["counts.count_1.description"].Value;
          if (!foundViolationTicketCount1[0].actRegulation) foundViolationTicketCount1[0].actRegulation = fields["counts.count_1.act_or_regulation"].Value;
          if (!foundViolationTicketCount1[0].section) foundViolationTicketCount1[0].section = fields["counts.count_1.section"].Value;
          if (!foundViolationTicketCount1[0].ticketedAmount) foundViolationTicketCount1[0].ticketedAmount = fields["counts.count_1.ticketed_amount"].Value?.substring(1);
          if (!foundViolationTicketCount1[0].isAct) foundViolationTicketCount1[0].isAct = fields["counts.count_1.is_act"].Value == "selected" ? true : false;
          if (!foundViolationTicketCount1[0].isRegulation) foundViolationTicketCount1[0].isRegulation = fields["counts.count_1.is_regulation"].Value == "selected" ? true : false;
        } else {
          let violationTicketCount = {
            count: 1,
            description: fields["counts.count_1.description"].Value,
            actRegulation: fields["counts.count_1.act_or_regulation"].Value,
            section: fields["counts.count_1.section"].Value,
            ticketedAmount: fields["counts.count_1.ticketed_amount"].Value?.substring(1),
            isAct: fields["counts.count_1.is_act"].Value == "selected" ? true : false,
            isRegulation: fields["counts.count_1.is_regulation"].Value == "selected" ? true : false
          } as ViolationTicketCount;
          dispute.violationTicket.violationTicketCounts = dispute.violationTicket.violationTicketCounts.concat(violationTicketCount);
        }
      }

      // // set up ticket count 2
      if (fields["counts.count_2.description"]) {
        const foundViolationTicketCount2 = dispute.violationTicket.violationTicketCounts.filter(x => x.count == 2);
        if (foundViolationTicketCount2.length > 0) {
          if (!foundViolationTicketCount2[0].description) foundViolationTicketCount2[0].description = fields["counts.count_2.description"].Value;
          if (!foundViolationTicketCount2[0].actRegulation) foundViolationTicketCount2[0].actRegulation = fields["counts.count_2.act_or_regulation"].Value;
          if (!foundViolationTicketCount2[0].section) foundViolationTicketCount2[0].section = fields["counts.count_2.section"].Value;
          if (!foundViolationTicketCount2[0].ticketedAmount) foundViolationTicketCount2[0].ticketedAmount = fields["counts.count_2.ticketed_amount"].Value?.substring(1);
          if (!foundViolationTicketCount2[0].isAct) foundViolationTicketCount2[0].isAct = fields["counts.count_2.is_act"].Value == "selected" ? true : false;
          if (!foundViolationTicketCount2[0].isRegulation) foundViolationTicketCount2[0].isRegulation = fields["counts.count_2.is_regulation"].Value == "selected" ? true : false;
        } else {
          let violationTicketCount = {
            count: 2,
            description: fields["counts.count_2.description"].Value,
            actRegulation: fields["counts.count_2.act_or_regulation"].Value,
            section: fields["counts.count_2.section"].Value,
            ticketedAmount: fields["counts.count_2.ticketed_amount"].Value?.substring(1),
            isAct: fields["counts.count_2.is_act"].Value == "selected" ? true : false,
            isRegulation: fields["counts.count_2.is_regulation"].Value == "selected" ? true : false
          } as ViolationTicketCount;
          dispute.violationTicket.violationTicketCounts = dispute.violationTicket.violationTicketCounts.concat(violationTicketCount);
        }
      }

      // // set up ticket count 3
      if (fields["counts.count_1.description"]) {
        const foundViolationTicketCount3 = dispute.violationTicket.violationTicketCounts.filter(x => x.count == 3);
        if (foundViolationTicketCount3.length > 0) {
          if (!foundViolationTicketCount3[0].description) foundViolationTicketCount3[0].description = fields["counts.count_3.description"].Value;
          if (!foundViolationTicketCount3[0].actRegulation) foundViolationTicketCount3[0].actRegulation = fields["counts.count_3.act_or_regulation"].Value;
          if (!foundViolationTicketCount3[0].section) foundViolationTicketCount3[0].section = fields["counts.count_3.section"].Value;
          if (!foundViolationTicketCount3[0].ticketedAmount) foundViolationTicketCount3[0].ticketedAmount = fields["counts.count_3.ticketed_amount"].Value?.substring(1);
          if (!foundViolationTicketCount3[0].isAct) foundViolationTicketCount3[0].isAct = fields["counts.count_3.is_act"].Value == "selected" ? true : false;
          if (!foundViolationTicketCount3[0].isRegulation) foundViolationTicketCount3[0].isRegulation = fields["counts.count_3.is_regulation"].Value == "selected" ? true : false;
        } else {
          let violationTicketCount = {
            count: 3,
            description: fields["counts.count_3.description"].Value,
            actRegulation: fields["counts.count_3.act_or_regulation"].Value,
            section: fields["counts.count_3.section"].Value,
            ticketedAmount: fields["counts.count_3.ticketed_amount"].Value?.substring(1),
            isAct: fields["counts.count_3.is_act"].Value == "selected" ? true : false,
            isRegulation: fields["counts.count_3.is_regulation"].Value == "selected" ? true : false
          } as ViolationTicketCount;
          dispute.violationTicket.violationTicketCounts = dispute.violationTicket.violationTicketCounts.concat(violationTicketCount);
        }
      }
    }

    return dispute;
  }

  // get dispute by id
  getDispute(): void {
    this.logger.log('TicketInfoComponent::getDispute');

    this.busy = this.disputesService.getDispute(this.disputeInfo.id).subscribe((response: Dispute) => {
      // this.disputesService.dispute$.next(response);
      this.logger.info(
        'TicketInfoComponent::getDispute response',
        response
      );

      // var responseObject = JSON.parse("{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"status\":\"NEW\",\"ticketNumber\":\"AQ92926841\",\"provincialCourtHearingLocation\":\"Franklin\",\"issuedDate\":\"2022-05-11T14:38:15.225Z\",\"submittedDate\":\"2022-05-11T14:38:15.225Z\",\"surname\":\"Dame\",\"givenNames\":\"Lorraine Frances\",\"birthdate\":\"2022-05-11T14:38:15.225Z\",\"driversLicenceProvince\":\"British Columbia\",\"driversLicenceNumber\":\"3333333\",\"address\":\"3-1409 Camosun Street\",\"city\":\"Victoria\",\"province\":\"British Columbia\",\"postalCode\":\"V8V4L5\",\"homePhoneNumber\":\"2222222222\",\"workPhoneNumber\":\"2222222222\",\"emailAddress\":\"lfdpanda@live.ca\",\"filingDate\":\"2022-05-11T14:38:15.225Z\",\"disputedCounts\":[{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"plea\":\"NOT_GUILTY\",\"count\":1,\"requestTimeToPay\":false,\"requestReduction\":false,\"appearInCourt\":true,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa7\",\"plea\":\"NOT_GUILTY\",\"count\":2,\"requestTimeToPay\":false,\"requestReduction\":false,\"appearInCourt\":true,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa8\",\"plea\":\"NOT_GUILTY\",\"count\":3,\"requestTimeToPay\":false,\"requestReduction\":false,\"appearInCourt\":true,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}}],\"representedByLawyer\":true,\"legalRepresentation\":{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"lawFirmName\":\"MickeyMouse\",\"lawyerFullName\":\"Mickey\",\"lawyerEmail\":\"1@1.com\",\"lawyerAddress\":\"1234EasyStreet\",\"lawyerPhoneNumber\":\"3333333333\",\"additionalProperties\":{\"additionalProp1\":\"string\",\"additionalProp2\":\"string\",\"additionalProp3\":\"string\"}},\"interpreterLanguage\":\"Albanian\",\"numberOfWitness\":2,\"fineReductionReason\":\"finereductionreason\",\"timeToPayReason\":\"Timetopayreason\",\"rejectedReason\":\"string\",\"disputantDetectedOcrIssues\":true,\"disputantOcrIssuesDescription\":\"Name incorrect WWWWWWW\",\"systemDetectedOcrIssues\":true,\"jjAssigned\":\"Bryan\",\"ocrViolationTicket\":\"string\",\"violationTicket\":{\"violationTicketImage\":{\"image\":\"\",\"mimeType\":{\"name\":\"apple\",\"primaryType\":\"image\",\"subType\":\"pdf\",\"description\":\"descr\",\"extensions\":[\"string\"]}},\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"ticketNumber\":\"AQ92926841\",\"surname\":\"Dame\",\"givenNames\":\"Lorraine\",\"isYoungPerson\":false,\"driversLicenceNumber\":\"33333333\",\"driversLicenceProvince\":\"British Columbia\",\"driversLicenceProducedYear\":2020,\"driversLicenceExpiryYear\":2025,\"birthdate\":\"1965-07-01T14:38:15.225Z\",\"address\":\"3-1409CamosunStreet\",\"city\":\"Victoria\",\"province\":\"BritishColumbia\",\"postalCode\":\"V8V4L5\",\"isChangeOfAddress\":false,\"isDriver\":true,\"isCyclist\":false,\"isOwner\":true,\"isPedestrian\":false,\"isPassenger\":false,\"isOther\":false,\"otherDescription\":\"\",\"issuedDate\":\"2022-05-11T14:38:15.225Z\",\"issuedOnRoadOrHighway\":\"CamosunStreet\",\"issuedAtOrNearCity\":\"Victoria\",\"isMvaOffence\":true,\"isWlaOffence\":false,\"isLcaOffence\":false,\"isMcaOffence\":false,\"isFaaOffence\":false,\"isTcrOffence\":false,\"isCtaOffence\":false,\"isOtherOffence\":false,\"otherOffenceDescription\":\"other\",\"organizationLocation\":\"Victoria\",\"violationTicketCounts\":[{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa0\",\"count\":1,\"description\":\"Parking in a handicap spot without the required sticker\",\"actRegulation\":\"MVA\",\"fullSection\":\"139\",\"section\":\"c\",\"subsection\":\"\",\"paragraph\":\"\",\"ticketedAmount\":140,\"isAct\":false,\"isRegulation\":true,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa1\",\"count\":2,\"description\":\"Distracted driving\",\"actRegulation\":\"MVA\",\"fullSection\":\"182\",\"section\":\"7\",\"subsection\":\"\",\"paragraph\":\"\",\"ticketedAmount\":110,\"isAct\":false,\"isRegulation\":false,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa2\",\"count\":3,\"description\":\"Exceed licensed gross vehicle weight\",\"actRegulation\":\"MVA\",\"fullSection\":\"188\",\"section\":\"b\",\"subsection\":\"2\",\"paragraph\":\"\",\"ticketedAmount\":480,\"isAct\":false,\"isRegulation\":false,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}}],\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}}")
      this.initialDisputeValues = this.setViolationTicketFieldsFromJSON(response);
      this.lastUpdatedDispute = this.initialDisputeValues;
      this.form.patchValue(this.initialDisputeValues);

      // set violation date and time
      let violationDate = new Date(response.issuedDate);
      this.form.get('violationTicket').get('issuedDate').setValue(response.issuedDate);
      this.form.get('violationTicket').get('violationDate').setValue(this.datePipe.transform(violationDate, "yyyy-MM-dd"));
      this.form.get('violationTicket').get('violationTime').setValue(this.datePipe.transform(violationDate, "hhmm"));

      // ticket image
      this.imageToShow = 'data:image/png;base64,' + this.initialDisputeValues.violationTicket.violationTicketImage.image;

      // set disputant detected ocr issues
      this.flagsForm.get('disputantOcrIssuesDescription').setValue(response.disputantOcrIssuesDescription);

      // set provincial court hearing location in violation ticket subform
      this.form.get('violationTicket').get('provincialCourtHearingLocation').setValue(
        this.form.get('provincialCourtHearingLocation').value
      );

      // set counts 1,2,3 of violation ticket
      var i = 1;
      this.initialDisputeValues.violationTicket.violationTicketCounts.forEach(violationTicketCount => {

        if (i == 1) {
          this.form.get('violationTicket').get('violationTicketCount1').patchValue(violationTicketCount);
          this.form
            .get('violationTicket')
            .get('violationTicketCount1')
            .get('fullDescription')
            .setValue(this.getLegalParagraphing(violationTicketCount));
        }
        else if (i == 2) {
          this.form.get('violationTicket').get('violationTicketCount2').patchValue(violationTicketCount);
          this.form
            .get('violationTicket')
            .get('violationTicketCount2')
            .get('fullDescription')
            .setValue(this.getLegalParagraphing(violationTicketCount));
        }
        else if (i == 3) {
          this.form.get('violationTicket').get('violationTicketCount3').patchValue(violationTicketCount);
          this.form
            .get('violationTicket')
            .get('violationTicketCount3')
            .get('fullDescription')
            .setValue(this.getLegalParagraphing(violationTicketCount));
        }
        i++;
      });
    });
  }
}
