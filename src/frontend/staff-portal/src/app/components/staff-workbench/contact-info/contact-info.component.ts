import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { ConfigService } from '@config/config.service';
import { Dispute, DisputeService } from '../../../services/dispute.service';
import { Subscription } from 'rxjs';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ConfirmReasonDialogComponent } from '@shared/dialogs/confirm-reason-dialog/confirm-reason-dialog.component';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { CountryCodeValue, ProvinceCodeValue } from '@config/config.model';
import { DisputeContactTypeCd, DisputeStatus } from 'app/api';

@Component({
  selector: 'app-contact-info',
  templateUrl: './contact-info.component.html',
  styleUrls: ['./contact-info.component.scss', '../../../app.component.scss']
})
export class ContactInfoComponent implements OnInit {
  @Input() public disputeInfo: Dispute;
  @Output() public backInbox: EventEmitter<any> = new EventEmitter();
  public isMobile: boolean;
  public infoHeight: number = window.innerHeight - 175; // less size of other fixed elements
  public provinces: ProvinceCodeValue[];
  public states: ProvinceCodeValue[];
  public bc: ProvinceCodeValue;
  public canada: CountryCodeValue;
  public usa: CountryCodeValue;
  public busy: Subscription;
  public initialDisputeValues: Dispute;
  public todayDate: Date = new Date();
  public lastUpdatedDispute: Dispute;
  public retrieving: boolean = true;
  public violationDate: string = "";
  public violationTime: string = "";
  public conflict: boolean = false;
  public form: FormGroup;
  public ContactType = DisputeContactTypeCd;
  public DispStatus = DisputeStatus;
  public collapseObj: any = {
    contactInformation: true
  }

  constructor(
    protected route: ActivatedRoute,
    protected formBuilder: FormBuilder,
    private dialog: MatDialog,
    private utilsService: UtilsService,
    public config: ConfigService,
    private disputeService: DisputeService,
    private logger: LoggerService,
  ) {
    const today = new Date();
    this.isMobile = this.utilsService.isMobile();

    this.bc = this.config.bcCodeValue;
    this.canada = this.config.canadaCodeValue;
    this.usa = this.config.usaCodeValue;
    this.provinces = this.config.provincesAndStates.filter(x => x.ctryId === this.canada.ctryId && x.provSeqNo !== this.bc.provSeqNo);  // skip BC it will be manually at top of list
    this.states = this.config.provincesAndStates.filter(x => x.ctryId === this.usa.ctryId); // USA only
  }

