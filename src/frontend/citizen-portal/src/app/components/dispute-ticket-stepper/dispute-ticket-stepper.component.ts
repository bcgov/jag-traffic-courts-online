import { Component, OnInit, ViewChild } from "@angular/core";
import { FormArray, FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { MatStepper } from "@angular/material/stepper";
import { ActivatedRoute, Router } from "@angular/router";
import { ConfigService } from "@config/config.service";
import { UtilsService } from "@core/services/utils.service";
import { FormControlValidators } from "@core/validators/form-control.validators";
import { TranslateService } from "@ngx-translate/core";
import { Address } from "@shared/models/address.model";
import { NoticeOfDispute, ViolationTicket, Plea } from "app/api";
import { ticketTypes } from "@shared/enums/ticket-type.enum";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { Subscription } from "rxjs";
import { TicketTypePipe } from "@shared/pipes/ticket-type.pipe";
import { FormGroupValidators } from "@core/validators/form-group.validators";
import { MatCheckboxChange } from "@angular/material/checkbox";
import { FormUtilsService } from "@core/services/form-utils.service";
import { ToastService } from "@core/services/toast.service";
import { AppRoutes } from "app/app.routes";
import { NoticeOfDisputeService } from "app/services/notice-of-dispute.service";

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
  public noticeOfDispute: NoticeOfDispute;
  public ticketType;

  // Disputant
  public provinces = this.config.provinces;
  private MINIMUM_AGE = 18;
  public maxDateOfBirth: Date;
  public showManualButton: boolean = true;
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
  }

  private countFormFields = this.noticeOfDisputeService.countFormFields;

  private countFormSetting = {
    __skip: false,
    __apply_to_remaining_counts: false,
  };

  private additionFormFields = {
    represented_by_lawyer: false,
    interpreter_language: null,
    number_of_witness: 0,
    fine_reduction_reason: null,
    time_to_pay_reason: null,

    __witness_present: false,
    __interpreter_required: false,
  }

  private additionFormValidators = [
    FormGroupValidators.requiredIfTrue("__interpreter_required", "interpreter_language"),
    FormGroupValidators.requiredIfTrue("__witness_present", "number_of_witness"),
  ]

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
    protected noticeOfDisputeService: NoticeOfDisputeService,
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
      let obj = this.getCountFormInitValue(count);
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
    this.legalRepresentationForm = this.formBuilder.group(this.legalRepresentationFields);
  }

  private getCountFormInitValue(count) {
    return { ...this.countFormFields, ...this.countFormSetting, ...count };
  }

  private setCheckBoxes() {
    this.isShowCheckbox = this.noticeOfDisputeService.getIsShowCheckBoxes(this.form.value);
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
    let isSummary = this.stepper.steps.length === this.stepper.selectedIndex + 2;
    let isLast = this.stepper.steps.length === this.stepper.selectedIndex + 1;
    let isValid = this.formUtilsService.checkValidity(this.form);

    if (countInx !== undefined) {
      let countForm = this.countForms.controls[countInx];
      if (countForm.value.request_time_to_pay || countForm.value.request_reduction) {
        countForm.patchValue({ plea: Plea.Guilty });
      }
      if (countForm.value.__skip) {
        countForm.patchValue({ ...this.getCountFormInitValue(this.ticket.counts[countInx]), __skip: true });
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

    if (isSummary) {
      this.noticeOfDispute = <NoticeOfDispute>{ ...this.ticket, ...this.form.value };
    } else {
      this.noticeOfDispute = null;
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
      this.form.addControl("legal_representation", this.legalRepresentationForm);
    } else {
      this.form.removeControl("legal_representation");
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
    // this.busy = this.violationTicketService.createNoticeOfDispute(input); 
    this.noticeOfDisputeService.createNoticeOfDispute(this.noticeOfDispute);
  }
}
