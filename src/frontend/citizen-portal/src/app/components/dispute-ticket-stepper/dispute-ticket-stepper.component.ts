import { AfterViewInit, Component, OnInit, ViewChild, ChangeDetectionStrategy } from "@angular/core";
import { FormArray, FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatStepper } from "@angular/material/stepper";
import { MatCheckboxChange } from "@angular/material/checkbox";
import { MatDialog } from "@angular/material/dialog";
import { TranslateService } from "@ngx-translate/core";
import { Subscription } from "rxjs";
import { cloneDeep } from "lodash";

import { ToastService } from "@core/services/toast.service";
import { UtilsService } from "@core/services/utils.service";
import { FormUtilsService } from "@core/services/form-utils.service";
import { FormControlValidators } from "@core/validators/form-control.validators";
import { ConfigService } from "@config/config.service";
import { Config, ProvinceConfig } from "@config/config.model";
import { Address } from "@shared/models/address.model";
import { ticketTypes } from "@shared/enums/ticket-type.enum";
import { AddressAutocompleteComponent } from "@shared/components/address-autocomplete/address-autocomplete.component";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { ConfirmDialogComponent } from "@shared/dialogs/confirm-dialog/confirm-dialog.component";
import { FormErrorStateMatcher } from "@shared/directives/form-error-state-matcher.directive";
import { ViolationTicket, DisputeCountPleaCode, DisputeRepresentedByLawyer, DisputeCountRequestCourtAppearance, DisputeCountRequestTimeToPay, DisputeCountRequestReduction, Language } from "app/api";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { DisputeService, NoticeOfDispute } from "app/services/dispute.service";
import { LookupsService } from "app/services/lookups.service";

