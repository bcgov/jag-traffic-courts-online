import { AfterViewInit, Component, OnInit, ViewChild, ChangeDetectionStrategy } from "@angular/core";
import { FormArray, FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatStepper } from "@angular/material/stepper";
import { ActivatedRoute, Router } from "@angular/router";
import { ConfigService } from "@config/config.service";
import { UtilsService } from "@core/services/utils.service";
import { TranslateService } from "@ngx-translate/core";
import { Address } from "@shared/models/address.model";
import { FormControlValidators } from "@core/validators/form-control.validators";
import { NoticeOfDispute, ViolationTicket, Plea } from "app/api";
import { ticketTypes } from "@shared/enums/ticket-type.enum";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { Subscription } from "rxjs";
import { MatCheckboxChange } from "@angular/material/checkbox";
import { FormUtilsService } from "@core/services/form-utils.service";
import { ToastService } from "@core/services/toast.service";
import { NoticeOfDisputeService } from "app/services/notice-of-dispute.service";
import { AddressAutocompleteComponent } from "@shared/components/address-autocomplete/address-autocomplete.component";
import { ProvinceConfig } from "@config/config.model";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { ConfirmDialogComponent } from "@shared/dialogs/confirm-dialog/confirm-dialog.component";
import { MatDialog } from "@angular/material/dialog";
import { FormErrorStateMatcher } from "@shared/directives/form-error-state-matcher.directive";

