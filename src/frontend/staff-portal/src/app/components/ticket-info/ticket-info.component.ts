import { Component, EventEmitter, Input, OnInit, Output, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { DatePipe } from '@angular/common';
import { DisputeView, DisputesService } from '../../services/disputes.service';
import { Subscription } from 'rxjs';
import { ProvinceConfig, Config } from '@config/config.model';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { Dispute, ViolationTicket, ViolationTicketCount } from 'app/api';
import { LookupsService, StatuteView } from 'app/services/lookups.service';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ConfirmReasonDialogComponent } from '@shared/dialogs/confirm-reason-dialog/confirm-reason-dialog.component';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-ticket-info',
  templateUrl: './ticket-info.component.html',
  styleUrls: ['./ticket-info.component.scss'],
})
export class TicketInfoComponent implements OnInit {
  @Input() public disputeInfo: DisputeView;
  @Output() public backTicketList: EventEmitter<any> = new EventEmitter();
  public isMobile: boolean;
  public previousButtonIcon = 'keyboard_arrow_left';

  public retrieving: boolean = true;
  public conflict: boolean = false;
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
    protected formBuilder: FormBuilder,
    private utilsService: UtilsService,
    private datePipe: DatePipe,
    private dialog: MatDialog,
    private logger: LoggerService,
    private disputesService: DisputesService,
    public mockConfigService: MockConfigService,
    public lookupsService: LookupsService,
    @Inject(Router) private router,
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
      rejectedReason: [null],
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
    return this.lookupsService.statutes.filter(option => option.__statuteString.indexOf(val) >= 0);
  }

  public onBack() {
    this.backTicketList.emit();
  }

  // change validators on drivers licence number in violation ticket when changing province / state
  public onViolationTicketDLProvinceChange(province: string) {
    if (province == 'British Columbia')
      this.form.get('violationTicket').get('driversLicenceNumber').setValidators([Validators.required, Validators.minLength(7), Validators.maxLength(9), Validators.pattern(/^(\d{7}|\d{8}|\d{9})$/)]);
    else
      this.form.get('violationTicket').get('driversLicenceNumber').setValidators(Validators.required);
    this.form.get('violationTicket').get('driversLicenceNumber').updateValueAndValidity();
    this.form.updateValueAndValidity();
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

    this.logger.log('TicketInfoComponent::putDispute', putDispute);

    // no need to pass back byte array with image
    let tempDispute = putDispute;
    tempDispute.violationTicket.violationTicketImage = null;

    this.busy = this.disputesService.putDispute(tempDispute.id, tempDispute).subscribe((response: Dispute) => {
      this.logger.info(
        'TicketInfoComponent::putDispute response',
        response
      );

      // this structure contains last version of what was send to db
      this.lastUpdatedDispute = response;
    });

    // markAsUntouched form group
    this.form.get('violationTicket').markAsUntouched();
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
    putDispute.rejectedReason = this.form.get('rejectedReason').value;

    this.logger.log('TicketInfoComponent::putDispute', putDispute);

    // no need to pass back byte array with image
    let tempDispute = putDispute;
    tempDispute.violationTicket.violationTicketImage = null;

    this.busy = this.disputesService.putDispute(tempDispute.id, tempDispute).subscribe((response: Dispute) => {
      this.logger.info(
        'TicketInfoComponent::putDispute response',
        response
      );

      // this structure contains last version of what was send to db
      this.lastUpdatedDispute = response;

      // markAsUntouched notice of dispute fields
      this.form.get('surname').markAsUntouched();
      this.form.get('givenNames').markAsUntouched();
      this.form.get('driversLicenceNumber').markAsUntouched();
      this.form.get('driversLicenceProvince').markAsUntouched();
      this.form.get('homePhoneNumber').markAsUntouched();
      this.form.get('emailAddres').markAsUntouched();
      // this.form.get('birthdate').markAsUntouched();
      this.form.get('address').markAsUntouched();
      this.form.get('city').markAsUntouched();
      this.form.get('province').markAsUntouched();
      this.form.get('postalCode').markAsUntouched();
      this.form.get('rejectedReason').markAsUntouched();
    });
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

  public constructViolationTicketCount(__statuteString: string): ViolationTicketCount {

    this.tempViolationTicketCount = { description: "", actRegulation: "", fullSection: "", section: "", subsection: "", paragraph: "" };

    // look in list of statutes
    let statute = this.lookupsService.statutes.filter(x => x.__statuteString == __statuteString) as StatuteView[];
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
    const __statuteStringParts = __statuteString.split(" ");

    // three parts yay
    if (__statuteStringParts.length > 2) {
      this.tempViolationTicketCount.actRegulation = __statuteStringParts[0];
      let parts = this.unLegalParagraph(__statuteStringParts[1]);
      this.tempViolationTicketCount.fullSection = parts.fullSection;
      this.tempViolationTicketCount.section = parts.section;
      this.tempViolationTicketCount.subsection = parts.subsection;
      this.tempViolationTicketCount.paragraph = parts.paragraph;
      this.tempViolationTicketCount.description = __statuteString.substring(
        __statuteString.indexOf(__statuteStringParts[2])
      );
    }

    // two parts hmm....
    else if (__statuteStringParts.length > 1) {
      this.tempViolationTicketCount.actRegulation = "";
      this.tempViolationTicketCount.fullSection = __statuteStringParts[0];
      this.tempViolationTicketCount.description = __statuteString.substring(__statuteString.indexOf(__statuteStringParts[1]));

      // yuk shove it in description field
    } else {
      this.tempViolationTicketCount.actRegulation = "";
      this.tempViolationTicketCount.fullSection = "";
      this.tempViolationTicketCount.description = __statuteString;
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
    if (this.form.get('rejectedReason').invalid) return false;

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
    if (this.form.get('rejectedReason').touched) return true;

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

  validate(): void {
    this.lastUpdatedDispute.status = "VALIDATED";
  }

  public approve(): void {
    const data: DialogOptions = {
      titleKey: "Approve ticket resolution request?",
      messageKey:
        "Once you approve this request, the information will be sent to ICBC. Are you sure you are ready to approve and submit this request to ARC?",
      actionTextKey: "Approve and send request",
      actionType: "warn",
      cancelTextKey: "Go back",
      icon: "error_outline",
    };
    this.dialog.open(ConfirmDialogComponent, { data }).afterClosed()
      .subscribe((action: any) => {
        if (action) {

          // submit dispute and return to TRM home
          this.busy = this.disputesService.submitDispute(this.lastUpdatedDispute.id).subscribe(
            {
              next: response => { this.onBack(); },
              error: err => { },
              complete: () => { }
            }
          );
        }
      });
  }

  public reject(): void {
    const data: DialogOptions = {
      titleKey: "Reject ticket resolution request?",
      messageKey:
        "Please enter the reason this request is being rejected. This information will be sent to the user in email notification.",
      actionTextKey: "Send rejection notification",
      actionType: "warn",
      cancelTextKey: "Go back",
      icon: "error_outline",
    };
    this.dialog.open(ConfirmReasonDialogComponent, { data }).afterClosed()
      .subscribe((action: any) => {
        if (action.output.response) {
          this.lastUpdatedDispute.rejectedReason = action.output.reason;

          // udate the reason entered, reject dispute and return to TRM home 
          this.busy = this.disputesService.rejectDispute(this.lastUpdatedDispute.id, this.lastUpdatedDispute.rejectedReason).subscribe({
            next: response => { this.onBack(); },
            error: err => { },
            complete: () => { }
          });
        }
      });
  }

  public cancel(): void {
    const data: DialogOptions = {
      titleKey: "Cancel ticket resolution request?",
      messageKey:
        "Please enter the reason this request is being cancelled. This information will be sent to the user in email notification.",
      actionTextKey: "Send cancellation notification",
      actionType: "warn",
      cancelTextKey: "Go back",
      icon: "error_outline"
    };
    this.dialog.open(ConfirmReasonDialogComponent, { data }).afterClosed()
      .subscribe((action: any) => {
        if (action.output.response) {
          this.lastUpdatedDispute.rejectedReason = action.output.reason;

          // no need to pass back byte array with image
          let tempDispute = this.lastUpdatedDispute;
          tempDispute.violationTicket.violationTicketImage = null;

          // udate the reason entered, cancel dispute and return to TRM home since this will be filtered out
          this.busy = this.disputesService.putDispute(tempDispute.id, tempDispute).subscribe({
            next: response => {
              this.disputesService.cancelDispute(this.lastUpdatedDispute.id).subscribe({
                next: response => { this.onBack() },
                error: err => { },
                complete: () => {}
              });
            },
            error: err => { },
            complete: () => { }
          });
        }
      });
  }

  // get dispute by id
  getDispute(): void {
    this.logger.log('TicketInfoComponent::getDispute');

    this.conflict = false;
    this.initialDisputeValues = null;
    this.lastUpdatedDispute = null;

    this.busy = this.disputesService.getDispute(this.disputeInfo.id)
      .subscribe((response: Dispute) => {
        this.retrieving = false;
        this.logger.info(
          'TicketInfoComponent::getDispute response',
          response
        );

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
        this.initialDisputeValues.violationTicket.violationTicketCounts.forEach(violationTicketCount => {

          this.form.get('violationTicket').get('violationTicketCount' + violationTicketCount.count.toString()).patchValue(violationTicketCount);
          this.form
            .get('violationTicket')
            .get('violationTicketCount' + violationTicketCount.count.toString())
            .get('fullDescription')
            .setValue(this.getLegalParagraphing(violationTicketCount));
        });
      },
        (error: any) => {
          this.retrieving = false;
          if (error.status == 409) this.conflict = true;
        });
  }
}
