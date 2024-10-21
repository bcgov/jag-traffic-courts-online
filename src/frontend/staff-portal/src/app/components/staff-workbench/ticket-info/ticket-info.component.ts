import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatLegacyDialog as MatDialog, MatLegacyDialogConfig as MatDialogConfig } from '@angular/material/legacy-dialog';
import { ActivatedRoute } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { Dispute, DisputeService } from '../../../services/dispute.service';
import { CountryCodeValue, ProvinceCodeValue } from '@config/config.model';
import { ConfigService } from '@config/config.service';
import { DisputeContactTypeCd, ViolationTicket, ViolationTicketCount, ViolationTicketCountIsAct, ViolationTicketCountIsRegulation, DisputeStatus, DcfTemplateType } from 'app/api';
import { LookupsService, Statute } from 'app/services/lookups.service';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ConfirmReasonDialogComponent } from '@shared/dialogs/confirm-reason-dialog/confirm-reason-dialog.component';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { TicketImageDialogComponent } from '@shared/dialogs/ticket-image-dialog/ticket-image-dialog.component';
import { ViolationTicketService, OCRMessageToDisplay } from 'app/services/violation-ticket.service';
import { StringUtils } from '@core/utils/string-utils.class';

@Component({
  selector: 'app-ticket-info',
  templateUrl: './ticket-info.component.html',
  styleUrls: ['./ticket-info.component.scss'],
})
export class TicketInfoComponent implements OnInit {
  @Input() public disputeInfo: Dispute;
  @Output() public backInbox: EventEmitter<any> = new EventEmitter();

  public isMobile: boolean;
  public previousButtonIcon = 'keyboard_arrow_left';
  public validateClicked = false;

  public retrieving: boolean = true;
  public conflict: boolean = false;
  public previousButtonKey = 'stepper.backReview';
  public saveButtonKey = 'stepper.next';
  public lastUpdatedDispute: Dispute;
  public form: FormGroup;
  public flagsForm: FormGroup;
  public tempViolationTicketCount: ViolationTicketCount;
  public todayDate: Date = new Date();
  public provinces: ProvinceCodeValue[];
  public bc: ProvinceCodeValue;
  public canada: CountryCodeValue;
  public usa: CountryCodeValue;
  public states: ProvinceCodeValue[];
  public initialDisputeValues: Dispute;
  public imageToShow: any;
  public errorThreshold: number = 0.800;
  public courtLocationFlag: OCRMessageToDisplay;
  public IsAct = ViolationTicketCountIsAct;
  public IsRegulation = ViolationTicketCountIsRegulation;
  public ContactType = DisputeContactTypeCd;
  public DispStatus = DisputeStatus;
  public isTicketInformationReadOnly: boolean = false;
  public isViolationTicketCountDeleted: boolean = false;

  /**
   * @description
   * Whether to show the address line fields.
   */
  public filteredCount1Statutes: Statute[];
  public filteredCount2Statutes: Statute[];
  public filteredCount3Statutes: Statute[];
  public collapseObj: any = {
    ticketInformation: true,
    contactInformation: true,
    imageInformation: true
  }
  constructor(
    protected route: ActivatedRoute,
    protected formBuilder: FormBuilder,
    private utilsService: UtilsService,
    private dialog: MatDialog,
    private logger: LoggerService,
    public violationTicketService: ViolationTicketService,
    private disputeService: DisputeService,
    public config: ConfigService,
    public lookupsService: LookupsService
  ) {
    const today = new Date();

    this.bc = this.config.bcCodeValue;
    this.canada = this.config.canadaCodeValue;
    this.usa = this.config.usaCodeValue;

    this.isMobile = this.utilsService.isMobile();
    if (this.config.provincesAndStates) {
      this.provinces = this.config.provincesAndStates.filter(x => x.ctryId === this.canada.ctryId && x.provAbbreviationCd !== this.bc.provAbbreviationCd);
      this.states = this.config.provincesAndStates.filter(x => x.ctryId === this.usa.ctryId);
    }
  }

