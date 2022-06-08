import { Component, EventEmitter, Input, OnInit, Output, Inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { ProvinceConfig, Config } from '@config/config.model';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { Dispute, DisputeService } from '../../services/dispute.service';
import { Subscription } from 'rxjs';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ConfirmReasonDialogComponent } from '@shared/dialogs/confirm-reason-dialog/confirm-reason-dialog.component';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
@Component({
  selector: 'app-contact-info',
  templateUrl: './contact-info.component.html',
  styleUrls: ['./contact-info.component.scss', '../../app.component.scss']
})
export class ContactInfoComponent implements OnInit {
  @Input() public disputeInfo: Dispute;
  @Output() public backTicketList: EventEmitter<any> = new EventEmitter();
  public isMobile: boolean;
  public provinces: ProvinceConfig[];
  public states: ProvinceConfig[];
  public busy: Subscription;
  public initialDisputeValues: Dispute;
  public todayDate: Date = new Date();
  public lastUpdatedDispute: Dispute;
  public retrieving: boolean = true;
  public violationDate: string = "";
  public violationTime: string = "";
  public conflict: boolean = false;
  public form: FormGroup;
  public collapseObj: any = {
    contactInformation: true
  }

  constructor(
    protected route: ActivatedRoute,
    protected formBuilder: FormBuilder,
    private dialog: MatDialog,
    private utilsService: UtilsService,
    public mockConfigService: MockConfigService,
    private datePipe: DatePipe,
    private disputeService: DisputeService,
    private logger: LoggerService,
    @Inject(Router) private router,
  ) {
    const today = new Date();
    this.isMobile = this.utilsService.isMobile();

    if (this.mockConfigService.provinces) {
      this.provinces = this.mockConfigService.provinces.filter(x => x.countryCode == 'CA' && x.code != 'BC');
      this.states = this.mockConfigService.provinces.filter(x => x.countryCode == 'US');
    }
  }

  public ngOnInit() {
    this.form = this.formBuilder.group({
      ticketNumber: [null, [Validators.required]],
      homePhoneNumber: [null, [Validators.required, Validators.maxLength(20)]],
      emailAddress: [null, [Validators.email, Validators.required]],
      surname: [null, [Validators.required]],
      givenNames: [null, [Validators.required]],
      birthdate: [null, [Validators.required]],
      address: [null, [Validators.required]],
      city: [null, [Validators.required]],
      province: [null, [Validators.required, Validators.maxLength(30)]],
      rejectedReason: [null, Validators.maxLength(256)], // Optional
      country: [null, [Validators.required]],
      postalCode: [null, [Validators.required, Validators.maxLength(6), Validators.minLength(6)]],
      driversLicenceNumber: [null, [Validators.required, Validators.maxLength(20)]],
      driversLicenceProvince: [null, [Validators.required, Validators.maxLength(30)]],
    });
    this.getDispute();
  }

  public onCountryChange(country) {
    setTimeout(() => {
      this.form.get('postalCode').setValidators([Validators.maxLength(6)]);
      this.form.get('province').setValidators([Validators.maxLength(30)]);
      this.form.get('homePhoneNumber').setValidators([Validators.maxLength(20)]);
      this.form.get('driversLicenceNumber').setValidators([Validators.maxLength(20)]);
      this.form.get('driversLicenceProvince').setValidators([Validators.maxLength(30)]);

      if (country == 'Canada' || country == 'United States') {
        this.form.get('province').addValidators([Validators.required]);
        this.form.get('postalCode').addValidators([Validators.required]);
        this.form.get('homePhoneNumber').addValidators([Validators.required, FormControlValidators.phone]);
        this.form.get('driversLicenceNumber').addValidators([Validators.required]);
        this.form.get('driversLicenceProvince').addValidators([Validators.required]);
      }

      if (country == 'Canada') {
        this.form.get('postalCode').addValidators([Validators.minLength(6)]);
      }

      this.form.get('postalCode').updateValueAndValidity();
      this.form.get('province').updateValueAndValidity();
      this.form.get('homePhoneNumber').updateValueAndValidity();
      this.form.get('driversLicenceNumber').updateValueAndValidity();
      this.form.get('driversLicenceProvince').updateValueAndValidity();
    }, 5);
  }