@Component({
  selector: "app-dispute-ticket-stepper",
  templateUrl: "./dispute-ticket-stepper.component.html",
  styleUrls: ["./dispute-ticket-stepper.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DisputeTicketStepperComponent implements OnInit, AfterViewInit {
  @ViewChild(MatStepper) private stepper: MatStepper;
  @ViewChild(AddressAutocompleteComponent) private addressAutocomplete: AddressAutocompleteComponent;

  public busy: Subscription;
  public isMobile: boolean;
  public previousButtonIcon = "keyboard_arrow_left";
  public defaultLanguage: string;
  public ticketTypes = ticketTypes;
  public todayDate: Date = new Date();
  public Plea = Plea;
  public selected = null;

  public form: FormGroup;
  public countForms: FormArray;
  public additionalForm: FormGroup;
  public legalRepresentationForm: FormGroup;
  public ticket: ViolationTicket;
  public noticeOfDispute: NoticeOfDispute;
  public ticketType;
  public matcher = new FormErrorStateMatcher();

  // Disputant
  public showManualButton: boolean = true;
  public showAddressFields: boolean = true; // temporary preset for testing

  // Count
  public countIndexes: number[];

  // Additional
  public languages = this.config.languages;
  public countsActions: any;
  public customWitnessOption = false;
  public minWitnesses = 1;
  public maxWitnesses = 99;
  public additionalIndex: number;
  public provinces: ProvinceConfig[];
  public states: ProvinceConfig[];

  // Summary
  public declared = false;

  // Consume from the service
  private ticketFormFields = this.noticeOfDisputeService.ticketFormFields;
  private countFormFields = this.noticeOfDisputeService.countFormFields;
  private countFormDefaultValue = this.noticeOfDisputeService.countFormDefaultValue;
  private additionFormFields = this.noticeOfDisputeService.additionFormFields;
  private additionFormValidators = this.noticeOfDisputeService.additionFormValidators;
  private additionFormDefaultValue = this.noticeOfDisputeService.additionFormDefaultValue;
  private legalRepresentationFields = this.noticeOfDisputeService.legalRepresentationFields;

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
    public config: ConfigService,
    private dialog: MatDialog,
  ) {
    // config or static
    this.isMobile = this.utilsService.isMobile();
    this.defaultLanguage = this.translateService.getDefaultLang();
  }

  public ngOnInit(): void {
    this.ticket = this.violationTicketService.ticket;
    if (!this.ticket) {
      this.violationTicketService.goToFind();
      return;
    }
    this.ticketType = this.violationTicketService.ticketType;

    this.provinces = this.config.provinces.filter(x => x.countryCode == 'CA' && x.code != 'BC');
    this.states = this.config.provinces.filter(x => x.countryCode == 'US');

    // build inner object array before the form
    let countArray = [];
    this.ticket.counts.forEach(count => {
      countArray.push(this.formBuilder.group({ ...count, ...this.countFormFields }));
    })
    this.countForms = this.formBuilder.array(countArray);

    // build form
    this.form = this.formBuilder.group({
      ...this.ticketFormFields,
    });
    this.form.get('country').setValue('Canada');

    // take info from ticket, convert dl number to string
    Object.keys(this.ticket).forEach(key => {
      this.ticket[key] && this.form.get(key)?.patchValue(this.ticket[key]);
    });
    this.ticket.drivers_licence_number && this.form.controls['drivers_licence_number'].setValue(this.ticket.drivers_licence_number.toString());
    this.legalRepresentationForm = this.formBuilder.group(this.legalRepresentationFields);

    this.countIndexes = this.ticket.counts.map(i => i.count);
    let lastCountInx = this.countIndexes[this.countIndexes.length - 1]
    this.additionalIndex = lastCountInx + 1;
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.addressAutocomplete?.autocomplete.disable(); // disable auto complete component
      this.stepper?.selectionChange.subscribe(event => { // stepper change subscription
        if (event.previouslySelectedIndex < event.selectedIndex) {
          this.onStepSave();
        }
        this.scrollToSectionHook(); // correct angular faulty vertical scrolling
      })
    }, 0)
  }

  // make sure to scroll to top of mat-step
  // angular doesnt do this right on its own :) https://github.com/angular/components/issues/8881
  private scrollToSectionHook() {
    const stepId = this.stepper._getStepLabelId(this.stepper.selectedIndex);
    const stepElement = document.getElementById(stepId);
    if (stepElement) {
      setTimeout(() => {
        stepElement.scrollIntoView({ block: 'start', inline: 'nearest', behavior: 'smooth' });
      }, 250);
    }
  }

  public onCountryChange(country) {

    setTimeout(() => {
      this.form.get('postal_code').setValidators([Validators.maxLength(6)]);
      this.form.get('province').setValidators([Validators.maxLength(30)]);
      this.form.get('home_phone_number').setValidators([Validators.maxLength(20)]);
      this.form.get('drivers_licence_number').setValidators([Validators.maxLength(20)]);
      this.form.get('drivers_licence_province').setValidators([Validators.maxLength(30)]);

      if (country == 'Canada' || country == 'United States') {
        this.form.get('province').addValidators([Validators.required]);
        this.form.get('postal_code').addValidators([Validators.required]);
        this.form.get('home_phone_number').addValidators([Validators.required, FormControlValidators.phone]);
        this.form.get('drivers_licence_number').addValidators([Validators.required]);
        this.form.get('drivers_licence_province').addValidators([Validators.required]);
      }

      this.form.get('postal_code').updateValueAndValidity();
      this.form.get('province').updateValueAndValidity();
      this.form.get('home_phone_number').updateValueAndValidity();
      this.form.get('drivers_licence_number').updateValueAndValidity();
      this.form.get('drivers_licence_province').updateValueAndValidity();
    }, 0);
  }

  public onDLProvinceChange(province) {

    setTimeout(() => {
      if (province == 'BC') {
        this.form.get('drivers_licence_number').setValidators([Validators.maxLength(9)])
      } else {
        this.form.get('drivers_licence_number').setValidators([Validators.maxLength(20)]);
      }
      if (this.form.get('country').value === 'United States' || this.form.get('country').value === 'Canada') {
        this.form.get('drivers_licence_number').addValidators([Validators.required]);
      }
      this.form.get('drivers_licence_number').updateValueAndValidity();
    }, 0)
  }

  private getCountFormInitValue(count) {
    return { ...this.countFormDefaultValue, ...count };
  }

  private getCountsActions() {
    this.countsActions = this.noticeOfDisputeService.getCountsActions(this.countForms.value);
  }

  public onAddressAutocomplete({ countryCode, provinceCode, postalCode, address, city }: Address): void {
    // Will be implemented
  }

  public onStepCancel(): void {
    this.violationTicketService.goToInitiateResolution();
  }

  public onAttendHearingChange(countForm: FormGroup, event): void {
    countForm.patchValue({ ...this.countFormDefaultValue, appear_in_court: event.value, __skip: false });
  }

  public onStepSave(): void {
    let isAdditional = this.stepper.selectedIndex === this.additionalIndex;
    let isValid = this.formUtilsService.checkValidity(this.form);

    if (this.countIndexes?.indexOf(this.stepper.selectedIndex) > -1) {
      let countInx = this.stepper.selectedIndex - 1;
      let countForm = this.countForms.controls[countInx];
      if (countForm.value.request_time_to_pay || countForm.value.request_reduction) {
        countForm.patchValue({ plea: Plea.Guilty });
      }
      if (countForm.value.__skip) {
        countForm.patchValue({ ...this.getCountFormInitValue(this.ticket.counts[countInx]), __skip: true });
      }
      if (countForm.value.__apply_to_remaining_counts && countInx + 1 < this.countForms.length) {
        let value = this.countForms.controls[countInx].value;
        for (let i = countInx; i < this.countForms.length; i++) {
          this.countForms.controls[i].patchValue({ ...value, ...this.ticket.counts[i], __apply_to_remaining_counts: false })
        }
      }
      this.setAdditional();
    } else if (!isValid) {
      this.utilsService.scrollToErrorSection();
      this.toastService.openErrorToast(this.config.dispute_validation_error);
      return;
    }

    if (isAdditional) {
      this.noticeOfDispute = this.noticeOfDisputeService.getNoticeOfDispute({
        ...this.form.value,
        ...this.additionalForm.value,
        country: this.form.get("country").value, // disabled field is not available in this.form.value
        disputed_counts: this.countForms.value
      });
    } else {
      this.noticeOfDispute = null;
    }
  }

  private setAdditional() {
    this.getCountsActions();
    let fields = { ...this.additionFormFields };
    if (this.countsActions.request_reduction.length > 0 && fields.fine_reduction_reason[1].indexOf(Validators.required) < 0) {
      fields.fine_reduction_reason[1].push(Validators.required);
    }
    if (this.countsActions.request_time_to_pay.length > 0 && fields.time_to_pay_reason[1].indexOf(Validators.required) < 0) {
      fields.time_to_pay_reason[1].push(Validators.required);
    }
    this.additionalForm = this.formBuilder.group({
      ...fields,
    }, {
      validators: [...this.additionFormValidators]
    });
    this.additionalForm.markAsUntouched();
    this.additionalForm.markAsPristine();
  }

  public isValid(countInx?): boolean {
    let countForm = this.countForms?.controls[countInx]
    if (countForm) {
      let valid = countForm.valid || countForm.value.__skip;
      if (countForm.value.appear_in_court) {
        valid = valid && countForm.value.plea;
      } else if (countForm.value.appear_in_court === false) {
        valid = valid && (countForm.value.request_time_to_pay || countForm.value.request_reduction);
      }
      return valid && !this.isAllCountsSkipped;
    }
    return this.form.valid;
  }

  public onChangeRepresentedByLawyer(event: MatCheckboxChange) {
    if (event.checked) { // only append if selected
      this.additionalForm.addControl("legal_representation", this.legalRepresentationForm);
    } else {
      this.additionalForm.removeControl("legal_representation");
    }
    this.additionalForm.markAsUntouched();
  }

  public onChangeWitnessPresent(event: MatCheckboxChange) {
    if (event.checked) {
      this.additionalForm.controls.number_of_witness.setValidators([Validators.min(this.minWitnesses), Validators.max(this.maxWitnesses), Validators.required]);
    } else {
      this.additionalForm.controls.number_of_witness.clearValidators();
      this.additionalForm.controls.number_of_witness.updateValueAndValidity();
    }
  }

  public getToolTipDEata(data) {
    if (data) {
      let msg = "";
      this.languages.forEach(res => {
        if (res === data.value) {
          msg = res
        }
      })
      return msg;
    } else {
      return "please select a language";
    }
  }

  private get isAllCountsSkipped() {
    if (this.countForms.value.filter(i => i.__skip).length === this.countForms.length) {
      return true;
    }
    else {
      return false;
    }
  }

  onSkipChecked() {
    if (this.isAllCountsSkipped) {
      const data: DialogOptions = {
        titleKey: "Warning",
        actionType: "warn",
        messageKey: `You have selected "Skip this count, no action required" for all counts on your ticket. No request will be created. Please review your selection(s).`,
        actionTextKey: 'Close',
        cancelHide: true
      };
      this.dialog.open(ConfirmDialogComponent, { data });
    }
  }

  /**
   * @description
   * Submit the dispute
   */
  public submitDispute(): void {
    this.noticeOfDisputeService.createNoticeOfDispute(this.noticeOfDispute);
  }
}
