import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigService } from '@config/config.service';
import { UtilsService } from '@core/services/utils.service';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { Address } from '@shared/models/address.model';
import { ViolationTicket } from 'app/api';
import { ticketTypes } from '@shared/enums/ticket-type.enum';
import { ViolationTicketService } from 'app/services/violation-ticket.service';
import { Subscription } from 'rxjs';
import { TicketTypePipe } from '@shared/pipes/ticket-type.pipe';
import { FormGroupValidators } from '@core/validators/form-group.validators';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';

@Component({
  selector: 'app-dispute-ticket-stepper',
  templateUrl: './dispute-ticket-stepper.component.html',
  styleUrls: ['./dispute-ticket-stepper.component.scss'],
})
export class DisputeTicketStepperComponent implements OnInit {
  @ViewChild(MatStepper) private stepper: MatStepper;

  public busy: Subscription;
  public isMobile: boolean;
  public previousButtonIcon = 'keyboard_arrow_left';
  public defaultLanguage: string;
  public ticketTypesEnum = ticketTypes;

  public form: FormGroup;
  public legalRepresentationForm: FormGroup;
  public countForms: FormArray;
  public ticket: ViolationTicket;
  public ticketType;

  // Disputant
  public provinces = this.config.provinces;
  private MINIMUM_AGE = 18;
  public maxDateOfBirth: Date;
  public showManualButton: boolean = true;
  public showAddressFields: boolean = false;

  // Additional
  public languages = this.config.languages;
  public isShowCheckbox: any;
  public isErrorCheckMsg1: string;
  public countDataList: any;
  public customWitnessOption = false;

  // Overview
  public declared = false;

  // private ticketFormFields = {
  //   disputantSurname: [null, [Validators.required]],
  //   givenNames: [null, [Validators.required]],
  //   streetAddress: [null, [Validators.required]],
  //   postalCode: [null],
  //   city: [null],
  //   country: [null],
  //   province: [null],
  //   driversLicence: [null],
  //   driversLicenceProvince: [null],
  //   emailAddress: [null, [Validators.required, Validators.email]],
  //   homePhone: [null, [FormControlValidators.phone]],
  //   dateOfBirth: [null, []],
  //   ticketCounts: []
  // }

  private ticketFormFields = {
    disputantSurname: ["null", [Validators.required]],
    givenNames: ["null", [Validators.required]],
    streetAddress: ["null", [Validators.required]],
    postalCode: [null],
    city: [null],
    country: [null],
    province: [null],
    driversLicence: [null],
    driversLicenceProvince: [null],
    emailAddress: ["null@x", [Validators.required, Validators.email]],
    homePhone: [null, [FormControlValidators.phone]],
    dateOfBirth: [null, []],
    ticketCounts: []
  }

  private countFormFields = {
    offenceDeclaration: null,
    timeToPayRequest: false,
    fineReductionRequest: false,
    appearInCourt: null,
  }

  private countFormSetting = {
    __skip: false,
    __applyToRemainingCounts: false,
  };

  private additionFormFields = {
    representedByLawyer: false,
    interpreterLanguage: null,
    numberOfWitnesses: null,
    // requestReduction: false,
    // requestMoreTime: false,
    reductionReason: null,
    moreTimeReason: null,

    __witnessPresent: false,
    __isCourtRequired: false,
    __isReductionRequired: false,
    __interpreterRequired: false,
  }

  private additionFormValidators = [
    FormGroupValidators.requiredIfTrue('interpreterRequired', 'interpreterLanguage'),
  ]