  public onDLProvinceChange(province) {
    setTimeout(() => {
      if (province == 'BC') {
        this.form.get('driversLicenceNumber').setValidators([Validators.maxLength(9)])
      } else {
        this.form.get('driversLicenceNumber').setValidators([Validators.maxLength(20)]);
      }
      if (this.form.get('country').value == 'United States' || this.form.get('country').value == 'Canada') {
        this.form.get('driversLicenceNumber').addValidators([Validators.required]);
      }
      this.form.get('driversLicenceNumber').updateValueAndValidity();
    }, 5)
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
      actionType: "warn",
      cancelTextKey: "Go back",
      icon: "error_outline",
    };
    this.lastUpdatedDispute.status = 'PROCESSING';
    this.lastUpdatedDispute.rejectedReason = this.form.get('rejectedReason').value;
    this.dialog.open(ConfirmDialogComponent, { data }).afterClosed()
      .subscribe((action: any) => {
        if (action) {
          // submit dispute and return to TRM home
          this.busy = this.disputeService.submitDispute(this.lastUpdatedDispute.id).subscribe({
            next: response => { this.onBack(); },
            error: err => { },
            complete: () => { }
          });
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
      message: this.form.get('rejectedReason').value 
    };
    this.dialog.open(ConfirmReasonDialogComponent, { data }).afterClosed()
      .subscribe((action?: any) => {
        if (action?.output?.response) {
          this.form.get('rejectedReason').setValue(action.output.reason); // update on form for appearances
          this.lastUpdatedDispute.rejectedReason = action.output.reason; // update to send back on put

          // udate the reason entered, reject dispute and return to TRM home 
          this.busy = this.disputeService.rejectDispute(this.lastUpdatedDispute.id, this.lastUpdatedDispute.rejectedReason).subscribe({
            next: response => { 
              this.lastUpdatedDispute.status = 'REJECTED';
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

          // udate the reason entered, cancel dispute and return to TRM home since this will be filtered out
          this.busy = this.disputeService.putDispute(this.lastUpdatedDispute.id, this.lastUpdatedDispute).subscribe({
            next: response => {
              this.disputeService.cancelDispute(this.lastUpdatedDispute.id).subscribe({
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
    this.logger.log('TicketInfoComponent::putDispute', dispute);

    this.busy = this.disputeService.putDispute(dispute.id, dispute).subscribe((response: Dispute) => {
      this.logger.info(
        'TicketInfoComponent::putDispute response',
        response
      );

      // this structure contains last version of what was send to db
      this.lastUpdatedDispute = response;
      this.form.markAsUntouched();
    });
  }

  // get dispute by id
  getDispute(): void {
    this.logger.log('TicketInfoComponent::getDispute');

    this.busy = this.disputeService.getDispute(this.disputeInfo.id).subscribe((response: Dispute) => {
      this.retrieving = false;
      this.logger.info(
        'TicketInfoComponent::getDispute response',
        response
      );

      this.initialDisputeValues = response;
      this.lastUpdatedDispute = this.initialDisputeValues;

      // set violation date and time
      let tempViolationDate = new Date(this.lastUpdatedDispute.issuedDate);
      this.violationDate = this.datePipe.transform(tempViolationDate, "yyyy-MM-dd");
      this.violationTime = this.datePipe.transform(tempViolationDate, "hh:mm");

      this.form.patchValue(this.initialDisputeValues);

      // set country from province
      if (this.provinces.filter(x => x.name == this.lastUpdatedDispute.province || this.lastUpdatedDispute.province == "British Columbia").length > 0) this.form.get('country').setValue("Canada");
      else if (this.states.filter(x => x.name == this.initialDisputeValues.province).length > 0) this.form.get('country').setValue("United States");
      else this.form.get('country').setValue("International");

      this.onCountryChange(this.form.get('country').value);
      this.onDLProvinceChange(this.lastUpdatedDispute.driversLicenceProvince);
    });
  }

  public onBack() {
    this.backTicketList.emit();
  }

  public handleCollapse(name: string) {
    this.collapseObj[name] = !this.collapseObj[name]
  }

}
