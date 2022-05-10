import { Component, OnInit, ViewChild } from "@angular/core";
import { FormArray, FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { MatStepper } from "@angular/material/stepper";
import { ActivatedRoute, Router } from "@angular/router";
import { ConfigService } from "@config/config.service";
import { UtilsService } from "@core/services/utils.service";
import { FormControlValidators } from "@core/validators/form-control.validators";
import { TranslateService } from "@ngx-translate/core";
import { ConfirmDialogComponent } from "@shared/dialogs/confirm-dialog/confirm-dialog.component";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { Address } from "@shared/models/address.model";
import { DisputesService, NoticeOfDispute, ViolationTicket, Plea } from "app/api";
import { ticketTypes } from "@shared/enums/ticket-type.enum";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { Subscription } from "rxjs";
import { TicketTypePipe } from "@shared/pipes/ticket-type.pipe";
import { FormGroupValidators } from "@core/validators/form-group.validators";
import { MatCheckboxChange } from "@angular/material/checkbox";
import { FormUtilsService } from "@core/services/form-utils.service";
import { ToastService } from "@core/services/toast.service";
import { AppRoutes } from "app/app.routes";

@Component({
  selector: "app-dispute-ticket-stepper",
  templateUrl: "./dispute-ticket-stepper.component.html",
  styleUrls: ["./dispute-ticket-stepper.component.scss"],
})
export class DisputeTicketStepperComponent implements OnInit {
  @ViewChild(MatStepper) private stepper: MatStepper;

  public busy: Subscription;
  public isMobile: boolean;
  public previousButtonIcon = "keyboard_arrow_left";
  public defaultLanguage: string;
  public ticketTypesEnum = ticketTypes;
  public Plea = Plea;

  public form: FormGroup;
  public legalRepresentationForm: FormGroup;
  public countForms: FormArray;
  public ticket: ViolationTicket;
  public ticketType;

  // Disputant
  public provinces = this.config.provinces;
  private MINIMUM_AGE = 18;
  public maxDateOfBirth: Date;
  public showManualButton: boolean = false; // temporary preset for testing
  public showAddressFields: boolean = true; // temporary preset for testing

  // Additional
  public languages = this.config.languages;
  public isShowCheckbox: any;
  public isErrorCheckMsg1: string;
  public countDataList: any;
  public customWitnessOption = false;
  public minWitnesses = 1;
  public maxWitnesses = 9;

  // Overview
  public declared = false;

  private ticketFormFields = {
    surname: [null, [Validators.required]],
    given_names: [null, [Validators.required]],
    address: [null, [Validators.required]],
    city: [null],
    province: [null],
    country: [{ value: "Canada", disabled: true }],
    postal_code: [null],
    home_phone_number: [null, [FormControlValidators.phone]],
    work_phone_number: [null, [FormControlValidators.phone]],
    email_address: [null, [Validators.required, Validators.email]],
    birthdate: [null],
    drivers_licence_number: [null],
    drivers_licence_province: [null],
    disputed_counts: [],
    legal_representation: []
  }

  private countFormFields = {
    plea: null,
    request_time_to_pay: false,
    request_reduction: false,
    appear_in_court: null,
  }

  private countFormSetting = {
    __skip: false,
    __apply_to_remaining_counts: false,
  };

  private additionFormFields = {
    represented_by_lawyer: false,
    interpreter_language: null,
    number_of_witness: null,
    fine_reduction_reason: null,
    time_to_pay_reason: null,

    __witness_present: false,
    __interpreter_required: false,
  }

  private additionFormValidators = [
    FormGroupValidators.requiredIfTrue("__interpreter_required", "interpreter_language"),
    FormGroupValidators.requiredIfTrue("__witness_present", "number_of_witness"),
  ]

  // private legalRepresentationFields = {
  //   law_firm_name: ["null", [Validators.required]],
  //   lawyer_full_name: ["null", [Validators.required]],
  //   lawyer_email: ["null@t.ca", [Validators.required, Validators.email]],
  //   lawyer_phone: [null, [Validators.required]],
  //   lawyer_address: ["null", [Validators.required]],
  // }

  private legalRepresentationFields = {
    law_firm_name: [null, [Validators.required]],
    lawyer_full_name: [null, [Validators.required]],
    lawyer_email: [null, [Validators.required, Validators.email]],
    lawyer_phone: [null, [Validators.required]],
    lawyer_address: [null, [Validators.required]],
  }

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected violationTicketService: ViolationTicketService,
    protected disputesService: DisputesService,
    private utilsService: UtilsService,
    private formUtilsService: FormUtilsService,
    private translateService: TranslateService,
    private toastService: ToastService,
    private config: ConfigService,
    private ticketTypePipe: TicketTypePipe,
    private dialog: MatDialog,
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
      disputed_counts: this.countForms
    }, {
      validators: [...this.additionFormValidators]
    });

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
    this.isShowCheckbox.request_counts =
      [...this.isShowCheckbox.request_time_to_pay, ...this.isShowCheckbox.request_reduction]
        .filter((value, index, self) => { return self.indexOf(value) === index; }).sort();
    this.isShowCheckbox.not_guilty = this.countForms.value.filter(i => i.plea === Plea.NotGuilty).map(i => i.count);
  }

  public onAddressAutocomplete({ countryCode, provinceCode, postalCode, address, city }: Address): void {
    // Will be implemented
  }

  public onStepCancel(): void {
    this.violationTicketService.goToDisputeSummary();
  }

  public onAttendHearingChange(form: FormGroup, event): void {
    form.patchValue({ ...this.countFormFields, appear_in_court: event.value, __skip: false });
  }

  public onStepSave(countInx?, applyToRemaining?): void {
    let isLast = this.stepper.steps.length === this.stepper.selectedIndex + 1;
    let isValid = this.formUtilsService.checkValidity(this.form)
      && (!this.legalRepresentationForm || this.formUtilsService.checkValidity(this.legalRepresentationForm));

    if (countInx !== undefined) {
      let countForm = this.countForms.controls[countInx];
      if (countForm.value.__skip) {
        countForm.patchValue({ ...this.getCountFormValue(this.ticket.counts[countInx]), __skip: true });
      }
      if (applyToRemaining && countInx + 1 < this.countForms.length) {
        let value = this.countForms.controls[countInx].value;
        for (let i = countInx; i < this.countForms.length; i++) {
          this.countForms.controls[i].patchValue({ ...value, ...this.ticket.counts[i], __apply_to_remaining_counts: false })
        }
      }
      this.setAdditional();
    } else if (!isValid) {
      this.utilsService.scrollToErrorSection();
      if (isLast) {
        this.toastService.openErrorToast(this.config.dispute_validation_error);
      }
      return;
    }

    if (isLast) {
      this.submitDispute();
    } else {
      this.stepper.next();
    }
  }

  private setAdditional() {
    this.setCheckBoxes();
    this.form.patchValue(this.additionFormFields);
  }

  public onChangeRepresentedByLawyer(event: MatCheckboxChange) {
    if (event.checked) { // only append if selected
      this.legalRepresentationForm = this.formBuilder.group(this.legalRepresentationFields);
    } else {
      this.legalRepresentationForm = null;
    }
  }

  public onChangeWitnessPresent(event: MatCheckboxChange) {
    if (event.checked) {
      this.form.controls.number_of_witness.setValidators([Validators.min(this.minWitnesses), Validators.max(this.maxWitnesses), Validators.required]);
    } else {
      this.form.controls.number_of_witness.clearValidators();
      this.form.controls.number_of_witness.updateValueAndValidity();
    }
  }

  /**
   * @description
   * Submit the dispute
   */
  private submitDispute(): void {
    const data: DialogOptions = {
      titleKey: "Submit request",
      messageKey:
        "When your request is submitted for adjudication, it can no longer be updated. Are you ready to submit your request?",
      actionTextKey: "Submit request",
      cancelTextKey: "Cancel",
      icon: null,
    };
    this.dialog.open(ConfirmDialogComponent, { data }).afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          this.busy = this.disputesService.apiDisputesCreatePost(<NoticeOfDispute>{
            ...this.ticket,
            ...this.form.value,
            legal_representation: this.legalRepresentationForm.value
          }).subscribe(res => {
            let test;
            // this.router.navigate([
            //   AppRoutes.disputePath(AppRoutes.SUBMIT_SUCCESS),
            // ], {
            //   queryParams: this.countDataList,
            // });
          })
        }
      });
  }
}
