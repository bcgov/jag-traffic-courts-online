import { Component, EventEmitter, Input, OnInit, Output, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { DatePipe } from '@angular/common';
import { DisputeExtended, DisputeService } from '../../../services/dispute.service';
import { Subscription } from 'rxjs';
import { ProvinceConfig, CourthouseConfig } from '@config/config.model';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { ViolationTicket, ViolationTicketCount, ViolationTicketCountIsAct, ViolationTicketCountIsRegulation } from 'app/api';
import { LookupsService, StatuteView } from 'app/services/lookups.service';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ConfirmReasonDialogComponent } from '@shared/dialogs/confirm-reason-dialog/confirm-reason-dialog.component';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { ViolationTicketService, OCRMessageToDisplay } from 'app/services/violation-ticket.service';

@Component({
  selector: 'app-ticket-info',
  templateUrl: './ticket-info.component.html',
  styleUrls: ['./ticket-info.component.scss'],
})
export class TicketInfoComponent implements OnInit {
  @Input() public disputeInfo: DisputeExtended;
  @Output() public backInbox: EventEmitter<any> = new EventEmitter();

  public isMobile: boolean;
  public previousButtonIcon = 'keyboard_arrow_left';

  public retrieving: boolean = true;
  public conflict: boolean = false;
  public previousButtonKey = 'stepper.backReview';
  public saveButtonKey = 'stepper.next';
  public busy: Subscription;
  public lastUpdatedDispute: DisputeExtended;
  public form: FormGroup;
  public flagsForm: FormGroup;
  public tempViolationTicketCount: ViolationTicketCount;
  public todayDate: Date = new Date();
  public provinces: ProvinceConfig[];
  public states: ProvinceConfig[];
  public initialDisputeValues: DisputeExtended;
  public courtLocations: CourthouseConfig[];
  public imageToShow: any;
  public errorThreshold: number = 0.800;
  public courtLocationFlag: OCRMessageToDisplay;
  public IsAct = ViolationTicketCountIsAct;
  public IsRegulation = ViolationTicketCountIsRegulation;

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
    public violationTicketService: ViolationTicketService,
    private disputeService: DisputeService,
    public mockConfigService: MockConfigService,
    public lookupsService: LookupsService,
    @Inject(Router) private router,
  ) {
    const today = new Date();
    this.isMobile = this.utilsService.isMobile();
    if (this.mockConfigService.provinces) {
      this.provinces = this.mockConfigService.provinces.filter(x => x.countryCode == 'CA' && x.code != 'BC');
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
      disputantOcrIssues: [null],
      disputantDetectedOcrIssues: [null],
    });
    this.form = this.formBuilder.group({
      ticketNumber: [null, [Validators.required]],
      homePhoneNumber: [null, [Validators.required, Validators.maxLength(20)]],
      emailAddress: [null, [Validators.email]],
      disputantSurname: [null, [Validators.required]],
      disputantGivenNames: [null, [Validators.required]],
      country: [null, [Validators.required]],
      disputantBirthdate: [null, [Validators.required]], // Optional
      address: [null, [Validators.required, Validators.maxLength(300)]],
      addressCity: [null, [Validators.required]],
      addressProvince: [null, [Validators.required, Validators.maxLength(30)]],
      postalCode: [null, [Validators.required, Validators.maxLength(6), Validators.minLength(6)]], // space needs to be added back to the middle for display
      driversLicenceNumber: [null, [Validators.required, Validators.minLength(7), Validators.maxLength(9)]],
      driversLicenceProvince: [null, [Validators.required, Validators.maxLength(30)]],
      rejectedReason: [null, Validators.maxLength(256)],
      violationTicket: this.formBuilder.group({
        ticketNumber: [null, Validators.required],
        courtLocation: [null, [Validators.required]],
        disputantSurname: [null, Validators.required],
        disputantGivenNames: [null, Validators.required],
        disputantDriversLicenceNumber: [null, [Validators.required, Validators.minLength(7), Validators.maxLength(9)]],
        driversLicenceProvince: [null, [Validators.required, Validators.maxLength(30)]],
        issuedTs: [null, Validators.required],
        violationTicketCount1: this.formBuilder.group({
          description: [null],
          actOrRegulationNameCode: [null],
          subparagraph: [null],
          section: [null],
          subsection: [null],
          paragraph: [null],
          fullDescription: [null],
          ticketedAmount: [null, [FormControlValidators.currency]]
        }),
        violationTicketCount2: this.formBuilder.group({
          description: [null],
          actOrRegulationNameCode: [null],
          subparagraph: [null],
          section: [null],
          subsection: [null],
          paragraph: [null],
          fullDescription: [null],
          ticketedAmount: [null, [FormControlValidators.currency]]
        }),
        violationTicketCount3: this.formBuilder.group({
          description: [null],
          actOrRegulationNameCode: [null],
          subparagraph: [null],
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

  public onCountryChange(country) {

    setTimeout(() => {
      this.form.get('postalCode').setValidators([Validators.maxLength(6)]);
      this.form.get('addressProvince').setValidators([Validators.maxLength(30)]);
      this.form.get('homePhoneNumber').setValidators([Validators.maxLength(20)]);
      this.form.get('driversLicenceProvince').setValidators([Validators.maxLength(30)]);
      this.form.get('violationTicket').get('driversLicenceProvince').setValidators([Validators.maxLength(30)]);

      if (country == 'Canada' || country == 'United States') {
        this.form.get('addressProvince').addValidators([Validators.required]);
        this.form.get('postalCode').addValidators([Validators.required]);
        this.form.get('homePhoneNumber').addValidators([Validators.required, FormControlValidators.phone]);
        this.form.get('driversLicenceProvince').addValidators([Validators.required]);
        this.form.get('violationTicket').get('driversLicenceProvince').addValidators([Validators.required]);
      }

      if (country == 'Canada') {
        this.form.get('postalCode').addValidators([Validators.minLength(6)]);
      }

      this.form.get('postalCode').updateValueAndValidity();
      this.form.get('addressProvince').updateValueAndValidity();
      this.form.get('homePhoneNumber').updateValueAndValidity();
      this.form.get('driversLicenceProvince').updateValueAndValidity();
      this.form.get('violationTicket').get('driversLicenceProvince').updateValueAndValidity();
      this.onNoticeOfDisputeDLProvinceChange(this.form.get('driversLicenceProvince').value);
      this.onViolationTicketDLProvinceChange(this.form.get('violationTicket').get('driversLicenceProvince').value);
    }, 5);
  }

  onFullDescription1Keyup() {
    this.filteredCount1Statutes = this.filterStatutes(this.form.get('violationTicket').get('violationTicketCount1').get('fullDescription').value);
  }

  onFullDescription2Keyup() {
    this.filteredCount2Statutes = this.filterStatutes(this.form.get('violationTicket').get('violationTicketCount2').get('fullDescription').value);
  }

  onFullDescription3Keyup() {
    this.filteredCount3Statutes = this.filterStatutes(this.form.get('violationTicket').get('violationTicketCount3').get('fullDescription').value);
  }

  // return a filtered list of statutes
  public filterStatutes(val: string): StatuteView[] {
    if (!this.lookupsService.statutes || this.lookupsService.statutes.length == 0) return [];
    return this.lookupsService.statutes.filter(option => option.__statuteString.indexOf(val) >= 0);
  }

  public resendEmailVerification() {
    this.disputeService.resendEmailVerification(this.lastUpdatedDispute.disputeId)
    .subscribe(email => {
      const data: DialogOptions = {
        titleKey: "Email Verification Resent",
        icon: "email",
        actionType: "green",
        messageKey:
          "The email verification has been resent to the contact email address provided.\n\n" + this.lastUpdatedDispute.emailAddress,
        actionTextKey: "Ok",
        cancelHide: true
      };
      this.dialog.open(ConfirmDialogComponent, { data }).afterClosed()
        .subscribe((action: any) => {
        });
    })
  }

  public onBack() {
    this.backInbox.emit();
  }

  // violation ticket borders only for new status
  public applyOverErrThreshold(fieldName: string): boolean {
    if (this.lastUpdatedDispute.status != 'NEW') return false;
    if (this.lastUpdatedDispute.violationTicket.ocrViolationTicket && this.lastUpdatedDispute.violationTicket.ocrViolationTicket.fields[fieldName]?.fieldConfidence <= 0.80) return false;
    return true;
  }

  public applyUnderErrThreshold(fieldName: string): boolean {
    if (this.lastUpdatedDispute.status != 'NEW') return false;
    if (this.lastUpdatedDispute.violationTicket.ocrViolationTicket && this.lastUpdatedDispute.violationTicket.ocrViolationTicket.fields[fieldName]?.fieldConfidence > 0.80) return false;
    return true;
  }

  // change validators on drivers licence number in violation ticket when changing province / state
  public onViolationTicketDLProvinceChange(province: string) {

    setTimeout(() => {
      if (province == 'BC') {
        this.form.get('violationTicket').get('disputantDriversLicenceNumber').setValidators([Validators.maxLength(9)])
        this.form.get('violationTicket').get('disputantDriversLicenceNumber').addValidators([Validators.minLength(7)]);
      } else {
        this.form.get('violationTicket').get('disputantDriversLicenceNumber').setValidators([Validators.maxLength(20)]);
      }
      if (this.form.get('country').value == 'United States' || this.form.get('country').value == 'Canada') {
        this.form.get('violationTicket').get('disputantDriversLicenceNumber').addValidators([Validators.required]);
      }
      this.form.get('violationTicket').get('disputantDriversLicenceNumber').updateValueAndValidity();
    }, 5)
  }

  // change validators on drivers licence number in notice of dispute when changing province / state
  public onNoticeOfDisputeDLProvinceChange(province: string) {
    setTimeout(() => {
      if (province == 'BC') {
        this.form.get('driversLicenceNumber').setValidators([Validators.maxLength(9)]);
        this.form.get('driversLicenceNumber').addValidators([Validators.minLength(7)]);
      } else {
        this.form.get('driversLicenceNumber').setValidators([Validators.maxLength(20)]);
      }
      if (this.form.get('country').value == 'United States' || this.form.get('country').value == 'Canada') {
        this.form.get('driversLicenceNumber').addValidators([Validators.required]);
      }
      this.form.get('driversLicenceNumber').updateValueAndValidity();
    }, 5)
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
    putDispute.violationTicket.disputantSurname = this.form.get('violationTicket').get('disputantSurname').value;
    putDispute.violationTicket.disputantGivenNames = this.form.get('violationTicket').get('disputantGivenNames').value;
    putDispute.violationTicket.disputantDriversLicenceNumber = this.form.get('violationTicket').get('disputantDriversLicenceNumber').value;
    putDispute.violationTicket.driversLicenceProvince = this.form.get('violationTicket').get('driversLicenceProvince').value;
    putDispute.violationTicket.courtLocation = this.form.get('violationTicket').get('courtLocation').value;

    // reconstruct issued date as string from violation date and violation time format yyyy-mm-ddThh:mm
    putDispute.violationTicket.issuedTs =
      this.form.get('violationTicket').get('violationDate').value +
      "T" +
      this.form.get('violationTicket').get('violationTime').value.substring(0, 2)
      + ":" +
      this.form.get('violationTicket').get('violationTime').value.substring(2, 4);


    // Counts 1,2,3
    putDispute.violationTicket.violationTicketCounts = [] as ViolationTicketCount[];
    for (let i = 1; i <= 3; i++) {
      // if form has violation ticket, stuff it in putDispute
      if (this.form.get('violationTicket').get('violationTicketCount' + i.toString()).get('fullDescription').value) {
        let violationTicketCount = this.constructViolationTicketCount(
          this.form.get('violationTicket').get('violationTicketCount' + i.toString()).get('fullDescription').value, i);
        violationTicketCount.ticketedAmount = this.form.get('violationTicket').get('violationTicketCount' + i.toString()).get('ticketedAmount').value;
        putDispute.violationTicket.violationTicketCounts = [...putDispute.violationTicket.violationTicketCounts, violationTicketCount];
      }
    }

    this.logger.log('TicketInfoComponent::putDispute', putDispute);

    // no need to pass back byte array with image
    let tempDispute = putDispute;
    tempDispute.violationTicket.violationTicketImage = null;

    this.busy = this.disputeService.putDispute(tempDispute.disputeId, tempDispute).subscribe((response: DisputeExtended) => {
      this.logger.info(
        'TicketInfoComponent::putDispute response',
        response
      );
    });

    // markAsUntouched form group
    this.form.get('violationTicket').markAsUntouched();
  }

  public onSubmitNoticeOfDispute(): void {
    // We are only sending the notice of dispute fields so update a local copy of lastUpdatedDispute
    // with notice of dispute form fields only that were changed
    let putDispute = this.lastUpdatedDispute;

    putDispute.disputantSurname = this.form.get('disputantSurname').value;
    putDispute.disputantGivenNames = this.form.get('disputantGivenNames').value;
    putDispute.driversLicenceNumber = this.form.get('driversLicenceNumber').value;
    putDispute.driversLicenceProvince = this.form.get('driversLicenceProvince').value;
    putDispute.homePhoneNumber = this.form.get('homePhoneNumber').value;
    putDispute.emailAddress = this.form.get('emailAddress').value;
    putDispute.disputantBirthdate = this.form.get('disputantBirthdate').value;
    putDispute.address = this.form.get('address').value;
    putDispute.addressCity = this.form.get('addressCity').value;
    putDispute.addressProvince = this.form.get('addressProvince').value;
    putDispute.postalCode = this.form.get('postalCode').value;
    putDispute.rejectedReason = this.form.get('rejectedReason').value;

    this.logger.log('TicketInfoComponent::putDispute', putDispute);

    // no need to pass back byte array with image
    let tempDispute = putDispute;
    tempDispute.violationTicket.violationTicketImage = null;

    this.busy = this.disputeService.putDispute(tempDispute.disputeId, tempDispute).subscribe((response: DisputeExtended) => {
      this.logger.info(
        'TicketInfoComponent::putDispute response',
        response
      );

      // markAsUntouched notice of dispute fields
      this.form.get('disputantSurname').markAsUntouched();
      this.form.get('disputantGivenNames').markAsUntouched();
      this.form.get('driversLicenceNumber').markAsUntouched();
      this.form.get('driversLicenceProvince').markAsUntouched();
      this.form.get('homePhoneNumber').markAsUntouched();
      this.form.get('emailAddress').markAsUntouched();
      this.form.get('disputantBirthdate').markAsUntouched();
      this.form.get('address').markAsUntouched();
      this.form.get('addressCity').markAsUntouched();
      this.form.get('addressProvince').markAsUntouched();
      this.form.get('country').markAsUntouched();
      this.form.get('postalCode').markAsUntouched();
      this.form.get('rejectedReason').markAsUntouched();
    });
  }

  // decompose string into subparagraph, section, subsection, paragraph
  public unLegalParagraph(statuteLegalParagraphing: string): { subparagraph: string, section: string, subsection: string, paragraph: string } {
    let allParts = statuteLegalParagraphing.split("(");
    let subparagraph = "";
    let section = "";
    let subsection = "";
    let paragraph = "";

    // parts are section(section)(subsection)(paragraph)(subparagraph) if all are present
    // extract substrings but dont include final ')' of each part
    if (allParts.length > 0) section = allParts[0].substring(0, allParts[0].length);
    if (allParts.length > 1) subsection = allParts[1].substring(0, allParts[1].length - 1);
    if (allParts.length > 2) paragraph = allParts[2].substring(0, allParts[2].length - 1);
    if (allParts.length > 3) subparagraph = allParts[3].substring(0, allParts[3].length - 1);

    return { subparagraph: subparagraph, section: section, subsection: subsection, paragraph: paragraph };
  }

  public constructViolationTicketCount(__statuteString: string, count: number): ViolationTicketCount {

    this.tempViolationTicketCount = { description: "", ticketedAmount: null, actOrRegulationNameCode: "", isAct: this.IsAct.N, isRegulation: this.IsRegulation.N, paragraph: "", subparagraph: "", subsection: "", section: "" };
    this.tempViolationTicketCount.countNo = count;
    if (!__statuteString) return this.tempViolationTicketCount;

    // look in list of statutes
    let statute = this.lookupsService.statutes.filter(x => x.__statuteString == __statuteString) as StatuteView[];
    if (statute && statute.length > 0) {
      this.tempViolationTicketCount.description = statute[0].descriptionText;
      this.tempViolationTicketCount.actOrRegulationNameCode = statute[0].code; // to do break down legal paragraphing
      let parts = this.unLegalParagraph(statute[0].sectionText);
      this.tempViolationTicketCount.subparagraph = parts.subparagraph;
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
      this.tempViolationTicketCount.actOrRegulationNameCode = __statuteStringParts[0];
      let parts = this.unLegalParagraph(__statuteStringParts[1]);
      this.tempViolationTicketCount.subparagraph = parts.subparagraph;
      this.tempViolationTicketCount.section = parts.section;
      this.tempViolationTicketCount.subsection = parts.subsection;
      this.tempViolationTicketCount.paragraph = parts.paragraph;
      this.tempViolationTicketCount.description = __statuteString.substring(
        __statuteString.indexOf(__statuteStringParts[2])
      );
    }

    // two parts hmm....
    else if (__statuteStringParts.length > 1) {
      this.tempViolationTicketCount.actOrRegulationNameCode = "";
      this.tempViolationTicketCount.section = __statuteStringParts[0];
      this.tempViolationTicketCount.description = __statuteString.substring(__statuteString.indexOf(__statuteStringParts[1]));

      // yuk shove it in description field
    } else {
      this.tempViolationTicketCount.actOrRegulationNameCode = "";
      this.tempViolationTicketCount.section = "";
      this.tempViolationTicketCount.description = __statuteString;
    }

    return this.tempViolationTicketCount;
  }

  public enableNoticeOfDisputeSave(): boolean {

    // check for fields invalid in contact information only
    if (this.form.get('emailAddress').invalid) return false;
    if (this.form.get('homePhoneNumber').invalid) return false;
    if (this.form.get('disputantSurname').invalid) return false;
    if (this.form.get('disputantGivenNames').invalid) return false;
    if (this.form.get('country').invalid) return false;
    if (this.form.get('disputantBirthdate').invalid) return false;
    if (this.form.get('address').invalid) return false;
    if (this.form.get('addressCity').invalid) return false;
    if (this.form.get('addressProvince').invalid) return false;
    if (this.form.get('postalCode').invalid) return false;
    if (this.form.get('driversLicenceNumber').invalid) return false;
    if (this.form.get('driversLicenceProvince').invalid) return false;
    if (this.form.get('rejectedReason').invalid) return false;

    // check for touched fields
    if (this.form.get('emailAddress').touched) return true;
    if (this.form.get('homePhoneNumber').touched) return true;
    if (this.form.get('disputantSurname').touched) return true;
    if (this.form.get('disputantGivenNames').touched) return true;
    if (this.form.get('country').touched) return true;
    if (this.form.get('disputantBirthdate').touched) return true;
    if (this.form.get('address').touched) return true;
    if (this.form.get('addressCity').touched) return true;
    if (this.form.get('addressProvince').touched) return true;
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

  // get legal paragraphing for a particular count
  public getCountLegalParagraphing(countNumber: number, violationTicket: ViolationTicket): string {
    let violationTicketCount = violationTicket.violationTicketCounts.filter(x => x.countNo == countNumber)[0];
    if (violationTicketCount) {
      let desc = (this.violationTicketService
        .getLegalParagraphing(violationTicketCount) + (violationTicketCount.description ? " " + violationTicketCount.description : ""));
      return desc;
    }
    else return "";
  }

  // put dispute by id
  putDispute(dispute: DisputeExtended): void {
    this.logger.log('TicketInfoComponent::putDispute', dispute);

    // no need to pass back byte array with image
    let tempDispute = dispute;
    tempDispute.violationTicket.violationTicketImage = null;

    this.busy = this.disputeService.putDispute(dispute.disputeId, tempDispute).subscribe((response: DisputeExtended) => {
      this.logger.info(
        'TicketInfoComponent::putDispute response',
        response
      );
    });
  }

  // use violationTicket Service
  setFieldsFromJSON(dispute: DisputeExtended): DisputeExtended {
    var objOcrViolationTicket = dispute.violationTicket.ocrViolationTicket;

    if (objOcrViolationTicket && objOcrViolationTicket.fields) {
      var fields = objOcrViolationTicket.fields;

      dispute.violationTicket = this.violationTicketService.setViolationTicketFromJSON(dispute.violationTicket.ocrViolationTicket, dispute.violationTicket);
    }

    return dispute;
  }

  // send to api, on return update status
  validate(): void {
    this.busy = this.disputeService.validateDispute(this.lastUpdatedDispute.disputeId).subscribe({
      next: response => {
        this.lastUpdatedDispute.status = "VALIDATED";
        this.form.controls.violationTicket.disable();
      },
      error: err => { },
      complete: () => { }
    });
  }

  // dialog, if ok then send to api, on return update status, return to TRM home
  public approve(): void {
    const data: DialogOptions = {
      titleKey: "Approve ticket resolution request?",
      messageKey:
        "Once you approve this request, the information will be sent to ICBC. Are you sure you are ready to approve and submit this request to ARC?",
      actionTextKey: "Approve and send request",
      actionType: "green",
      cancelTextKey: "Go back",
      icon: "error_outline",
    };
    this.lastUpdatedDispute.status = 'PROCESSING';
    this.dialog.open(ConfirmDialogComponent, { data }).afterClosed()
      .subscribe((action: any) => {
        if (action) {

          // submit dispute and return to TRM home
          this.busy = this.disputeService.submitDispute(this.lastUpdatedDispute.disputeId).subscribe(
            {
              next: response => {
                this.lastUpdatedDispute.status = 'PROCESSING';
                this.onBack();
              },
              error: err => { },
              complete: () => { }
            }
          );
        }
      });
  }

  // dialog, if ok then send to api, on return update status, return to TRM home
  public reject(): void {
    const data: DialogOptions = {
      titleKey: "Reject ticket resolution request?",
      messageKey:
        "Please enter the reason this request is being rejected. This information will be sent to the user in email notification.",
      actionTextKey: "Send rejection notification",
      actionType: "warn",
      cancelTextKey: "Go back",
      icon: "error_outline",
      message: this.form.get('rejectedReason').value
    };
    this.dialog.open(ConfirmReasonDialogComponent, { data }).afterClosed()
      .subscribe((action?: any) => {
        if (action?.output?.response) {
          this.form.get('rejectedReason').setValue(action.output.reason); // update on form for appearances
          this.lastUpdatedDispute.rejectedReason = action.output.reason; // update to send back on put

          // udate the reason entered, reject dispute and return to TRM home
          this.busy = this.disputeService.rejectDispute(this.lastUpdatedDispute.disputeId, this.lastUpdatedDispute.rejectedReason).subscribe({
            next: response => {
              this.onBack();
              this.lastUpdatedDispute.status = 'REJECTED';
              this.lastUpdatedDispute.rejectedReason = action.output.reason;
            },
            error: err => { },
            complete: () => { }
          });
        }
      });
  }

  // dialog, if ok then send to api, on return update status, return to TRM home
  public cancel(): void {
    const data: DialogOptions = {
      titleKey: "Cancel ticket resolution request?",
      messageKey:
        "Please enter the reason this request is being cancelled. This information will be sent to the user in email notification.",
      actionTextKey: "Send cancellation notification",
      actionType: "warn",
      cancelTextKey: "Go back",
      icon: "error_outline",
      message: this.form.get('rejectedReason').value
    };
    this.dialog.open(ConfirmReasonDialogComponent, { data }).afterClosed()
      .subscribe((action?: any) => {
        if (action?.output?.response) {
          this.form.get('rejectedReason').setValue(action.output.reason); // update on form for appearances
          this.lastUpdatedDispute.rejectedReason = action.output.reason; // update to send back on put

          // no need to pass back byte array with image
          let tempDispute = this.lastUpdatedDispute;
          delete tempDispute.violationTicket.violationTicketImage;

          // udate the reason entered, cancel dispute and return to TRM home since this will be filtered out
          this.busy = this.disputeService.putDispute(tempDispute.disputeId, tempDispute).subscribe({
            next: response => {
              this.disputeService.cancelDispute(this.lastUpdatedDispute.disputeId).subscribe({
                next: response => {
                  this.lastUpdatedDispute.status = 'CANCELLED';
                  this.lastUpdatedDispute.rejectedReason = action.output.reason;
                  this.onBack();
                },
                error: err => { },
                complete: () => { }
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

    this.busy = this.disputeService.getDispute(this.disputeInfo.disputeId)
      .subscribe((response: DisputeExtended) => {
        this.retrieving = false;
        this.logger.info(
          'TicketInfoComponent::getDispute response',
          response
        );

        this.initialDisputeValues = this.setFieldsFromJSON(response);
        this.lastUpdatedDispute = this.initialDisputeValues;
        this.form.patchValue(this.initialDisputeValues);

        // set violation date and time
        let violationDate = response.issuedTs?.split("T");
        if (violationDate) {
          this.form.get('violationTicket').get('issuedTs').setValue(response.issuedTs);
          this.form.get('violationTicket').get('violationDate').setValue(violationDate[0]);
          this.form.get('violationTicket').get('violationTime').setValue(violationDate[1].split(":")[0] + violationDate[1].split(":")[1]);
        }

        // ticket image
        if (this.initialDisputeValues.violationTicket.violationTicketImage.mimeType) {
          this.imageToShow = "data:" + this.initialDisputeValues.violationTicket.violationTicketImage.mimeType + ";base64," + this.initialDisputeValues.violationTicket.violationTicketImage.image;
        } else {
          this.imageToShow = 'data:image/png;base64,' + this.initialDisputeValues.violationTicket.violationTicketImage.image;
        }

        // set disputant detected ocr issues
        this.flagsForm.get('disputantOcrIssues').setValue(response.disputantOcrIssues);

        // set counts 1,2,3 of violation ticket
        this.initialDisputeValues.violationTicket.violationTicketCounts.forEach(violationTicketCount => {
          let countForm = this.form.get('violationTicket').get('violationTicketCount' + violationTicketCount.countNo.toString());
          countForm.patchValue(violationTicketCount);
          if (!violationTicketCount.ticketedAmount)
            countForm.get('ticketedAmount').setValue(undefined);
          let fullDesc = this.getCountLegalParagraphing(violationTicketCount.countNo, this.initialDisputeValues.violationTicket);
          countForm
            .get('fullDescription')
            .setValue(fullDesc);
        });

        this.violationTicketService.getAllOCRMessages(this.lastUpdatedDispute.violationTicket.ocrViolationTicket);

        // set system flags for provincial court hearing location
        this.courtLocationFlag = {
          heading: "Court Location",
          key: "court_location",
          fieldConfidence: this.lastUpdatedDispute.violationTicket.ocrViolationTicket?.fields["court_location"]?.fieldConfidence
        };

        // update validation rule for drivers licence number
        // set country from province
        if (this.provinces.filter(x => x.name == this.lastUpdatedDispute.addressProvince || this.lastUpdatedDispute.addressProvince == "British Columbia").length > 0) this.form.get('country').setValue("Canada");
        else if (this.states.filter(x => x.name == this.initialDisputeValues.addressProvince).length > 0) this.form.get('country').setValue("United States");
        else this.form.get('country').setValue("International");

        this.onCountryChange(this.form.get('country').value);
        if (this.lastUpdatedDispute.status !== "NEW") {
          this.form.controls.violationTicket.disable();
        }
      }, (error: any) => {
        this.retrieving = false;
        if (error.status == 409) this.conflict = true;
      });
  }
}