  public ngOnInit() {
    this.flagsForm = this.formBuilder.group({
      disputantOcrIssues: [null],
      disputantDetectedOcrIssues: [null],
    });
    this.form = this.formBuilder.group({
      ticketNumber: [null, [Validators.required]],
      homePhoneNumber: [null, [Validators.maxLength(20)]],
      emailAddress: [null, [Validators.email, Validators.maxLength(100)]],
      disputantSurname: [null, [Validators.required, Validators.maxLength(30)]],
      disputantGivenNames: [null, [Validators.required, Validators.maxLength(92)]],
      contactTypeCd: [null, [Validators.required]],
      contactSurnameNm: [null, [Validators.maxLength(30)]],
      contactGivenNames: [null, [Validators.maxLength(92)]],
      contactLawFirmNm: [null, [Validators.maxLength(200)]],
      addressCountryId: [null, [Validators.required]],
      address: [null, [Validators.required, Validators.maxLength(300)]],
      addressCity: [null, [Validators.required, Validators.maxLength(30)]],
      addressProvince: [null, [Validators.maxLength(30)]],
      addressProvinceProvId: [null],
      addressProvinceCountryId: [null],
      addressProvinceSeqNo: [null],
      postalCode: [null], // space needs to be added back to the middle for display
      driversLicenceNumber: [null, [Validators.minLength(7), Validators.maxLength(9)]],
      driversLicenceProvince: [null, [Validators.maxLength(30)]],
      driversLicenceProvinceProvId: [null],
      driversLicenceCountryId: [null],
      driversLicenceProvinceSeqNo: [null],
      rejectedReason: [null, Validators.maxLength(256)],
      violationTicket: this.formBuilder.group({
        ticketNumber: [null, Validators.required],
        courtLocation: [null, [Validators.required, Validators.maxLength(50)]],
        disputantSurname: [null, [Validators.required, Validators.maxLength(30)]],
        disputantGivenNames: [null, [Validators.required, Validators.maxLength(92)]],
        disputantDriversLicenceNumber: [null, [Validators.minLength(7), Validators.maxLength(9)]],
        driversLicenceProvince: [null, [Validators.maxLength(30)]],
        driversLicenceCountry: [null],
        issuedTs: [null, Validators.required],
        violationTicketCount1: this.formBuilder.group({
          actOrRegulationNameCode: [null],
          description: [null],
          subparagraph: [null],
          section: [null],
          subsection: [null],
          paragraph: [null],
          fullDescription: [null],
          ticketedAmount: [null, [FormControlValidators.currency]]
        }),
        violationTicketCount2: this.formBuilder.group({
          actOrRegulationNameCode: [null],
          description: [null],
          subparagraph: [null],
          section: [null],
          subsection: [null],
          paragraph: [null],
          fullDescription: [null],
          ticketedAmount: [null, [FormControlValidators.currency]]
        }),
        violationTicketCount3: this.formBuilder.group({
          actOrRegulationNameCode: [null],
          description: [null],
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

  onSelectContactType(newContactType: any) {
    this.form.get('contactGivenNames').setValue(null);
    this.form.get('contactSurnameNm').setValue(null);
    this.form.get('contactLawFirmNm').removeValidators(Validators.required);
    this.form.get('contactSurnameNm').removeValidators(Validators.required);
    this.form.get('contactGivenNames').removeValidators(Validators.required);
    this.form.get('contactLawFirmNm').setValue(null);
    if (newContactType == this.ContactType.Lawyer) {
      // make all contact info required
      this.form.get('contactLawFirmNm').addValidators(Validators.required);
      this.form.get('contactSurnameNm').addValidators(Validators.required);
      this.form.get('contactGivenNames').addValidators(Validators.required);
    } else if (newContactType == this.ContactType.Individual) {
      // leave contact info null and not required
    } else {
      // only contact names required
      this.form.get('contactSurnameNm').addValidators(Validators.required);
      this.form.get('contactGivenNames').addValidators(Validators.required);
    }
    this.form.get('contactLawFirmNm').updateValueAndValidity();
    this.form.get('contactSurnameNm').updateValueAndValidity();
    this.form.get('contactGivenNames').updateValueAndValidity();
    this.form.updateValueAndValidity();
  }

  public onFieldChange() {
    // Force the Save button to reevaluate validity.
    this.form.updateValueAndValidity();
  }

  public onCountryChange(ctryId: number) {

    setTimeout(() => {
      this.form.get('postalCode').setValidators([Validators.maxLength(6)]);
      this.form.get('addressProvince').setValidators([Validators.maxLength(30)]);
      this.form.get('addressProvince').setValue(null);
      this.form.get('addressProvinceSeqNo').setValidators(null);
      this.form.get('addressProvinceSeqNo').setValue(null);
      this.form.get('addressProvinceCountryId').setValue(null);
      this.form.get('addressProvinceProvId').setValue(null);
      this.form.get('homePhoneNumber').setValidators([Validators.maxLength(20)]);
      this.form.get('driversLicenceProvince').setValidators([Validators.maxLength(30)]);
      this.form.get("driversLicenceProvinceSeqNo").setValidators(null);

      if (ctryId === this.canada.ctryId || ctryId == this.usa.ctryId) {
        this.form.get('addressProvinceSeqNo').addValidators([Validators.required]);
        this.form.get('postalCode').addValidators([Validators.required]);
        this.form.get('homePhoneNumber').addValidators([FormControlValidators.phone]);
      }

      if (ctryId == this.canada.ctryId) {
        this.form.get('postalCode').addValidators([Validators.minLength(6)]);        
        this.form.get('addressProvinceCountryId').setValue(ctryId);
        this.form.get("addressProvinceSeqNo").setValue(this.bc.provSeqNo);
        this.form.get("addressProvinceProvId").setValue(this.bc.provId);
        this.form.get("addressProvince").setValue(this.bc.provAbbreviationCd);
      }

      this.form.get('postalCode').updateValueAndValidity();
      this.form.get('addressProvince').updateValueAndValidity();
      this.form.get("addressProvinceCountryId").updateValueAndValidity();
      this.form.get("addressProvinceSeqNo").updateValueAndValidity();
      this.form.get("addressProvinceProvId").updateValueAndValidity();
      this.form.get('homePhoneNumber').updateValueAndValidity();
      this.form.get('driversLicenceProvince').updateValueAndValidity();
      this.form.get("driversLicenceProvinceSeqNo").updateValueAndValidity();
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
  public filterStatutes(val: string): Statute[] {
    if (!this.lookupsService.statutes || this.lookupsService.statutes.length == 0) return [];
    return this.lookupsService.statutes?.filter(option => (option.__statuteString || "").toUpperCase().indexOf((val || "").toUpperCase()) >= 0);
  }

  // is the statute valid? on the form
  public isStatuteValid(countNo: number): boolean {
    let countForm = this.form.get('violationTicket').get('violationTicketCount' + countNo.toString());
    if (countForm.get('fullDescription').value && !countForm.get('section').value && 
      countForm.get('fullDescription').value !== " ") 
      return false;
    return true;
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

  public onExpandTicketImage() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = { imageToShow: this.imageToShow }
    dialogConfig.hasBackdrop = false;
    dialogConfig.position = {
      top: '200px',
      left: '0px'
    }
    this.dialog.open(TicketImageDialogComponent, dialogConfig).afterClosed()
      .subscribe((action: any) => {
      });
  }

  public onBack() {
    this.backInbox.emit();
  }

  // violation ticket borders only for new status
  public applyOverErrThreshold(fieldName: string): boolean {
    if (this.lastUpdatedDispute.status != this.DispStatus.New) return false;
    if (this.lastUpdatedDispute.violationTicket.ocrViolationTicket && this.lastUpdatedDispute.violationTicket.ocrViolationTicket.fields[fieldName]?.fieldConfidence <= 0.80) return false;
    return true;
  }

  public applyUnderErrThreshold(fieldName: string): boolean {
    if (this.lastUpdatedDispute.status != this.DispStatus.New) return false;
    if (this.lastUpdatedDispute.violationTicket.ocrViolationTicket && this.lastUpdatedDispute.violationTicket.ocrViolationTicket.fields[fieldName]?.fieldConfidence > 0.80) return false;
    return true;
  }

  // change validators on drivers licence number in violation ticket when changing province / state
  public onViolationTicketDLProvinceChange(provAbbreviationCd: string) {

    setTimeout(() => {
      if(provAbbreviationCd === null){
        this.form.get('violationTicket').get('driversLicenceCountry').setValue(null);
        this.form.get('violationTicket').get('driversLicenceProvince').setValue(null);
        this.form.get('violationTicket').get('disputantDriversLicenceNumber').setValidators([Validators.maxLength(20)]);
      } else{
        if (provAbbreviationCd == this.bc.provAbbreviationCd) {
          this.form.get('violationTicket').get('disputantDriversLicenceNumber').setValidators([Validators.maxLength(9)])
          this.form.get('violationTicket').get('disputantDriversLicenceNumber').addValidators([Validators.minLength(7)]);
        } else {
          this.form.get('violationTicket').get('disputantDriversLicenceNumber').setValidators([Validators.maxLength(20)]);
        }
        let provFound = this.config.provincesAndStates.filter(x => x.provAbbreviationCd === provAbbreviationCd).shift();
        if (provFound) {
          let ctryFound = this.config.countries.filter(x => x.ctryId === provFound.ctryId).shift();
          this.form.get('violationTicket').get('driversLicenceCountry').setValue(ctryFound.ctryLongNm);
        }
      }      
      this.form.get('violationTicket').get('disputantDriversLicenceNumber').updateValueAndValidity();
    }, 5)
  }

  public onAddressProvinceChange(provId: number) {
    setTimeout(() => {
      let provFound = this.config.provincesAndStates.filter(x => x.provId === provId).shift();
      this.form.get("addressProvinceCountryId").setValue(provFound.ctryId);
      this.form.get("addressProvinceSeqNo").setValue(provFound.provSeqNo);
      this.form.get("addressProvinceProvId").setValue(provFound.provId);
      this.form.get("addressProvince").setValue(provFound.provAbbreviationCd);
    }, 0)
  }

  // change validators on drivers licence number in notice of dispute when changing province / state
  public onNoticeOfDisputeDLProvinceChange(provId: number) {
    setTimeout(() => {
      if(provId === null){
        this.form.get("driversLicenceProvince").setValue(null);
        this.form.get("driversLicenceCountryId").setValue(null);
        this.form.get("driversLicenceProvinceSeqNo").setValue(null);
        this.form.get('driversLicenceNumber').setValidators([Validators.maxLength(20)]);
      } else{
        let provFound = this.config.provincesAndStates.filter(x => x.provId === provId).shift();
        this.form.get("driversLicenceCountryId").setValue(provFound.ctryId);
        this.form.get("driversLicenceProvinceSeqNo").setValue(provFound.provSeqNo);
        if (provId === this.bc.provId) {
          this.form.get('driversLicenceNumber').setValidators([Validators.maxLength(9)]);
          this.form.get('driversLicenceNumber').addValidators([Validators.minLength(7)]);
        } else {
          this.form.get('driversLicenceNumber').setValidators([Validators.maxLength(20)]);
        }        
      }
      this.form.get('driversLicenceNumber').updateValueAndValidity();
      this.onFieldChange();
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
    putDispute.violationTicket.driversLicenceCountry = this.form.get('violationTicket').get('driversLicenceCountry').value;
    putDispute.violationTicket.courtLocation = this.form.get('violationTicket').get('courtLocation').value;

    // reconstruct issued date as string from violation date and violation time format yyyy-mm-ddTHH:mm
    putDispute.violationTicket.issuedTs =
      this.form.get('violationTicket').get('violationDate').value +
      "T" +
      this.form.get('violationTicket').get('violationTime').value.substring(0, 2)
      + ":" +
      this.form.get('violationTicket').get('violationTime').value.substring(2, 4) + "Z";
    putDispute.issuedTs = this.form.get('violationTicket').get('violationDate').value +
      "T" +
      this.form.get('violationTicket').get('violationTime').value.substring(0, 2)
      + ":" +
      this.form.get('violationTicket').get('violationTime').value.substring(2, 4) + ":00Z";;

    // Counts 1,2,3
    putDispute.violationTicket.violationTicketCounts = [] as ViolationTicketCount[];
    for (let i = 1; i <= 3; i++) {
      // stuff 3 violation ticket counts in putDispute
      let violationTicketCount = this.form.get('violationTicket').get('violationTicketCount' + i.toString()).value as ViolationTicketCount;
      violationTicketCount.countNo = i;
      violationTicketCount.ticketedAmount = this.form.get('violationTicket').get('violationTicketCount' + i.toString()).get('ticketedAmount').value;
      putDispute.violationTicket.violationTicketCounts = [...putDispute.violationTicket.violationTicketCounts, violationTicketCount];
    }

    this.logger.log('TicketInfoComponent::putDispute', putDispute);
    this.lastUpdatedDispute = putDispute;

    if(this.validateClicked) {
      this.validate(putDispute);
    }
    else {
      this.putDispute(putDispute);
    }
  }

  public onSubmitNoticeOfDispute(): void {
    // We are only sending the notice of dispute fields so update a local copy of lastUpdatedDispute
    // with notice of dispute form fields only that were changed
    let putDispute = this.lastUpdatedDispute;
    putDispute.disputantSurname = this.form.get('disputantSurname').value;
    putDispute.disputantGivenNames = this.form.get('disputantGivenNames').value;
    putDispute.driversLicenceNumber = this.form.get('driversLicenceNumber').value;
    putDispute.driversLicenceProvince = this.form.get('driversLicenceProvince').value;
    putDispute.driversLicenceIssuedCountryId = this.form.get('driversLicenceCountryId').value;
    putDispute.driversLicenceIssuedProvinceSeqNo = this.form.get('driversLicenceProvinceSeqNo').value;
    putDispute.homePhoneNumber = this.form.get('homePhoneNumber').value;
    putDispute.emailAddress = this.form.get('emailAddress').value;
    putDispute.address = this.form.get('address').value;
    putDispute.addressCity = this.form.get('addressCity').value;
    putDispute.addressProvince = this.form.get('addressProvince').value;
    putDispute.addressProvinceCountryId = this.form.get('addressProvinceCountryId').value;
    putDispute.addressProvinceSeqNo = this.form.get('addressProvinceSeqNo').value;
    putDispute.addressCountryId = this.form.get('addressCountryId').value;
    putDispute.postalCode = this.form.get('postalCode').value;
    putDispute.rejectedReason = this.form.get('rejectedReason').value;

    // set dispute courtagenid from violation ticket courthouse location
    let courtFound = this.lookupsService.courthouseAgencies.filter(x => x.name === putDispute.violationTicket.courtLocation).shift();
    putDispute.courtAgenId = courtFound?.id;

    this.logger.log('TicketInfoComponent::putDispute', putDispute);
    this.lastUpdatedDispute = putDispute;

    this.putDispute(putDispute, true);
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

  public enableNoticeOfDisputeSave(): boolean {

    // check for fields invalid in contact information only
    if (this.form.get('emailAddress').invalid) return false;
    if (this.form.get('homePhoneNumber').invalid) return false;
    if (this.form.get('disputantSurname').invalid) return false;
    if (this.form.get('disputantGivenNames').invalid) return false;
    if (this.form.get('addressCountryId').invalid) return false;
    if (this.form.get('address').invalid) return false;
    if (this.form.get('addressCity').invalid) return false;
    if (this.form.get('addressProvince').invalid) return false;
    if (this.form.get('addressProvinceCountryId').invalid) return false;
    if (this.form.get('addressProvinceSeqNo').invalid) return false;
    if (this.form.get('addressProvinceProvId').invalid) return false;
    if (this.form.get('postalCode').invalid) return false;
    if (this.form.get('driversLicenceNumber').invalid) return false;
    if (this.form.get('driversLicenceProvince').invalid) return false;
    if (this.form.get('driversLicenceCountryId').invalid) return false;
    if (this.form.get('driversLicenceProvinceProvId').invalid) return false;
    if (this.form.get('driversLicenceProvinceSeqNo').invalid) return false;
    if (this.form.get('rejectedReason').invalid) return false;

    // check for touched fields
    if (this.form.get('emailAddress').touched) return true;
    if (this.form.get('homePhoneNumber').touched) return true;
    if (this.form.get('disputantSurname').touched) return true;
    if (this.form.get('disputantGivenNames').touched) return true;
    if (this.form.get('addressCountryId').touched) return true;
    if (this.form.get('address').touched) return true;
    if (this.form.get('addressCity').touched) return true;
    if (this.form.get('addressProvince').touched) return true;
    if (this.form.get('addressProvinceCountryId').touched) return true;
    if (this.form.get('addressProvinceSeqNo').touched) return true;
    if (this.form.get('addressProvinceProvId').touched) return true;
    if (this.form.get('postalCode').touched) return true;
    if (this.form.get('driversLicenceNumber').touched) return true;
    if (this.form.get('driversLicenceProvince').touched) return true;
    if (this.form.get('driversLicenceCountryId').touched) return true;
    if (this.form.get('driversLicenceProvinceProvId').touched) return true;
    if (this.form.get('driversLicenceProvinceSeqNo').touched) return true;
    if (this.form.get('rejectedReason').touched) return true;

    // no contact information touched, all valid
    return false;
  }

  public handleCollapse(name: string) {
    this.collapseObj[name] = !this.collapseObj[name]
  }

  // get legal paragraphing for a particular count
  public getCountLegalParagraphing(countNumber: number, violationTicket: ViolationTicket): string {
    let violationTicketCount = violationTicket.violationTicketCounts?.filter(x => x.countNo == countNumber)[0];
    if (violationTicketCount) {
      let desc = (this.violationTicketService
        .getLegalParagraphing(violationTicketCount) + (violationTicketCount.description ? " " + violationTicketCount.description : ""));
      return desc;
    }
    else return "";
  }

  // put dispute by id
  putDispute(dispute: Dispute, isSubmittingNoticeOfDispute: boolean = false): void {
    this.logger.log('TicketInfoComponent::putDispute', dispute);

    // no need to pass back byte array with image
    let tempDispute = dispute;
    tempDispute.violationTicket.violationTicketImage = null;

    const data: DialogOptions = {
      titleKey: "Enter File History Comment",
      messageKey: "Please describe the changes you made to the dispute.",
      actionTextKey: "Save",
      actionType: "primary",
      cancelTextKey: "Go back",
      icon: "error_outline"
    };

    this.dialog.open(ConfirmReasonDialogComponent, { data }).afterClosed()
      .subscribe((action?: any) => {
        if (action?.output?.response) {
          this.disputeService.putDispute(dispute.disputeId, action?.output?.reason, tempDispute).subscribe((response: Dispute) => {
            this.logger.info(
              'TicketInfoComponent::putDispute response',
              response
            );
            this.isViolationTicketCountDeleted = false;
            if (!isSubmittingNoticeOfDispute) {
              // markAsUntouched form group
              this.form.get('violationTicket').markAsUntouched();
            } else {
              // markAsUntouched notice of dispute fields
              this.form.get('disputantSurname').markAsUntouched();
              this.form.get('disputantGivenNames').markAsUntouched();
              this.form.get('driversLicenceNumber').markAsUntouched();
              this.form.get('driversLicenceProvince').markAsUntouched();
              this.form.get('driversLicenceCountryId').markAsUntouched();
              this.form.get('driversLicenceProvinceSeqNo').markAsUntouched();
              this.form.get('driversLicenceProvinceProvId').markAsUntouched();
              this.form.get('homePhoneNumber').markAsUntouched();
              this.form.get('emailAddress').markAsUntouched();
              this.form.get('address').markAsUntouched();
              this.form.get('addressCity').markAsUntouched();
              this.form.get('addressProvince').markAsUntouched();
              this.form.get('addressProvinceSeqNo').markAsUntouched();
              this.form.get('addressProvinceCountryId').markAsUntouched();
              this.form.get('addressProvinceProvId').markAsUntouched();
              this.form.get('addressCountryId').markAsUntouched();
              this.form.get('postalCode').markAsUntouched();
              this.form.get('rejectedReason').markAsUntouched();
            }
          });
        }
      });
  }

  // use violationTicket Service
  setFieldsFromJSON(dispute: Dispute): Dispute {
    if (dispute.violationTicket?.ocrViolationTicket && dispute.violationTicket?.ocrViolationTicket?.fields) {
      var fields = dispute.violationTicket?.ocrViolationTicket.fields;

      dispute.violationTicket = this.violationTicketService.setViolationTicketFromJSON(dispute.violationTicket.ocrViolationTicket, dispute.violationTicket);
    }

    return dispute;
  }

  // send to api, on return update status
  validate(dispute: Dispute): void {
    // no need to pass back byte array with image
    let tempDispute = dispute;
    tempDispute.violationTicket.violationTicketImage = null;

    this.disputeService.validateDispute(this.lastUpdatedDispute.disputeId, tempDispute).subscribe({
      next: response => {
        this.lastUpdatedDispute.status = this.DispStatus.Validated;
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
    this.dialog.open(ConfirmDialogComponent, { data }).afterClosed()
      .subscribe((action: any) => {
        if (action) {

          // submit dispute and return to TRM home
          this.disputeService.submitDispute(this.lastUpdatedDispute.disputeId).subscribe(
            {
              next: response => {
                this.lastUpdatedDispute.status = this.DispStatus.Processing;
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
      message: ""
    };
    this.dialog.open(ConfirmReasonDialogComponent, { data }).afterClosed()
      .subscribe((action?: any) => {
        if (action?.output?.response) {
          this.form.get('rejectedReason').setValue(action.output.reason); // update on form for appearances
          this.lastUpdatedDispute.rejectedReason = action.output.reason; // update to send back on put

          // udate the reason entered, reject dispute and return to TRM home
          this.disputeService.rejectDispute(this.lastUpdatedDispute.disputeId, this.lastUpdatedDispute.rejectedReason).subscribe({
            next: response => {
              this.onBack();
              this.lastUpdatedDispute.status = this.DispStatus.Rejected;
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
      message: ""
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
          this.disputeService.cancelDispute(this.lastUpdatedDispute.disputeId, action.output.reason).subscribe({
            next: response => {
              this.lastUpdatedDispute.status = this.DispStatus.Cancelled;
              this.lastUpdatedDispute.rejectedReason = action.output.reason;
              this.onBack();
            },
            error: err => { },
            complete: () => { }
          });
        }
      });
  }

  // count description changed in form
  onChangeCount(countNo: number, fullDescription: string) {
    let countForm = this.form.get('violationTicket').get('violationTicketCount' + countNo.toString());
    let parts = fullDescription.split(" "); // act/code fullsection description

    // lookup legal statute from part[0] which should be in legal paragraph form
    if (parts && parts.length > 1) {
      let foundStatute = this.lookupsService.statutes?.find(x => StringUtils.nullSafeCompare(x.actCode, parts[0]) && StringUtils.nullSafeCompare(x.code, parts[1]));
      if (foundStatute) {
        countForm.get('actOrRegulationNameCode').setValue(foundStatute.actCode);
        countForm.get('section').setValue(foundStatute.sectionText);
        countForm.get('subsection').setValue(foundStatute.subsectionText);
        countForm.get('paragraph').setValue(foundStatute.paragraphText);
        countForm.get('subparagraph').setValue(foundStatute.subparagraphText);
        countForm.get('description').setValue(foundStatute.shortDescriptionText);
        countForm.get('fullDescription').setValue(`${foundStatute.actCode} ${foundStatute.code} ${foundStatute.shortDescriptionText}`);
      } else {
        countForm.get('actOrRegulationNameCode').setValue(undefined);
        countForm.get('section').setValue(undefined);
        countForm.get('subsection').setValue(undefined);
        countForm.get('paragraph').setValue(undefined);
        countForm.get('subparagraph').setValue(undefined);
        countForm.get('description').setValue(undefined);
      }
      countForm.updateValueAndValidity();
    }
  }

  // get dispute by id
  getDispute(): void {
    this.logger.log('TicketInfoComponent::getDispute');

    this.conflict = false;
    this.initialDisputeValues = null;
    this.lastUpdatedDispute = null;

    this.disputeService.getDispute(this.disputeInfo.disputeId)
      .subscribe((response: Dispute) => {
        this.retrieving = false;
        this.logger.info(
          'TicketInfoComponent::getDispute response',
          response
        );

        // If disputant surname is filled in, then this is not the first time this ticket has been opened, only call setFieldsFromJSON the first time
        if (response.violationTicket.disputantSurname) {
          this.initialDisputeValues = response;
        } else {
          this.initialDisputeValues = this.setFieldsFromJSON(response);
        }

        // set court agency id if possible
        let courtFound = this.lookupsService.courthouseAgencies.filter(x => x.name === this.initialDisputeValues.violationTicket.courtLocation);
        if (courtFound?.length > 0) this.initialDisputeValues.courtAgenId = courtFound[0].id;

        this.lastUpdatedDispute = JSON.parse(JSON.stringify(this.initialDisputeValues));
        this.form.patchValue(this.initialDisputeValues);
        this.form.get('driversLicenceProvinceSeqNo').setValue(this.initialDisputeValues.driversLicenceIssuedProvinceSeqNo);
        this.form.get('driversLicenceCountryId').setValue(this.initialDisputeValues.driversLicenceIssuedCountryId);

        // set provId for drivers Licence and address this field is only good client side as angular dropdown needs a single value key to behave well, doesnt like two part key of ctryid & seqno
        let provFound = this.config.provincesAndStates.filter(x => x.ctryId === this.initialDisputeValues.driversLicenceIssuedCountryId && x.provSeqNo === this.initialDisputeValues.driversLicenceIssuedProvinceSeqNo).shift();
        if (provFound) this.form.get('driversLicenceProvinceProvId').setValue(provFound.provId);

        // set violation date and time using violation ticket issuedTs as source of truth
        if (!this.initialDisputeValues.violationTicket.issuedTs) this.initialDisputeValues.violationTicket.issuedTs = this.initialDisputeValues.issuedTs;
        let violationDate = this.initialDisputeValues.violationTicket.issuedTs?.split("T");
        if (violationDate && violationDate.length > 1) {
          this.form.get('violationTicket').get('issuedTs').setValue(this.initialDisputeValues.violationTicket.issuedTs);
          this.form.get('violationTicket').get('violationDate').setValue(violationDate[0]);
          this.form.get('violationTicket').get('violationTime').setValue(violationDate[1].split(":")[0] + violationDate[1].split(":")[1]);
          this.initialDisputeValues.issuedTs = this.initialDisputeValues.violationTicket.issuedTs;
          this.lastUpdatedDispute.issuedTs = this.initialDisputeValues.violationTicket.issuedTs;
        }

        // ticket image
        if (this.initialDisputeValues?.violationTicket?.violationTicketImage?.mimeType) {
          this.imageToShow = "data:" + this.initialDisputeValues.violationTicket.violationTicketImage.mimeType + ";base64," + this.initialDisputeValues.violationTicket.violationTicketImage.image;
        } else if (this.initialDisputeValues?.violationTicket?.violationTicketImage?.image) {
          this.imageToShow = 'data:image/png;base64,' + this.initialDisputeValues?.violationTicket?.violationTicketImage?.image;
        }

        // set disputant detected ocr issues
        this.flagsForm.get('disputantOcrIssues').setValue(response.disputantOcrIssues);

        // set counts 1,2,3 of violation ticket
        this.assignViolationTicketCountForm();

        this.violationTicketService.getAllOCRMessages(this.lastUpdatedDispute.violationTicket.ocrViolationTicket);

        // set system flags for provincial court hearing location
        this.courtLocationFlag = {
          heading: "Court Location",
          key: "court_location",
          fieldConfidence: this.lastUpdatedDispute.violationTicket.ocrViolationTicket?.fields["court_location"]?.fieldConfidence
        };

        // update address field validators
        provFound = this.config.provincesAndStates.filter(x => x.ctryId === this.initialDisputeValues.addressProvinceCountryId && x.provSeqNo === this.initialDisputeValues.addressProvinceSeqNo).shift();
        if (provFound) this.form.get('addressProvinceProvId').setValue(provFound.provId);
        this.form.get('addressProvince').setValidators([Validators.maxLength(30)]);
        this.form.get('homePhoneNumber').setValidators([Validators.maxLength(20)]);
        this.form.get('driversLicenceProvince').setValidators([Validators.maxLength(30)]);
        this.form.get("driversLicenceProvinceSeqNo").setValidators(null);

        if (this.form.get('addressCountryId').value === this.canada.ctryId || this.form.get('addressCountryId').value === this.usa.ctryId) {
          this.form.get('addressProvinceSeqNo').addValidators([Validators.required]);
          this.form.get('postalCode').addValidators([Validators.required]);
          this.form.get('homePhoneNumber').addValidators([FormControlValidators.phone]);
        }

        if (this.form.get('addressCountryId').value == this.canada.ctryId) {
          this.form.get('postalCode').addValidators([Validators.minLength(6)]);
        }
        this.form.get('postalCode').updateValueAndValidity();
        this.form.get('addressProvince').updateValueAndValidity();
        this.form.get("addressProvinceSeqNo").updateValueAndValidity();
        this.form.get("addressProvinceProvId").updateValueAndValidity();
        this.form.get('homePhoneNumber').updateValueAndValidity();
        this.form.get('driversLicenceProvince').updateValueAndValidity();
        this.form.get("driversLicenceProvinceSeqNo").updateValueAndValidity();

        if (this.lastUpdatedDispute.status !== this.DispStatus.New) {
          this.form.controls.violationTicket.disable();
        }
        this.form.get('violationTicket').updateValueAndValidity();

        // TCVP-2554 make a static variable to indicate if the TicketInformation section is editable or not.
        this.isTicketInformationReadOnly = this.lastUpdatedDispute.status === this.DispStatus.Validated;
      }, (error: any) => {
        this.retrieving = false;
        if (error.status == 409) this.conflict = true;
      });
  }

  assignViolationTicketCountForm(){
    this.initialDisputeValues.violationTicket.violationTicketCounts.forEach(violationTicketCount => {
      let countForm = this.form.get('violationTicket').get('violationTicketCount' + violationTicketCount.countNo.toString());
      countForm.markAsUntouched();
      countForm.get('ticketedAmount').setValue(violationTicketCount.ticketedAmount);
      let fullDesc = this.getCountLegalParagraphing(violationTicketCount.countNo, this.initialDisputeValues.violationTicket);
      countForm
        .get('fullDescription')
        .setValue(fullDesc);
      countForm.get('description').setValue(violationTicketCount.description);

      // lookup legal statute
      let actCode = violationTicketCount.actOrRegulationNameCode;
      if (!actCode) {
        actCode = violationTicketCount.isAct === ViolationTicketCountIsAct.Y ? "MVA" : "MVR";
      }
      const sectionCode = this.getSectionText(violationTicketCount);
      let foundStatute = this.lookupsService.statutes?.find(x => StringUtils.nullSafeCompare(x.actCode, actCode) && StringUtils.nullSafeCompare(x.code, sectionCode));
      if (foundStatute) {
        countForm.get('actOrRegulationNameCode').setValue(foundStatute.actCode);
        countForm.get('section').setValue(foundStatute.sectionText);
        countForm.get('subsection').setValue(foundStatute.subsectionText);
        countForm.get('paragraph').setValue(foundStatute.paragraphText);
        countForm.get('subparagraph').setValue(foundStatute.subparagraphText);
      }
      countForm.updateValueAndValidity();
    });
  }

  /**
   * Returns the full section text from a ViolationTicketCount, ie 44(3)(a)(iii)
   * @param vtc 
   * @returns 
   */
  private getSectionText(vtc: ViolationTicketCount): string {
    let sectionText = vtc.section ?? "";
    sectionText += vtc.subsection ? '(' + vtc.subsection + ')' : "";
    sectionText += vtc.paragraph ? '(' + vtc.paragraph + ')' : "";
    sectionText += vtc.subparagraph ? '(' + vtc.subparagraph + ')' : "";
    return sectionText;
  }

  public onValidateClick(): void {
    this.validateClicked = true;
  }

  onPrint() {
    var type = DcfTemplateType.DcfTemplate;
    
    this.disputeService.apiTicketValidationPrintGet(this.lastUpdatedDispute.disputeId, type).subscribe(result => {
      if (result) {
        var url = URL.createObjectURL(result);
        window.open(url);
      } else {
        alert("File contents not found");
      }
    });
  }

  onDeleteViolationTicketCount(countNumber: number) {
    let violationTicketCounts = this.lastUpdatedDispute.violationTicket.violationTicketCounts;
    let count = violationTicketCounts.find(x => x.countNo == countNumber);
    const data: DialogOptions = {
      titleKey: "Delete Violation Ticket Count?",
      messageKey: "Are you sure you want to delete this violation ticket count?",
      actionTextKey: "Delete",
      actionType: "warn",
      cancelTextKey: "Cancel",
      icon: "delete"
    };
    this.dialog.open(ConfirmDialogComponent, { data }).afterClosed()
      .subscribe((action: any) => {
        if (action) {
          var countForm = this.form.get('violationTicket').get('violationTicketCount' + countNumber.toString());
          countForm.reset();
          countForm.get('ticketedAmount').reset();
          countForm.get('fullDescription').reset();
          countForm.get('description').reset();
          countForm.get('actOrRegulationNameCode').reset();
          countForm.get('section').reset();
          countForm.get('subsection').reset();
          countForm.get('paragraph').reset();
          countForm.get('subparagraph').reset();
          countForm.updateValueAndValidity();
          countForm.markAsTouched();
          count.ticketedAmount = null;
          count.description = null;
          count.actOrRegulationNameCode = null;
          count.section = null;
          count.subsection = null;
          count.paragraph = null;
          count.subparagraph = null;
          this.lastUpdatedDispute.violationTicket.violationTicketCounts = violationTicketCounts.filter(x => x.countNo != countNumber);
          this.lastUpdatedDispute.violationTicket.violationTicketCounts.splice(countNumber - 1, 0, count);
          this.isViolationTicketCountDeleted = true;
          this.lastUpdatedDispute = { ...this.lastUpdatedDispute };
        }
      });
  }

  onCancelChanges() {
    this.assignViolationTicketCountForm();
    this.lastUpdatedDispute.violationTicket.violationTicketCounts = 
      JSON.parse(JSON.stringify(this.initialDisputeValues.violationTicket.violationTicketCounts));
    this.lastUpdatedDispute = { ...this.lastUpdatedDispute };
    this.validateClicked = false;
    this.isViolationTicketCountDeleted = false;
  }
}