  private legalRepresentationFields = {
    lawFirmName: [null],
    lawyerName: [null],
    lawyerSurname: [null],
    lawyerEmail: [null],
    lawyerPhone: [null],
    lawyerAddress: [null],
  }

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected violationTicketService: ViolationTicketService,
    private utilsService: UtilsService,
    private formUtilsService: FormUtilsService,
    private translateService: TranslateService,
    private config: ConfigService,
    private ticketTypePipe: TicketTypePipe,
    private dialog: MatDialog,
    private logger: LoggerService,
    private toastService: ToastService,
  ) {
    // config or static
    this.maxDateOfBirth = new Date();
    this.maxDateOfBirth.setFullYear(this.maxDateOfBirth.getFullYear() - this.MINIMUM_AGE);
    this.isMobile = this.utilsService.isMobile();
    this.defaultLanguage = this.translateService.getDefaultLang();
  }

  public ngOnInit(): void {
    this.ticket = this.violationTicketService.ticket
    if (!this.ticket) {
      this.violationTicketService.goToFind();
      return;
    }
    this.ticketType = this.ticketTypePipe.transform(this.ticket.ticket_number.charAt(0));

    // build inner object array before the form
    let countArray = [];
    this.ticket.counts.forEach(count => {
      let obj = this.getCountFormValue(count);
      countArray.push(this.formBuilder.group(obj));
    })
    this.countForms = this.formBuilder.array(countArray);

    // build form
    this.form = this.formBuilder.group({
      ...this.ticketFormFields,
      ...this.additionFormFields,
      ticketCounts: this.countForms
    }, this.additionFormValidators);

    this.setAdditional();
  }

  private getCountFormValue(count) {
    return { ...this.countFormFields, ...this.countFormSetting, ...count };
  }

  private setCheckBoxes() {
    this.isShowCheckbox = {};
    let fields = Object.keys(this.countFormFields);
    fields.forEach(field => {
      if (this.countForms && this.countForms.value && this.countForms.value.length > -1) {
        this.isShowCheckbox[field] = this.countForms.value.filter(i => i[field]).map(i => i.count);
      } else {
        this.isShowCheckbox[field] = [];
      }
    });
    this.isShowCheckbox.requestCounts =
      [...this.isShowCheckbox.timeToPayRequest, ...this.isShowCheckbox.fineReductionRequest]
        .filter((value, index, self) => { return self.indexOf(value) === index; }).sort();
  }

  public onAddressAutocomplete({ countryCode, provinceCode, postalCode, address, city }: Address): void {
    // Will be implemented
  }

  public onStepCancel(): void {
    this.violationTicketService.goToDisputeSummary();
  }

  public onAttendHearingChange(form: FormGroup, event): void {
    form.patchValue({ ...this.countFormFields, appearInCourt: event.value });
  }

  public onStepSave(countInx?, applyToRemaining?): void {
    if (countInx !== undefined) {
      let countForm = this.countForms.controls[countInx];
      if (countForm.value.__skip) {
        countForm.patchValue({ ...this.getCountFormValue(this.ticket.counts[countInx]), __skip: true });
      }
      if (applyToRemaining && countInx + 1 < this.countForms.length) {
        let value = this.countForms.controls[countInx].value;
        for (let i = countInx; i < this.countForms.length; i++) {
          this.countForms.controls[i].patchValue({ ...value, ...this.ticket.counts[i], __applyToRemainingCounts: false })
        }
      }
      this.setAdditional();
    } else if (!this.formUtilsService.checkValidity(this.form)) {
      this.utilsService.scrollToErrorSection();
      // this.toastService.openErrorToast(this.config.dispute_validation_error);
      return;
    }

    if (this.stepper.steps.length === this.stepper.selectedIndex + 1) {
      this.submitDispute();
    } else {
      this.stepper.next();
    }
  }

  setAdditional() {
    this.setCheckBoxes();
    this.form.patchValue(this.additionFormFields);
    this.legalRepresentationForm = this.formBuilder.group(this.legalRepresentationFields);
  }

  public onChangeCallWitnesses(event: MatCheckboxChange) {
    if (event.checked) {
      this.form.controls.numberOfWitnesses.setValidators([Validators.min(0), Validators.required]);
    } else {
      this.form.controls.numberOfWitnesses.clearValidators();
      this.form.controls.numberOfWitnesses.updateValueAndValidity();
    }
  }

  /**
   * @description
   * Submit the dispute
   */
  private submitDispute(): void {
    const data: DialogOptions = {
      titleKey: 'Submit request',
      messageKey:
        'When your request is submitted for adjudication, it can no longer be updated. Are you ready to submit your request?',
      actionTextKey: 'Submit request',
      cancelTextKey: 'Cancel',
      icon: null,
    };
    this.dialog.open(ConfirmDialogComponent, { data }).afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          // const payload = this.disputeFormStateService.jsonTicketDispute;
          // payload.violationTicketNumber = this.ticket.violationTicketNumber;
          // payload.violationTime = this.ticket.violationTime;

          // this.busy = this.disputeResource
          //   .createTicketDispute(payload)
          //   .subscribe((newDisputeTicket: TicketDisputeView) => {
          //     // newDisputeTicket.additional = {
          //     //   _isCourtRequired:true,
          //     //   _isReductionRequired: true,
          //     //   _isReductionNotInCourt: true
          //     // }
          //     newDisputeTicket.countList = this.countDataList;
          //     this.disputeService.ticket$.next(newDisputeTicket);

          //     this.router.navigate([
          //       AppRoutes.disputePath(AppRoutes.SUBMIT_SUCCESS),
          //     ], {
          //       queryParams: this.countDataList,
          //     });
          //   });
        }
      });
  }
}