@Component({
  selector: "app-dispute-ticket-stepper",
  templateUrl: "./dispute-ticket-stepper.component.html",
  styleUrls: ["./dispute-ticket-stepper.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DisputeTicketStepperComponent implements OnInit, AfterViewInit {
  @ViewChild(MatStepper) private stepper: MatStepper;
  @ViewChild(AddressAutocompleteComponent) private addressAutocomplete: AddressAutocompleteComponent;
  
  languages: Language[] = [];
  countries: Config<string>[] = [];
  allProvinces: ProvinceConfig[] = [];
  provinces: ProvinceConfig[];
  states: ProvinceConfig[];

  busy: Subscription;
  isMobile: boolean;
  previousButtonIcon = "keyboard_arrow_left";
  defaultLanguage: string;
  ticketTypes = ticketTypes;
  todayDate: Date = new Date();
  Plea = DisputeCountPleaCode;
  RepresentedByLawyer = DisputeRepresentedByLawyer;
  RequestCourtAppearance = DisputeCountRequestCourtAppearance;
  RequestTimeToPay = DisputeCountRequestTimeToPay;
  RequestReduction = DisputeCountRequestReduction;
  selected = null;

  form: FormGroup;
  countForms: FormArray;
  additionalForm: FormGroup;
  legalRepresentationForm: FormGroup;
  ticket: ViolationTicket;
  noticeOfDispute: NoticeOfDispute;
  ticketType;
  matcher = new FormErrorStateMatcher();

  // Disputant
  showManualButton: boolean = true;
  showAddressFields: boolean = true; // temporary preset for testing
  optOut: boolean = false;

  // Count
  countIndexes: number[];

  // Additional
  countsActions: any;
  customWitnessOption = false;
  minWitnesses = 1;
  maxWitnesses = 99;
  additionalIndex: number;

  // Summary
  declared = false;

  // Consume from the service
  private ticketFormFields = this.disputeService.ticketFormFields;
  private countFormFields = this.disputeService.countFormFields;
  private countFormDefaultValue = this.disputeService.countFormDefaultValue;
  private additionFormFields = this.disputeService.additionFormFields;
  private additionFormValidators = this.disputeService.additionFormValidators;
  // private additionFormDefaultValue = this.disputeService.additionFormDefaultValue;
  private legalRepresentationFields = this.disputeService.legalRepresentationFields;

  constructor(
    private formBuilder: FormBuilder,
    private dialog: MatDialog,
    private violationTicketService: ViolationTicketService,
    private disputeService: DisputeService,
    private utilsService: UtilsService,
    private formUtilsService: FormUtilsService,
    private translateService: TranslateService,
    private toastService: ToastService,
    private config: ConfigService,
    private lookups: LookupsService
  ) {
    // config or static
    this.isMobile = this.utilsService.isMobile();
    this.defaultLanguage = this.translateService.getDefaultLang();
  }

  ngOnInit(): void {
    this.ticket = this.violationTicketService.ticket;
    if (!this.ticket) {
      this.violationTicketService.goToFind();
      return;
    }
    this.ticketType = this.violationTicketService.ticketType;
    
    this.languages = this.lookups.languages;
    this.countries = this.config.countries;
    this.allProvinces = this.config.provinces;
    this.provinces = this.config.provinces.filter(x => x.countryCode == "CA" && x.code != "BC");
    this.states = this.config.provinces.filter(x => x.countryCode == "US");

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

    // take info from ticket, convert dl number to string
    Object.keys(this.ticket).forEach(key => {
      this.ticket[key] && this.form.get(key)?.patchValue(this.ticket[key]);
    });
    this.ticket.drivers_licence_number && this.form.controls["drivers_licence_number"].setValue(this.ticket.drivers_licence_number.toString());
    this.legalRepresentationForm = this.formBuilder.group(this.legalRepresentationFields);

    this.countIndexes = this.ticket.counts.map(i => i.count_no);
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
        stepElement.scrollIntoView({ block: "start", inline: "nearest", behavior: "smooth" });
      }, 250);
    }
  }

  onCountryChange(country) {
    setTimeout(() => {
      this.form.get("postal_code").setValidators([Validators.maxLength(6)]);
      this.form.get("address_province").setValidators([Validators.maxLength(30)]);
      this.form.get("home_phone_number").setValidators([Validators.maxLength(20)]);
      this.form.get("drivers_licence_number").setValidators([Validators.maxLength(20)]);
      this.form.get("drivers_licence_province").setValidators([Validators.maxLength(30)]);

      if (country === "Canada" || country === "United States") {
        this.form.get("address_province").addValidators([Validators.required]);
        this.form.get("postal_code").addValidators([Validators.required]);
        this.form.get("home_phone_number").addValidators([Validators.required, FormControlValidators.phone]);
        this.form.get("drivers_licence_number").addValidators([Validators.required]);
        this.form.get("drivers_licence_province").addValidators([Validators.required]);
      }

      this.form.get("postal_code").updateValueAndValidity();
      this.form.get("address_province").updateValueAndValidity();
      this.form.get("home_phone_number").updateValueAndValidity();
      this.form.get("drivers_licence_number").updateValueAndValidity();
      this.form.get("drivers_licence_province").updateValueAndValidity();
    }, 0);
  }

  onDLProvinceChange(province) {
    setTimeout(() => {
      if (province == "BC") {
        this.form.get("drivers_licence_number").setValidators([Validators.maxLength(9)]);
        this.form.get("drivers_licence_number").addValidators([Validators.minLength(7)]);
      } else {
        this.form.get("drivers_licence_number").setValidators([Validators.maxLength(20)]);
      }
      if (this.form.get("country").value === "United States" || this.form.get("country").value === "Canada") {
        this.form.get("drivers_licence_number").addValidators([Validators.required]);
      }
      this.form.get("drivers_licence_number").updateValueAndValidity();
    }, 0)
  }

  private getCountFormInitValue(count) {
    return { ...this.countFormDefaultValue, ...count };
  }

  private getCountsActions() {
    this.countsActions = this.disputeService.getCountsActions(this.countForms.value);
  }

  onAddressAutocomplete({ countryCode, provinceCode, postalCode, address, city }: Address): void {
    // Will be implemented
  }

  onStepCancel(): void {
    this.violationTicketService.goToInitiateResolution();
  }

  onAttendHearingChange(countForm: FormGroup, event): void {
    countForm.patchValue({ ...this.countFormDefaultValue, request_court_appearance: event.value, __skip: false });
  }

  onStepSave(): void {
    let isAdditional = this.stepper.selectedIndex === this.additionalIndex;
    let isValid = this.formUtilsService.checkValidity(this.form);

    if (this.countIndexes?.indexOf(this.stepper.selectedIndex) > -1) {
      let countInx = this.stepper.selectedIndex - 1;
      let countForm = this.countForms.controls[countInx];
      if (countForm.value.request_time_to_pay === this.RequestTimeToPay.Y || countForm.value.request_reduction === this.RequestReduction.Y) {
        countForm.patchValue({ plea_cd: DisputeCountPleaCode.G });
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
      this.noticeOfDispute = this.disputeService.getNoticeOfDispute({
        ...this.form.value,
        ...this.additionalForm.value,
        ...this.legalRepresentationForm.value,
        country: this.form.get("country").value, // disabled field is not available in this.form.value
        dispute_counts: this.countForms.value
      });
    } else {
      this.noticeOfDispute = null;
    }
  }

  private setAdditional() {
    this.getCountsActions();
    let fields = cloneDeep(this.additionFormFields);
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
  }

  isValid(countInx?): boolean {
    let countForm = this.countForms?.controls[countInx]
    if (countForm) {
      let valid = countForm.valid || countForm.value.__skip;
      if (countForm.value.request_court_appearance === this.RequestCourtAppearance.Y) {
        valid = valid && (countForm.value.plea_cd === this.Plea.G || countForm.value.plea_cd === this.Plea.N);
      } else if (countForm.value.request_court_appearance === this.RequestCourtAppearance.N) {
        valid = valid && ((countForm.value.request_time_to_pay === this.RequestTimeToPay.Y) || (countForm.value.request_reduction === this.RequestReduction.Y));
      }
      return valid && !this.isAllCountsSkipped;
    }
    return this.form.valid;
  }

  onChangeRepresentedByLawyer(event: MatCheckboxChange) {
    this.additionalForm.markAsUntouched();
  }

  onChangeWitnessPresent(event: MatCheckboxChange) {
    if (event.checked) {
      this.additionalForm.controls.witness_no.setValidators([Validators.min(this.minWitnesses), Validators.max(this.maxWitnesses), Validators.required]);
    } else {
      this.additionalForm.controls.witness_no.clearValidators();
      this.additionalForm.controls.witness_no.updateValueAndValidity();
    }
  }

  getToolTipDEata(data) {
    if (data) {
      let msg = "";
      this.lookups.languages.forEach(res => {
        if (res.code === data.value) {
          msg = res.description
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
        actionTextKey: "Close",
        cancelHide: true
      };
      this.dialog.open(ConfirmDialogComponent, { data });
    }
  }

  onOptOut() {
    if (this.optOut) {
      this.form.get("email_address").setValue(null);
      this.form.get("email_address").disable();
    } else {
      this.form.get("email_address").enable();
    }
  }

  /**
   * @description
   * Submit the dispute
   */
  public submitDispute(): void {
    this.disputeService.createNoticeOfDispute(this.noticeOfDispute);
  }
}