  public ngOnInit() {
    this.form = this.formBuilder.group({
      ticketNumber: [null, [Validators.required]],
      homePhoneNumber: [null, [Validators.required, Validators.maxLength(20)]],
      emailAddress: [null, [Validators.email, Validators.maxLength(100)]],
      contactTypeCd: [null, [Validators.required]],
      contactSurnameNm: [null, [Validators.maxLength(30)]],
      contactGivenNames: [null, [Validators.maxLength(92)]],
      contactLawFirmNm: [null, [Validators.maxLength(200)]],
      disputantSurname: [null, [Validators.required, Validators.maxLength(30)]],
      disputantGivenNames: [null, [Validators.required, Validators.maxLength(92)]],
      address: [null, [Validators.required, Validators.maxLength(300)]],
      addressCity: [null, [Validators.required, Validators.maxLength(30)]],
      addressProvince: [null, [Validators.required, Validators.maxLength(30)]],
      addressProvinceProvId: [null],
      addressProvinceCountryId: [null],
      addressProvinceSeqNo: [null],
      addressCountryId: [null, [Validators.required]],
      rejectedReason: [null, Validators.maxLength(256)], // Optional
      postalCode: [null, [Validators.required]],
      driversLicenceNumber: [null, [Validators.required, Validators.minLength(7), Validators.maxLength(9)]],
      driversLicenceProvince: [null, [Validators.required, Validators.maxLength(30)]],
      driversLicenceProvinceProvId: [null],
      driversLicenceCountryId: [null],
      driversLicenceProvinceSeqNo: [null],
    });
    this.getDispute();
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
      this.form.get('driversLicenceProvinceSeqNo').setValidators(null);

      if (ctryId === this.canada.ctryId || ctryId === this.usa.ctryId) {
        this.form.get('addressProvinceSeqNo').addValidators([Validators.required]);
        this.form.get('postalCode').addValidators([Validators.required]);
        this.form.get('homePhoneNumber').addValidators([Validators.required, FormControlValidators.phone]);
        this.form.get('driversLicenceProvinceSeqNo').addValidators([Validators.required]);
      } else this.form.get('addressProvince').addValidators([Validators.required]);

      if (ctryId == this.canada.ctryId) {
        this.form.get('postalCode').addValidators([Validators.minLength(6)]);
        this.form.get("addressProvince").setValue(this.bc.provNm);
        this.form.get("addressProvinceSeqNo").setValue(this.bc.provSeqNo)
        this.form.get("addressProvinceProvId").setValue(this.bc.provId);
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

  public onDLProvinceChange(provId: number) {
    setTimeout(() => {
      let provFound = this.config.provincesAndStates.filter(x => x.provId === provId).shift();
      if (!provFound) return;
      this.form.get("driversLicenceProvince").setValue(provFound.provNm);
      this.form.get("driversLicenceCountryId").setValue(provFound.ctryId);
      this.form.get("driversLicenceProvinceSeqNo").setValue(provFound.provSeqNo);
      if (provFound.provAbbreviationCd === this.bc.provAbbreviationCd) {
        this.form.get('driversLicenceNumber').setValidators([Validators.maxLength(9)]);
        this.form.get('driversLicenceNumber').addValidators([Validators.minLength(7)]);
      } else {
        this.form.get('driversLicenceNumber').setValidators([Validators.maxLength(20)]);
      }
      if (provFound.ctryId == this.usa.ctryId || provFound.ctryId == this.canada.ctryId) {
        this.form.get('driversLicenceNumber').addValidators([Validators.required]);
      }
      this.form.get('driversLicenceNumber').updateValueAndValidity();
    }, 5)
  }

  public onAddressProvinceChange(provId: number) {
    setTimeout(() => {
      let provFound = this.config.provincesAndStates.filter(x => x.provId === provId).shift();
      this.form.get("addressProvince").setValue(provFound.provNm);
      this.form.get("addressProvinceCountryId").setValue(provFound.ctryId);
      this.form.get("addressProvinceSeqNo").setValue(provFound.provSeqNo);
    }, 0)
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
      this.form.get('contactLawFirmNm').addValidators([Validators.required]);
      this.form.get('contactSurnameNm').addValidators([Validators.required]);
      this.form.get('contactGivenNames').addValidators([Validators.required]);
    } else if (newContactType == this.ContactType.Individual) {
      // leave contact info null and not required
    } else {
      // only contact names required
      this.form.get('contactSurnameNm').addValidators([Validators.required]);
      this.form.get('contactGivenNames').addValidators([Validators.required]);
    }
    this.form.get('contactLawFirmNm').updateValueAndValidity();
    this.form.get('contactSurnameNm').updateValueAndValidity();
    this.form.get('contactGivenNames').updateValueAndValidity();
    this.form.updateValueAndValidity();
  }

  public onSubmit(): void {
    this.putDispute({ ...this.lastUpdatedDispute, ...this.form.value });
  }

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
          this.busy = this.disputeService.submitDispute(this.lastUpdatedDispute.disputeId).subscribe({
            next: response => {
              this.lastUpdatedDispute.status = this.DispStatus.Processing;
              this.onBack();
            },
            error: err => { },
            complete: () => { }
          });
        }
      });
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
              this.lastUpdatedDispute.status = this.DispStatus.Rejected;
              this.lastUpdatedDispute.rejectedReason = action.output.reason;
              this.onBack();
            },
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
      icon: "error_outline",
      message: this.form.get('rejectedReason').value
    };
    this.dialog.open(ConfirmReasonDialogComponent, { data }).afterClosed()
      .subscribe((action?: any) => {
      if (action?.output?.response) {
        this.form.get('rejectedReason').setValue(action.output.reason); // update on form for appearances
        this.lastUpdatedDispute.rejectedReason = action.output.reason; // update to send back on put

        // cancel dispute and return to TRM home since this will be filtered out
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

  // put dispute by id
  putDispute(dispute: Dispute): void {
    this.logger.log('ContactInfoComponent::putDispute', dispute);

    this.busy = this.disputeService.putDispute(dispute.disputeId, dispute).subscribe((response: Dispute) => {
      this.logger.info(
        'ContactInfoComponent::putDispute response',
        response
      );

      // this structure contains last version of what was send to db
      this.lastUpdatedDispute = response;
      this.form.markAsUntouched();
    });
  }

  // get dispute by id
  getDispute(): void {
    this.logger.log('ContactInfoComponent::getDispute');

    this.busy = this.disputeService.getDispute(this.disputeInfo.disputeId).subscribe((response: Dispute) => {
      this.retrieving = false;
      this.logger.info(
        'ContactInfoComponent::getDispute response',
        response
      );

      this.initialDisputeValues = response;
      this.lastUpdatedDispute = this.initialDisputeValues;

      // set violation date and time
      let tempViolationDate = this.lastUpdatedDispute?.issuedTs?.split("T");
      if (tempViolationDate) {
        this.violationDate = tempViolationDate[0];
        this.violationTime = tempViolationDate[1].split(":")[0] + ":" + tempViolationDate[1].split(":")[1];
      }

      // set provId for drivers Licence and address this field is only good client side as angular dropdown needs a single value key to behave well, doesnt like two part key of ctryid & seqno
      this.form.patchValue(this.initialDisputeValues);
      this.form.get('driversLicenceCountryId').setValue(this.initialDisputeValues.driversLicenceIssuedCountryId);
      this.form.get('driversLicenceProvinceSeqNo').setValue(this.initialDisputeValues.driversLicenceIssuedProvinceSeqNo);
      let provFound = this.config.provincesAndStates.filter(x => x.ctryId === this.initialDisputeValues.driversLicenceIssuedCountryId && x.provSeqNo === this.initialDisputeValues.driversLicenceIssuedProvinceSeqNo).shift();
      if (provFound) {
        this.form.get('driversLicenceProvinceProvId').setValue(provFound.provId);
      }

      provFound = this.config.provincesAndStates.filter(x => x.ctryId === this.initialDisputeValues.addressProvinceCountryId && x.provSeqNo === this.initialDisputeValues.addressProvinceSeqNo).shift();
      if (provFound) this.form.get('addressProvinceProvId').setValue(provFound.provId);
      this.form.get('addressProvince').setValidators([Validators.maxLength(30)]);
      this.form.get('homePhoneNumber').setValidators([Validators.maxLength(20)]);
      this.form.get('driversLicenceProvince').setValidators([Validators.maxLength(30)]);
      this.form.get("driversLicenceProvinceSeqNo").setValidators(null);

      if (this.form.get('addressCountryId').value === this.canada.ctryId || this.form.get('addressCountryId').value === this.usa.ctryId) {
        this.form.get('addressProvinceSeqNo').addValidators([Validators.required]);
        this.form.get('postalCode').addValidators([Validators.required]);
        this.form.get('homePhoneNumber').addValidators([Validators.required, FormControlValidators.phone]);
        this.form.get('driversLicenceProvinceSeqNo').addValidators([Validators.required]);
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
    });
  }

  public onBack() {
    this.backInbox.emit();
  }

  public handleCollapse(name: string) {
    this.collapseObj[name] = !this.collapseObj[name]
  }
}
